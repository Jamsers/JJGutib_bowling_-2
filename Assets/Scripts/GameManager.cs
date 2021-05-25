using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    GameState gameState = GameState.StartMenu;

    [SerializeField] Level[] levels;

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject hudMenu;
    [SerializeField] GameObject pinsToppledMenu;
    [SerializeField] GameObject finishScreen;

    public GameState state
    {
        get { return gameState; }
        set { SetGameState(value); }
    }

    int level = 2;

    bool isFirstInit = true;

    int playerPower = powerGrantedByNonColorMatch;
    public const int maxPlayerPower = 10;
    int powerGrantedByColorMatch;
    const int powerGrantedByNonColorMatch = 1;

    [System.Serializable]
    public enum Color {
        Green,
        Yellow,
        Red
    }

    public enum GameState {
        StartMenu,
        Running,
        KickingBall,
        BallKicked,
        FirstPinMoved,
        FinishScreen
    }

    [System.Serializable]
    struct Level {
        public GameObject prefab;
        [HideInInspector] public GameObject reference;
    }

    void Awake() {
        if (Instance != null)
            Debug.LogError("Error! There is more than one GameManager in the scene!");
        else
            Instance = this;

        for (int i = 0; i < levels.Length; i++) {
            levels[i].reference = Instantiate(levels[i].prefab, null);
            levels[i].reference.SetActive(false);
        }
    }

    public void PickUpBall(Color ballColor) {
        if (ballColor == PlayerController.Instance.color) {
            AddPlayerPower(powerGrantedByColorMatch);
        }
        else {
            AddPlayerPower(powerGrantedByNonColorMatch);
        }
    }

    void AddPlayerPower(int power) {
        int addedPower = playerPower + power;
        if (addedPower > maxPlayerPower) {
            addedPower = maxPlayerPower;
        }
        playerPower = addedPower;
    }

    public void SetGameState(int state) {
        SetGameState((GameState) state);
    }
    public void SetGameState(GameState state) {
        if (state == GameState.StartMenu) {
            for (int i = 0; i < levels.Length; i++) {
                levels[i].reference.SetActive(false);
            }

            levels[level - 1].reference.SetActive(true);

            for (int i = 0; i < levels[level-1].reference.transform.childCount; i++) {
                levels[level - 1].reference.transform.GetChild(i).gameObject.SetActive(true);
            }

            PlayerController.Instance.SetPlayerColor(levels[level - 1].reference.GetComponent<LevelData>().startingColor);

            powerGrantedByColorMatch = Mathf.CeilToInt((float)(maxPlayerPower-powerGrantedByNonColorMatch) / levels[level - 1].reference.GetComponent<LevelData>().ballRows);

            if (isFirstInit)
                isFirstInit = false;
            else
                PlayerController.Instance.playerAnimator.SetTrigger("switchToIdle");
            
            startMenu.SetActive(true);
            finishScreen.SetActive(false);
            pinsToppledMenu.SetActive(false);
            PlayerController.Instance.ResetPlayer();
            PinsManager.Instance.ResetPins();
        }
        if (state == GameState.Running) {
            startMenu.SetActive(false);
            hudMenu.SetActive(true);
            PlayerController.Instance.playerAnimator.SetTrigger("switchToRun");
        }
        if (state == GameState.KickingBall) {
            PlayerController.Instance.playerAnimator.SetTrigger("switchToKick");
        }
        if (state == GameState.BallKicked) {
            PlayerController.Instance.KickBall();
        }
        if (state == GameState.FirstPinMoved) {
            pinsToppledMenu.SetActive(true);
            PinsManager.Instance.ContinouslyCheckIfPinsHaveStoppedMoving();
        }
        if (state == GameState.FinishScreen) {
            PlayerController.Instance.playerAnimator.SetTrigger("switchToJump");
            hudMenu.SetActive(false);
            finishScreen.SetActive(true);
        }

        gameState = state;
        Debug.Log(state.ToString());
    }


    // Start is called before the first frame update
    void Start()
    {
        SetGameState(GameState.StartMenu);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
