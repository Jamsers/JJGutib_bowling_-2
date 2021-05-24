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


    ChangeColorWall.WallColor playerColor = ChangeColorWall.WallColor.Green;
    bool isMousedDown = false;
    Vector3 mousedDownLocation;
    float origXLocation;
    float baseXLocation;
    const float movementRangeX = 1;

    bool hasBallBeenKicked = false;

    void Awake() {
        if (Instance != null)
            Debug.LogError("Error! There is more than one PlayerController in the scene!");
        else
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        baseXLocation = transform.position.x;
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

    void SetPlayerColor(ChangeColorWall.WallColor color) {
        Material materialToAssign = null;

        switch (color) {
            case ChangeColorWall.WallColor.Green:
                materialToAssign = greenMaterial;
                break;
            case ChangeColorWall.WallColor.Yellow:
                materialToAssign = yellowMaterial;
                break;
            case ChangeColorWall.WallColor.Red:
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

    void PickUpBall(ChangeColorWall.WallColor color) {
        if (playerColor == color) {
            Debug.Log("ball increased");
        }
        else {
            Debug.Log("ball decreased");
        }
    }

    public void OnTriggerEnter(Collider other) {
        switch (other.tag) {
            case "Change Color Wall":
                SetPlayerColor(other.GetComponent<ChangeColorWall>().color);
                break;
            case "Pickup Ball":
                PickUpBall(other.GetComponent<PickupBall>().color);
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
