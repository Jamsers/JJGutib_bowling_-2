using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerGaugeManager : MonoBehaviour
{
    public static PowerGaugeManager Instance;

    [SerializeField] RectTransform powerFullMaskRect;
    [SerializeField] RectTransform powerFullRect;
    [SerializeField] RectTransform pipBaseRect;

    public int testGaugeFill;
    readonly float[] HardCodedPips = { 0.0549f, 0.1172f, 0.1818f, 0.2540f, 0.3404f, 0.4268f, 0.5267f, 0.6233f, 0.7238f, 0.8358f, 0.9644f };

    float gaugeBaseHeight;

    bool isPipMoving = false;
    float pipPower;
    const float pipMoveSpeed = 0.5f;

    void Awake() {
        if (Instance != null)
            Debug.LogError("Error! There is more than one PowerGaugeManager in the scene!");
        else
            Instance = this;

        gaugeBaseHeight = GetComponent<RectTransform>().rect.height;

        ConvertRectToBottomBase(powerFullMaskRect);
        ConvertRectToBottomBase(powerFullRect);
        ConvertRectToBottomBase(pipBaseRect);

        SetRectHeightToPipLevel(powerFullMaskRect, 0);
        SetRectHeightToPipLevel(powerFullRect, 1f);
        SetRectHeightToPipLevel(pipBaseRect, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        //setPowerLevel(9);
        //StartPipMovement(9);
        //Invoke("StopPipAndGetPower", 5);
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

    public void setPowerLevel(int powerLevel) {
        SetRectHeightToPipLevel(powerFullMaskRect, powerLevel);
    }

    public void StartPipMovement(int powerLevel) {
        pipBaseRect.gameObject.SetActive(true);
        StartCoroutine(PipMovement(powerLevel));
    }

    public float StopPipAndGetPower() {
        isPipMoving = false;
        pipBaseRect.gameObject.SetActive(false);
        return pipPower;
    }

    public void ResetPip() {
        SetRectHeightToPipLevel(pipBaseRect, 0);
        pipBaseRect.gameObject.SetActive(false);
    }

    IEnumerator PipMovement(int moveCap) {
        isPipMoving = true;

        bool isMovingUp = true;
        float floor = HardCodedPips[0];
        float currentLocation = floor;
        float ceiling = HardCodedPips[moveCap];

        while (isPipMoving) {
            float moveAmount = pipMoveSpeed * Time.deltaTime;
            if (isMovingUp) {
                currentLocation = currentLocation + moveAmount;
                if (currentLocation >= ceiling) {
                    currentLocation = ceiling;
                    isMovingUp = false;
                }
            }
            else if (!isMovingUp) {
                currentLocation = currentLocation - moveAmount;
                if (currentLocation <= floor) {
                    currentLocation = floor;
                    isMovingUp = true;
                }
             }

            SetRectHeightToPipLevel(pipBaseRect, currentLocation);
            pipPower = (currentLocation-floor) / (HardCodedPips[10]-floor);

            yield return null;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
