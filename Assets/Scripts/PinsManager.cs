using System.Collections;
using UnityEngine;
using Bowling;

public class PinsManager : MonoBehaviour {
    [SerializeField] float pinMovementCheckingInterval;
    [SerializeField] float pinMovementDistanceThreshold;
    [SerializeField] float dudKickTimeout;

    Vector3[] pinOriginalPositions;
    Quaternion[] pinOriginalRotations;

    static float movedPins;

    public static float MovedPins { get => movedPins; }
    
    void Awake() {
        pinOriginalPositions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            pinOriginalPositions[i] = transform.GetChild(i).transform.position;
        }

        pinOriginalRotations = new Quaternion[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            pinOriginalRotations[i] = transform.GetChild(i).transform.rotation;
        }

        GameManager.StateChanged.AddListener(GameStateChanged);
    }

    void GameStateChanged(State state) {
        switch (state) {
            case State.StartMenu:
                ResetPins();
                break;
            case State.BallKicked:
                StartCoroutine(nameof(FirstPinMovementChecking));
                break;
            case State.FirstPinMoved:
                StartCoroutine(nameof(PinsMovementChecking));
                break;
        }
    }

    void ResetPins() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).transform.position = pinOriginalPositions[i];
        }
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).transform.rotation = pinOriginalRotations[i];
        }
    }

    IEnumerator FirstPinMovementChecking() {
        Invoke(nameof(StopFirstPinMovementChecking), dudKickTimeout);

        while (true) {
            for (int i = 0; i < transform.childCount; i++) {
                if (Vector3.Distance(transform.GetChild(i).position, pinOriginalPositions[i]) > pinMovementDistanceThreshold) {
                    goto FirstPinMoved;
                }
            }
            goto NoPinsMoved;

            NoPinsMoved:
            yield return null;
            continue;

            FirstPinMoved:
            StopFirstPinMovementChecking();
            break;
        }
    }

    void StopFirstPinMovementChecking() {
        CancelInvoke(nameof(StopFirstPinMovementChecking));
        StopCoroutine(nameof(FirstPinMovementChecking));
        GameManager.State = State.FirstPinMoved;
    }

    IEnumerator PinsMovementChecking() {
        StartCoroutine(nameof(MovedPinsTallying));

        Vector3[] pinLastPositions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            pinLastPositions[i] = transform.GetChild(i).position;
        }

        while (true) {
            // Don't check every frame (differences become too small to detect)
            yield return new WaitForSeconds(pinMovementCheckingInterval);

            for (int i = 0; i < transform.childCount; i++) {
                if (Vector3.Distance(transform.GetChild(i).position, pinLastPositions[i]) > pinMovementDistanceThreshold) {
                    // If we find a pin that's moved greater than distanceToCheck, refill array of last pin positions
                    for (int x = 0; x < transform.childCount; x++) {
                        pinLastPositions[x] = transform.GetChild(x).position;
                    }
                    // and keep checking
                    goto FoundOne;
                }
            }
            // else, inform GameManager that pins have stopped moving, and stop checking
            goto FoundNone;

            FoundOne:
                continue;

            FoundNone:
                StopPinsMovementChecking();
                break;
        }
    }

    void StopPinsMovementChecking() {
        StopCoroutine(nameof(MovedPinsTallying));
        GameManager.State = State.FinishScreen;
    }

    IEnumerator MovedPinsTallying() {
        movedPins = 0;
        while (true) {
            float amountOfMovedPins = 0;
            for (int i = 0; i < transform.childCount; i++) {
                if (Vector3.Distance(transform.GetChild(i).position, pinOriginalPositions[i]) > pinMovementDistanceThreshold)
                    amountOfMovedPins++;
            }
            movedPins = amountOfMovedPins;
            yield return null;
        }
    }
}