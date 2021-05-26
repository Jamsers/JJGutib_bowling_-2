using UnityEngine;
using UnityEngine.UI;
using Bowling;

public class SetStateOnClick : MonoBehaviour {
    [SerializeField] State stateToSwitchToOnClick;

    void Awake() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick() {
        GameManager.State = stateToSwitchToOnClick;
    }
}