using UnityEngine;

public class PassTriggerEnterUp : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        transform.parent.parent.GetComponent<PlayerController>().OnTriggerEnter(other);
    }
}