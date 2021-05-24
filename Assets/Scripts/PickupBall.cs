using UnityEngine;

public class PickupBall : MonoBehaviour {
    [SerializeField] ChangeColorWall.WallColor ballColor;

    [SerializeField] Material greenMaterial;
    [SerializeField] Material yellowMaterial;
    [SerializeField] Material redMaterial;

    public ChangeColorWall.WallColor color {
        get => ballColor;
    }

    void Start() {
        Material materialToAssign = null;

        switch (ballColor) {
            case ChangeColorWall.WallColor.Green:
                materialToAssign = greenMaterial;
                break;
            case ChangeColorWall.WallColor.Yellow:
                materialToAssign = yellowMaterial;
                break;
            case ChangeColorWall.WallColor.Red:
                materialToAssign = redMaterial;
                break;
        }

        Renderer renderer = transform.GetChild(0).GetComponent<Renderer>();
        Material[] currentMaterials = renderer.materials;
        currentMaterials[0] = materialToAssign;
        renderer.materials = currentMaterials;
    }
}
