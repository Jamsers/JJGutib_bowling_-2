using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerGaugeManager : MonoBehaviour
{
    public int testGaugeFill;
    readonly float[] HardCodedPips = { 0.02291885483f, 0.11720629032f, 0.18182854838f, 0.2540176344f, 0.34041736559f, 0.42681688172f, 0.52679016129f, 0.62333225806f, 0.72386075268f, 0.8358376344f, 0.98822741935f };

    // Start is called before the first frame update
    void Start()
    {
        float height = transform.parent.GetComponent<RectTransform>().rect.height;

        Vector2 currentAnchor;
        Vector2 currentRect;
        Vector2 currentPos;

        currentAnchor = GetComponent<RectTransform>().anchorMax;
        currentAnchor.y = 0;
        GetComponent<RectTransform>().anchorMax = currentAnchor;
        currentRect = GetComponent<RectTransform>().sizeDelta;
        currentRect.y = height;
        GetComponent<RectTransform>().sizeDelta = currentRect;
        currentPos = GetComponent<RectTransform>().anchoredPosition;
        currentPos.y = height/2;
        GetComponent<RectTransform>().anchoredPosition = currentPos;

        currentAnchor = transform.GetChild(0).GetComponent<RectTransform>().anchorMax;
        currentAnchor.y = 0;
        transform.GetChild(0).GetComponent<RectTransform>().anchorMax = currentAnchor;
        currentRect = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        currentRect.y = height;
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = currentRect;
        currentPos = transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition;
        currentPos.y = height / 2;
        transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = currentPos;

        
    }

    void SetGaugeFill(int fillLevel) {
        float height = transform.parent.GetComponent<RectTransform>().rect.height;
        Vector2 currentRect;
        Vector2 currentPos;
        float heightTarget = height * HardCodedPips[fillLevel];

        currentRect = GetComponent<RectTransform>().sizeDelta;
        currentRect.y = heightTarget;
        GetComponent<RectTransform>().sizeDelta = currentRect;
        currentPos = GetComponent<RectTransform>().anchoredPosition;
        currentPos.y = heightTarget / 2;
        GetComponent<RectTransform>().anchoredPosition = currentPos;
    }

    // Update is called once per frame
    void Update()
    {
        SetGaugeFill(testGaugeFill);
    }
}
