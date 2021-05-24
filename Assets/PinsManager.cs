using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinsManager : MonoBehaviour
{
    public static PinsManager Instance;
    Vector3[] pinOriginalPositions;
    Quaternion[] pinOriginalRotations;
    // Start is called before the first frame update
    Vector3[] pinLastPositions;

    const float distanceToCheck = 0.2f;
    const float checkingInterval = 0.5f;

    void Awake() {
        if (Instance != null)
            Debug.LogError("Error! There is more than one PinsManager in the scene!");
        else
            Instance = this;

        pinOriginalPositions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            pinOriginalPositions[i] = transform.GetChild(i).transform.position;
        }
        pinOriginalRotations = new Quaternion[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            pinOriginalRotations[i] = transform.GetChild(i).transform.rotation;
        }
    }

    void Start()
    {
        
    }

    public void ResetPins() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).transform.position = pinOriginalPositions[i];
        }
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).transform.rotation = pinOriginalRotations[i];
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
            pinLastPositions = new Vector3[transform.childCount];
            for (int i = 0; i < transform.childCount; i++) {
                pinLastPositions[i] = transform.GetChild(i).transform.position;
            }
            Invoke("ContinouslyCheckIfPinsHaveStoppedMoving", checkingInterval);
            return;
        }
            
        for (int i = 0; i < transform.childCount; i++) {
            if (Vector3.Distance(transform.GetChild(i).transform.position, pinLastPositions[i]) > distanceToCheck) {
                for (int x = 0; x < transform.childCount; x++) {
                    pinLastPositions[x] = transform.GetChild(x).transform.position;
                }
                Invoke("ContinouslyCheckIfPinsHaveStoppedMoving", checkingInterval);
                return;
            }
        }

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
