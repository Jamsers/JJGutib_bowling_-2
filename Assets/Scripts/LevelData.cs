using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] GameManager.Color playerStartingColor;
    [SerializeField] int amountOfBallRows;

    public GameManager.Color startingColor {
        get => playerStartingColor;
    }

    public int ballRows {
        get => amountOfBallRows;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
