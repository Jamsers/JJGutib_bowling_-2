using UnityEngine;

public class PickupBall : MonoBehaviour {
    [SerializeField] GameManager.Color ballColor;

    [SerializeField] Material greenMaterial;
    [SerializeField] Material yellowMaterial;
    [SerializeField] Material redMaterial;

    public GameManager.Color color {
        get => ballColor;
    }

    void Start() {
        Material materialToAssign = null;

        switch (ballColor) {
            case GameManager.Color.Green:
                materialToAssign = greenMaterial;
                break;
            case GameManager.Color.Yellow:
                materialToAssign = yellowMaterial;
                break;
            case GameManager.Color.Red:
                materialToAssign = redMaterial;
                break;
        }

        Renderer renderer = transform.GetChild(0).GetComponent<Renderer>();
        Material[] currentMaterials = renderer.materials;
        currentMaterials[0] = materialToAssign;
        renderer.materials = currentMaterials;
    }

    public void HideBall() {
        gameObject.SetActive(false);
    }
}
