using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    const float playerSpeed = 3;

    [SerializeField] Renderer playerRenderer;
    [SerializeField] Renderer ballRenderer;
    [SerializeField] public Animator playerAnimator;
    [SerializeField] Material greenMaterial;
    [SerializeField] Material yellowMaterial;
    [SerializeField] Material redMaterial;
    [SerializeField] GameObject ball;


    GameManager.Color playerColor = GameManager.Color.Green;
    bool isMousedDown = false;
    Vector3 mousedDownLocation;
    float origXLocation;
    float baseXLocation;
    const float movementRangeX = 1;

    Vector3 originalBallPosition;
    Quaternion originalBallRotation;
    Vector3 originalPlayerPosition;

    bool hasBallBeenKicked = false;

    public GameManager.Color color {
        get => playerColor;
    }

    void Awake() {
        if (Instance != null)
            Debug.LogError("Error! There is more than one PlayerController in the scene!");
        else
            Instance = this;

        baseXLocation = transform.position.x;
        originalBallPosition = ball.transform.position;
        originalPlayerPosition = transform.position;
        originalBallRotation = ball.transform.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void KickBall() {
        Vector3 currentPosition = ball.transform.position;
        currentPosition.y += 0.2f;
        ball.transform.position = currentPosition;

        ball.GetComponent<Rigidbody>().useGravity = true;
        ball.GetComponent<Rigidbody>().isKinematic = false;
        ball.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        ball.GetComponent<Rigidbody>().AddForce(ball.transform.forward * 900);
    }

    public void UnkickBall() {
        ball.transform.position = originalBallPosition;
        ball.transform.rotation = originalBallRotation;
        ball.GetComponent<Rigidbody>().useGravity = false;
        ball.GetComponent<Rigidbody>().isKinematic = true;
        ball.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
        hasBallBeenKicked = false;
    }

    public void ResetPlayer() {
        transform.position = originalPlayerPosition;
        UnkickBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.state == GameManager.GameState.Running) {

        

            if (Input.GetMouseButtonDown(0) && isMousedDown == false) {
            isMousedDown = true;
            mousedDownLocation = Input.mousePosition;
            origXLocation = transform.position.x;
        }

        if (Input.GetMouseButtonUp(0)) {
            isMousedDown = false;
            mousedDownLocation = Vector3.zero;
        }

        if (isMousedDown) {
            float rawMove = mousedDownLocation.x - Input.mousePosition.x;
            float percentMove = rawMove / Screen.width;
            Vector3 currentLocation = transform.position;
            currentLocation.x = origXLocation - (percentMove * 3);
            if (currentLocation.x > (baseXLocation+ movementRangeX)) {
                currentLocation.x = (baseXLocation + movementRangeX);
            }
            else if (currentLocation.x < (baseXLocation - movementRangeX)) {
                currentLocation.x = (baseXLocation - movementRangeX);
            }
            transform.position = currentLocation;
        }

            transform.position = transform.position + ((transform.forward * playerSpeed) * Time.deltaTime);
        }
        else if (GameManager.state == GameManager.GameState.KickingBall) {
            ball.transform.position = ball.transform.position + ((ball.transform.forward * playerSpeed/10) * Time.deltaTime);
        }

    }

    public void SetPlayerColor(GameManager.Color color) {
        Material materialToAssign = null;

        switch (color) {
            case GameManager.Color.Green:
                materialToAssign = greenMaterial;
                break;
            case GameManager.Color.Yellow:
                materialToAssign = yellowMaterial;
                break;
            case GameManager.Color.Red:
                materialToAssign = redMaterial;
                break;
        }

        Material[] currentMaterials = playerRenderer.materials;
        currentMaterials[2] = materialToAssign;
        playerRenderer.materials = currentMaterials;
        currentMaterials = ballRenderer.materials;
        currentMaterials[0] = materialToAssign;
        ballRenderer.materials = currentMaterials;

        playerColor = color;
    }

    public void OnTriggerEnter(Collider other) {
        switch (other.tag) {
            case "Change Color Wall":
                SetPlayerColor(other.GetComponent<ChangeColorWall>().color);
                break;
            case "Pickup Ball":
                GameManager.Instance.PickUpBall(other.GetComponent<PickupBall>().color);
                break;
            case "Goal Post":
                if (hasBallBeenKicked == true)
                    break;
                GameManager.Instance.SetGameState(GameManager.GameState.KickingBall);
                hasBallBeenKicked = true;
                break;
        }
    }
}
