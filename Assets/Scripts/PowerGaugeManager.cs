using System.Collections;
using UnityEngine;
using Bowling;

public class PowerGaugeManager : MonoBehaviour {
    [SerializeField] float pipMoveSpeed;
    [SerializeField] RectTransform powerFullMaskRect;
    [SerializeField] RectTransform powerFullRect;
    [SerializeField] RectTransform pipBaseRect;

    // Manually checked, depends on gauge image graphic
    readonly float[] HardCodedPips = { 0.0549f, 0.1172f, 0.1818f, 0.2540f, 0.3404f, 0.4268f, 0.5267f, 0.6233f, 0.7238f, 0.8358f, 0.9644f };

    float gaugeBaseHeight;

    void Awake() {
        gaugeBaseHeight = GetComponent<RectTransform>().rect.height;

        ConvertRectToBottomBase(powerFullMaskRect);
        ConvertRectToBottomBase(powerFullRect);
        ConvertRectToBottomBase(pipBaseRect);
        SetRectHeightToPipLevel(powerFullMaskRect, 0);
        SetRectHeightToPipLevel(powerFullRect, 1f);
        SetRectHeightToPipLevel(pipBaseRect, 0);

        GameManager.StateChanged.AddListener(GameStateChanged);
        GameManager.PowerChanged.AddListener(PowerChanged);
        GameManager.ManualKick.AddListener(StopPipMovement);
    }

    void GameStateChanged(State state) {
        switch (state) {
            case State.StartMenu:
                pipBaseRect.gameObject.SetActive(false);
                SetRectHeightToPipLevel(pipBaseRect, 0);
                break;
            case State.KickingBall:
                StartCoroutine(nameof(PipMovement), GameManager.Power);
                break;
        }
    }

    void ConvertRectToBottomBase(RectTransform rectTransform) {
        Vector2 currentAnchor;
        currentAnchor = rectTransform.anchorMax;
        currentAnchor.y = 0;
        rectTransform.anchorMax = currentAnchor;
    }

    void SetRectHeightToPipLevel(RectTransform rectTransform, int pipLevel) {
        SetRectHeightToPipLevel(rectTransform, HardCodedPips[pipLevel]);
    }
    void SetRectHeightToPipLevel(RectTransform rectTransform, float pipPercentage) {
        float heightTarget = gaugeBaseHeight * pipPercentage;

        Vector2 currentSize;
        Vector2 currentPosition;

        currentSize = rectTransform.sizeDelta;
        currentSize.y = heightTarget;
        rectTransform.sizeDelta = currentSize;

        currentPosition = rectTransform.anchoredPosition;
        currentPosition.y = heightTarget / 2;
        rectTransform.anchoredPosition = currentPosition;
    }

    void PowerChanged(int power) {
        SetRectHeightToPipLevel(powerFullMaskRect, power);
    }

    IEnumerator PipMovement(int moveCap) {
        pipBaseRect.gameObject.SetActive(true);
        bool isMovingUp = true;
        float floor = HardCodedPips[0];
        float currentLocation = floor;
        float ceiling = HardCodedPips[moveCap];

        while (true) {
            float moveAmount = pipMoveSpeed * GameManager.PowerPercent * Time.deltaTime;

            if (isMovingUp) {
                currentLocation += moveAmount;
                if (currentLocation >= ceiling) {
                    currentLocation = ceiling;
                    isMovingUp = false;
                }
            }
            else if (!isMovingUp) {
                currentLocation -= moveAmount;
                if (currentLocation <= floor) {
                    currentLocation = floor;
                    isMovingUp = true;
                }
            }

            SetRectHeightToPipLevel(pipBaseRect, currentLocation);
            yield return null;
        }
    }

    void StopPipMovement() {
        StopCoroutine(nameof(PipMovement));
    }
}