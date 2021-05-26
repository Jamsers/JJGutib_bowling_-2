using UnityEngine;

public class ChangeColorWall : MonoBehaviour {
    [SerializeField] Bowling.Color wallColor;

    [SerializeField] Material greenMaterial;
    [SerializeField] Material yellowMaterial;
    [SerializeField] Material redMaterial;

    public Bowling.Color Color { get => wallColor; }

    void Start() {
        Material materialToAssign = null;

        switch (wallColor) {
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

        Transform animationContainer = transform.GetChild(0);

        for (int i = 0; i < animationContainer.childCount; i++) {
            Renderer renderer = animationContainer.GetChild(i).GetComponent<Renderer>();
            Material[] currentMaterials = renderer.materials;
            currentMaterials[1] = materialToAssign;
            renderer.materials = currentMaterials;
        }
    }
}