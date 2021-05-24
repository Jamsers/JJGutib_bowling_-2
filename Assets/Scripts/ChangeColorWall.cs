using UnityEngine;

public class ChangeColorWall : MonoBehaviour {
    [SerializeField] WallColor wallColor;

    [SerializeField] Material greenMaterial;
    [SerializeField] Material yellowMaterial;
    [SerializeField] Material redMaterial;

    [System.Serializable]
    public enum WallColor {
        Green,
        Yellow,
        Red
    }

    public WallColor color {
        get => wallColor;
    }

    void Start() {
        Material materialToAssign = null;

        switch (wallColor) {
            case WallColor.Green:
                materialToAssign = greenMaterial;
                break;
            case WallColor.Yellow:
                materialToAssign = yellowMaterial;
                break;
            case WallColor.Red:
                materialToAssign = redMaterial;
                break;
        }

        for (int i = 0; i < transform.childCount; i++) {
            Renderer renderer = transform.GetChild(i).GetComponent<Renderer>();
            Material[] currentMaterials = renderer.materials;
            currentMaterials[1] = materialToAssign;
            renderer.materials = currentMaterials;
        }
    }
}
