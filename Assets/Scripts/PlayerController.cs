using System.Collections;
using UnityEngine;
using Bowling;

public class PlayerController : MonoBehaviour {
    [Header("Celebration")]
    [SerializeField] int celebrationThreshold;

    [Header("Movement")]
    [SerializeField] float playerSpeed;
    [SerializeField] float ballRotateSpeed;
    [SerializeField] float leftRightMovementCap;
    [SerializeField] float leftRightMovementSpeed;

    [Header("Kicking Ball Modifiers")]
    [SerializeField] float kickingBallSpeedMultiplier;
    [SerializeField] float kickingBallExtraPush;
    [SerializeField] float afterManualKickMultiplier;
    [SerializeField] float ballLiftBeforeKick;
    [SerializeField] float animationSpeedBeforeManualKick;
    [SerializeField] float animationSpeedAfterManualKick;

    [Header("Kick Force")]
    [SerializeField] float baseKickForce;
    [SerializeField] float maxAdditionalKickForce;

    [Header("Ball Scaling")]
    [SerializeField] float maxBallScale;
    [SerializeField] float maxBallLift;
    [SerializeField] float maxBallForward;

    [Header("Renderers")]
    [SerializeField] Renderer playerRenderer;
    [SerializeField] Renderer ballRenderer;

    [Header("Animators")]
    [SerializeField] public Animator playerAnimator;

    [Header("Materials")]
    [SerializeField] Material greenMaterial;
    [SerializeField] Material yellowMaterial;
    [SerializeField] Material redMaterial;

    [Header("Transforms")]
    [SerializeField] GameObject ball;
    [SerializeField] Transform camera;
    [SerializeField] Transform model;

    [Header("Camera Targets")]
    [SerializeField] CameraTarget cameraKicking;
    [SerializeField] CameraTarget cameraCelebration;

    Vector3 originalPlayerPosition;
    Vector3 originalModelPosition;
    Quaternion originalModelRotation;
    Vector3 originalBallPosition;
    Quaternion originalBallRotation;
    Vector3 originalBallScale;
    Vector3 originalCameraPosition;
    Quaternion originalCameraRotation;

    bool isFirstInit = true;
    bool goalPostTriggerTimeout;
    bool manualKicked;

    [System.Serializable]
    struct CameraTarget {
        public Transform target;
        public float time;
    }

    void Awake() {
        originalPlayerPosition = transform.position;
        originalModelPosition = model.position;
        originalModelRotation = model.rotation;
        originalBallPosition = ball.transform.localPosition;
        originalBallRotation = ball.transform.rotation;
        originalBallScale = ball.transform.localScale;
        originalCameraPosition = camera.position;
        originalCameraRotation = camera.rotation;

        GameManager.StateChanged.AddListener(GameStateChanged);
        GameManager.PowerChanged.AddListener(SetBallSize);
        GameManager.ColorChanged.AddListener(SetPlayerColor);
        GameManager.ManualKick.AddListener(PlayerKickedBall);
    }

    void GameStateChanged(State state) {
        switch (state) {
            case State.StartMenu:
                if (isFirstInit)
                    isFirstInit = false;
                else
                    playerAnimator.SetTrigger("switchToIdle");
                goalPostTriggerTimeout = false;
                ResetPlayer();
                break;
            case State.Running:
                playerAnimator.SetTrigger("switchToRun");
                StartCoroutine(nameof(TakeXInput));
                StartCoroutine(nameof(MovePlayerForward), 1f);
                break;
            case State.KickingBall:
                playerAnimator.SetFloat("kickSpeed", animationSpeedBeforeManualKick);
                playerAnimator.SetTrigger("switchToKick");
                manualKicked = false;

                StopCoroutine(nameof(MovePlayerForward));

                StartCoroutine(nameof(MovePlayerForward), kickingBallSpeedMultiplier);
                StartCoroutine(nameof(KickAnimationCompensation), kickingBallExtraPush);
                StartCoroutine(nameof(MoveCameraTo), cameraKicking);
                StartCoroutine(nameof(WaitForManualKick));
                break;
            case State.BallKicked:
                playerAnimator.SetFloat("kickSpeed", 1f);
                KickBall();

                StopCoroutine(nameof(MovePlayerForward));
                StopCoroutine(nameof(KickAnimationCompensation));
                StopCoroutine(nameof(MoveCameraTo));

                StartCoroutine(nameof(MoveCameraTo), cameraCelebration);
                break;
            case State.FinishScreen:
                if (PinsManager.MovedPins >= celebrationThreshold)
                    playerAnimator.SetTrigger("switchToJump");
                else
                    playerAnimator.SetTrigger("switchToIdle");
                break;
        }
    }

    void ResetPlayer() {
        transform.position = originalPlayerPosition;
        model.position = originalModelPosition;
        model.rotation = originalModelRotation;
        camera.position = originalCameraPosition;
        camera.rotation = originalCameraRotation;
        UnkickBall();
    }

    void KickBall() {
        Vector3 currentPosition = ball.transform.position;
        currentPosition.y += ballLiftBeforeKick;
        ball.transform.position = currentPosition;
        ball.GetComponent<Rigidbody>().useGravity = true;
        ball.GetComponent<Rigidbody>().isKinematic = false;
        ball.GetComponent<Rigidbody>().AddForce(ball.transform.forward * (baseKickForce + (maxAdditionalKickForce * PowerGaugeManager.PipPower)));
    }

    void UnkickBall() {
        ball.transform.localPosition = originalBallPosition;
        ball.transform.rotation = originalBallRotation;
        ball.transform.localScale = originalBallScale;
        ball.GetComponent<Rigidbody>().useGravity = false;
        ball.GetComponent<Rigidbody>().isKinematic = true;
    }

