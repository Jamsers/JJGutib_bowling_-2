using UnityEngine;

public class LevelData : MonoBehaviour {
    [SerializeField] Bowling.Color playerStartingColor;
    [SerializeField] int amountOfBallRows;

    public Bowling.Color StartingColor { get => playerStartingColor; }
    public int BallRows { get => amountOfBallRows; }

    public void ResetLevel() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}