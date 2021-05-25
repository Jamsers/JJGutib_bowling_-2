using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    GameState gameState = GameState.StartMenu;

    [SerializeField] Level[] levels;

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject hudMenu;
    [SerializeField] GameObject pinsToppledMenu;
    [SerializeField] public Text pinsToppledtext;
    [SerializeField] Text levelText;
    [SerializeField] GameObject finishScreen;

    public GameState state
    {
        get { return gameState; }
        set { SetGameState(value); }
    }

    int level = 1;

    bool isFirstInit = true;
    float retrievedPower;

    int playerPower = powerGrantedByNonColorMatch;
    public const int maxPlayerPower = 10;
    int powerGrantedByColorMatch;
    public const int powerGrantedByNonColorMatch = 1;

    bool hasPowerBeenRetrieved = false;

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
        SetPlayerPower(addedPower);
    }

    void SetPlayerPower(int power) {
        PlayerController.Instance.SetBallSize(power);
        PowerGaugeManager.Instance.setPowerLevel(power);
        playerPower = power;
    }

    public void SetGameState(int state) {
        SetGameState((GameState) state);
    }
    public void SetGameState(GameState state) {
        if (state == GameState.StartMenu) {
            if (isFirstInit) {
                isFirstInit = false;
            }
            else {
                PlayerController.Instance.playerAnimator.SetTrigger("switchToIdle");
                level++;
                if (level > levels.Length)
                    level = 1;
            }

            levelText.text = "Level " + level.ToString();

            for (int i = 0; i < levels.Length; i++) {
                levels[i].reference.SetActive(false);
            }

            levels[level - 1].reference.SetActive(true);

            for (int i = 0; i < levels[level-1].reference.transform.childCount; i++) {
                levels[level - 1].reference.transform.GetChild(i).gameObject.SetActive(true);
            }

            hasPowerBeenRetrieved = false;
            SetPlayerPower(powerGrantedByNonColorMatch);

            PlayerController.Instance.SetPlayerColor(levels[level - 1].reference.GetComponent<LevelData>().startingColor);
            PowerGaugeManager.Instance.ResetPip();

            powerGrantedByColorMatch = Mathf.CeilToInt((float)(maxPlayerPower-powerGrantedByNonColorMatch) / levels[level - 1].reference.GetComponent<LevelData>().ballRows);

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
            PlayerController.Instance.playerAnimator.SetFloat("kickSpeed", 0.2f);
            PlayerController.Instance.kickingMoveForwardDivider = 60;
            PowerGaugeManager.Instance.StartPipMovement(playerPower);
        }
        if (state == GameState.BallKicked) {
            if (hasPowerBeenRetrieved == false) {
                RetrievePower();
            }
            PlayerController.Instance.KickBall(retrievedPower);
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

    public void RetrievePower() {
        if (hasPowerBeenRetrieved)
            return;

        hasPowerBeenRetrieved = true;
        retrievedPower = PowerGaugeManager.Instance.StopPipAndGetPower();
        PlayerController.Instance.playerAnimator.SetFloat("kickSpeed", 3f);
        PlayerController.Instance.kickingMoveForwardDivider = 5;
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
