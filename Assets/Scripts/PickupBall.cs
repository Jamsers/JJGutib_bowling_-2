using UnityEngine;

public class PickupBall : MonoBehaviour {
    [SerializeField] Bowling.Color ballColor;

    [SerializeField] Material greenMaterial;
    [SerializeField] Material yellowMaterial;
    [SerializeField] Material redMaterial;

    public Bowling.Color Color { get => ballColor; }

    void Start() {
        Material materialToAssign = null;

        switch (ballColor) {
            case Bowling.Color.Green:
                materialToAssign = greenMaterial;
                break;
            case Bowling.Color.Yellow:
                materialToAssign = yellowMaterial;
                break;
            case Bowling.Color.Red:
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