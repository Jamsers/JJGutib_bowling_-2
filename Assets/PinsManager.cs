using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinsManager : MonoBehaviour
{
    public static PinsManager Instance;
    Vector3[] pinOriginalPositions;
    // Start is called before the first frame update
    Vector3[] pinLastPositions;

    const float distanceToCheck = 0.2f;
    const float checkingInterval = 0.5f;

    void Awake() {
        if (Instance != null)
            Debug.LogError("Error! There is more than one PinsManager in the scene!");
        else
            Instance = this;
    }

    void Start()
    {
        pinOriginalPositions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            pinOriginalPositions[i] = transform.GetChild(i).transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.state == GameManager.GameState.BallKicked) {
            if (FirstPinHasMoved()) {
                GameManager.Instance.SetGameState(GameManager.GameState.FirstPinMoved);
            }
        }
        if (GameManager.state == GameManager.GameState.FirstPinMoved) {
           
        }
    }

    void PinsHaveStoppedMoving() {
        GameManager.Instance.SetGameState(GameManager.GameState.FinishScreen);
    }

    public void ContinouslyCheckIfPinsHaveStoppedMoving () {
        if (pinLastPositions == null) {
            Debug.Log("Filling in pinlastpositions");
            pinLastPositions = new Vector3[transform.childCount];
            for (int i = 0; i < transform.childCount; i++) {
                pinLastPositions[i] = transform.GetChild(i).transform.position;
            }
            Invoke("ContinouslyCheckIfPinsHaveStoppedMoving", checkingInterval);
            return;
        }
            
        for (int i = 0; i < transform.childCount; i++) {
            if (Vector3.Distance(transform.GetChild(i).transform.position, pinLastPositions[i]) > distanceToCheck) {
                Debug.Log("Found one beyond " + distanceToCheck);
                for (int x = 0; x < transform.childCount; x++) {
                    pinLastPositions[x] = transform.GetChild(x).transform.position;
                }
                Invoke("ContinouslyCheckIfPinsHaveStoppedMoving", checkingInterval);
                return;
            }
        }

        Debug.Log("Pins stopped");
        PinsHaveStoppedMoving();
    }

    bool FirstPinHasMoved() {
        bool returnValue = false;
        for (int i = 0; i < transform.childCount; i++) {
            if (Vector3.Distance(transform.GetChild(i).transform.position, pinOriginalPositions[i]) > distanceToCheck) {
                returnValue = true;
                break;
            }
        }
        return returnValue;
    }
}
