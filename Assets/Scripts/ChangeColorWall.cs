using UnityEngine;

public class ChangeColorWall : MonoBehaviour {
    [SerializeField] GameManager.Color wallColor;

    [SerializeField] Material greenMaterial;
    [SerializeField] Material yellowMaterial;
    [SerializeField] Material redMaterial;

    

    public GameManager.Color color {
        get => wallColor;
    }

    void Start() {
        Material materialToAssign = null;

        switch (wallColor) {
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

        for (int i = 0; i < transform.childCount; i++) {
            Renderer renderer = transform.GetChild(i).GetComponent<Renderer>();
            Material[] currentMaterials = renderer.materials;
            currentMaterials[1] = materialToAssign;
            renderer.materials = currentMaterials;
        }
    }
}
