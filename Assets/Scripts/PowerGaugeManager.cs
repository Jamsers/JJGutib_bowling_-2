using System.Collections;
using UnityEngine;
using Bowling;

public class PowerGaugeManager : MonoBehaviour {
    [SerializeField] float pipMoveSpeed;
    [SerializeField] float speedFloor;
    [SerializeField] RectTransform powerFullMaskRect;
    [SerializeField] RectTransform powerFullRect;
    [SerializeField] RectTransform pipBaseRect;

    // Manually checked, depends on gauge image graphic due to non linear notches
    readonly float[] HardCodedPips = { 0.0549f, 0.1172f, 0.1818f, 0.2540f, 0.3404f, 0.4268f, 0.5267f, 0.6233f, 0.7238f, 0.8358f, 0.9644f };

    float gaugeBaseHeight;
    static float gaugePipPower;

    public static float PipPower { get => gaugePipPower; }

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

    // Convert rect anchors to bottom at runtime so that images can be freely adjusted in the editor
    void ConvertRectToBottomBase(RectTransform rectTransform) {
        Vector2 currentAnchor;
        currentAnchor = rectTransform.anchorMax;
        currentAnchor.y = 0;
        rectTransform.anchorMax = currentAnchor;
    }

    // Grows and shrinks image based on current bottom as anchor
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

        // Make speed slower if gauge level is lower
        float speedRemainder = 1 - speedFloor;
        float moveCapPercentage = moveCap * 0.1f;
        float speedMultiplier = speedFloor + speedRemainder * moveCapPercentage;

        bool isMovingUp = true;
        float floor = HardCodedPips[0];
        float ceiling = HardCodedPips[moveCap];
        float lerpPercent = 0;

        while (true) {
            float moveAmount = pipMoveSpeed * speedMultiplier * Time.deltaTime;

            // move up and down
            if (isMovingUp) {
                lerpPercent += moveAmount;
                if (lerpPercent >= 1) {
                    lerpPercent = 1;
                    isMovingUp = false;
                }
            }
            else if (!isMovingUp) {
                lerpPercent -= moveAmount;
                if (lerpPercent <= 0) {
                    lerpPercent = 0;
                    isMovingUp = true;
                }
            }

            float pipLocation = Mathf.SmoothStep(floor, ceiling, lerpPercent);
            SetRectHeightToPipLevel(pipBaseRect, pipLocation);

            // then store current power pointed to by pip
            float adjustedPip = pipLocation - floor;
            float adjustedCeiling = HardCodedPips[10] - floor;
            gaugePipPower = adjustedPip / adjustedCeiling;

            yield return null;
        }
    }

    void StopPipMovement() {
        StopCoroutine(nameof(PipMovement));
    }
}