    IEnumerator WaitForManualKick() {
        while (GameManager.State == State.KickingBall) {
            if (Input.GetMouseButtonDown(0)) {
                break;
            }
            yield return null;
        }
        GameManager.ManualKick.Invoke();
    }

    IEnumerator TakeXInput() {
        bool isStored = false;
        float storedXPosition = 0;
        Vector3 storedMousePosition = Vector3.zero;

        while (GameManager.State == State.Running) {
            // If mouse/finger is up, invalidate stored data and don't do anything
            if (Input.GetMouseButton(0) == false) {
                isStored = false;
                goto End;
            }
                
            // Store location of cursor/finger and player transform, used as base for movement
            if (isStored == false) {
                storedXPosition = transform.position.x;
                storedMousePosition = Input.mousePosition;
                isStored = true;
                goto End;
            }

            // Use percentage rather than raw numbers to ensure consistent movement across resolutions
            float rawMove = storedMousePosition.x - Input.mousePosition.x;
            float percentMove = rawMove / Screen.width;

            Vector3 currentLocation = transform.position;
            currentLocation.x = storedXPosition - (percentMove * leftRightMovementSpeed);

            float maxCap = originalPlayerPosition.x + leftRightMovementCap;
            float minCap = originalPlayerPosition.x - leftRightMovementCap;

            // Cap movement range
            if (currentLocation.x > maxCap)
                currentLocation.x = maxCap;
            else if (currentLocation.x < minCap)
                currentLocation.x = minCap;

            transform.position = currentLocation;

            End:
            yield return null;
        }
    }

    IEnumerator MovePlayerForward(float speedMultiplier) {
        float moveSpeedMultiplied = playerSpeed * speedMultiplier;
        float rotateSpeedMultiplied = ballRotateSpeed * speedMultiplier;
        while (true) {
            transform.position = transform.position + (moveSpeedMultiplied * Time.deltaTime * transform.forward);
            Transform ballModel = ball.transform.GetChild(0);
            ballModel.Rotate(rotateSpeedMultiplied * Time.deltaTime, 0, 0);
            yield return null;
        }
    }

    // Adjustable push forward of ball while kicking to match up with player (prevent clipping/lack of contact on kick)
    IEnumerator KickAnimationCompensation(float speed) {
        while (true) {
            if (manualKicked) {
                speed *= afterManualKickMultiplier;
                manualKicked = false;
            }
            ball.transform.position = ball.transform.position + (speed * Time.deltaTime * ball.transform.forward);
            yield return null;
        }
    }

    void PlayerKickedBall() {
        playerAnimator.SetFloat("kickSpeed", animationSpeedAfterManualKick);
        manualKicked = true;
    }

    void SetBallSize(int power) {
        float percentage = power * 0.1f;
        Vector3 maxBallScaleVector = new Vector3(maxBallScale, maxBallScale, maxBallScale);
        ball.transform.localScale = Vector3.Lerp(originalBallScale, maxBallScaleVector, percentage);

        Vector3 currentPosition = ball.transform.localPosition;
        currentPosition.y = Mathf.Lerp(originalBallPosition.y, originalBallPosition.y + maxBallLift, percentage);
        currentPosition.z = Mathf.Lerp(originalBallPosition.z, originalBallPosition.z + maxBallForward, percentage);
        ball.transform.localPosition = currentPosition;
    }

    IEnumerator MoveCameraTo(CameraTarget cameraTarget) {
        Vector3 currentCameraPostion = camera.position;
        Quaternion currentCameraRotation = camera.rotation;
        float timeStarted = Time.time;
        float lerpPercent = 0;

        while (lerpPercent < 1) {
            lerpPercent = (Time.time - timeStarted) / cameraTarget.time;
            float smoothedLerp = Mathf.SmoothStep(0, 1, lerpPercent);
            camera.position = Vector3.Lerp(currentCameraPostion, cameraTarget.target.position, smoothedLerp);
            camera.rotation = Quaternion.Lerp(currentCameraRotation, cameraTarget.target.rotation, smoothedLerp);
            yield return null;
        }

        camera.position = cameraTarget.target.position;
        camera.rotation = cameraTarget.target.rotation;
    }

    void SetPlayerColor(Bowling.Color color) {
        Material materialToAssign = null;

        switch (color) {
            case Bowling.Color.Green:
                materialToAssign = greenMaterial;
                break;
            case Bowling.Color.Yellow:
                materialToAssign = yellowMaterial;
                break;
            case Bowling.Color.Red:
                materialToAssign = redMaterial;
                break;
        }

        Material[] currentMaterials = playerRenderer.materials;
        currentMaterials[2] = materialToAssign;
        playerRenderer.materials = currentMaterials;

        currentMaterials = ballRenderer.materials;
        currentMaterials[0] = materialToAssign;
        ballRenderer.materials = currentMaterials;
    }

    public void OnTriggerEnter(Collider other) {
        switch (other.tag) {
            case "Change Color Wall":
                GameManager.Color = other.GetComponent<ChangeColorWall>().Color;
                break;
            case "Pickup Ball":
                GameManager.PickedUpBall.Invoke(other.GetComponent<PickupBall>().Color);
                other.GetComponent<PickupBall>().HideBall();
                break;
            case "Goal Post":
                GoalPostTrigger();
                break;
        }
    }

    void GoalPostTrigger() {
        if (goalPostTriggerTimeout)
            return;

        goalPostTriggerTimeout = true;
        GameManager.State = State.KickingBall;
    }

    public void Kicked() {
        GameManager.State = State.BallKicked;
    }
}