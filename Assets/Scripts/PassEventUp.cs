using UnityEngine;

public class PassEventUp : MonoBehaviour {
    public void Kicked() {
        transform.parent.GetComponent<PlayerController>().Kicked();
    }
}