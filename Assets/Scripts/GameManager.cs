using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Bowling;

public class GameManager : MonoBehaviour {
    [Header("Game Settings")]
    [SerializeField] int maxPlayerPower;
    [SerializeField] int powerGrantedByNonColorMatch;
    [SerializeField] float scoreMultiplierForPinsScreen;

    [Header("Hud Canvas")]
    [SerializeField] GameObject startCanvas;
    [SerializeField] GameObject hudCanvas;
    [SerializeField] GameObject pinsToppledCanvas;
    [SerializeField] GameObject finishCanvas;

    [Header("Text")]
    [SerializeField] Text levelText;
    [SerializeField] Text pinsToppledtext;
    [SerializeField] Text gameFinishedText;
    [SerializeField] Text gameFinishedButtonText;

    [Header("Level Prefabs")]
    [SerializeField] Level[] levels;

    static float powerPlaySpacePercent;
    
    static GameManager instance;
    static StateChangedEvent stateChangedEvent;
    static PowerChangedEvent powerChangedEvent;
    static ColorChangedEvent colorChangedEvent;
    static ManualKickEvent playerKickedBallEvent;
    static PickedUpBallEvent playerPickedUpBallEvent;

    static State gameState;
    static int playerPower;
    static Bowling.Color playerColor;

    int level;
    bool hasPenalty;

    public static float PowerPercent { get => powerPlaySpacePercent; }

    public static GameManager Instance { get => instance; }
    public static StateChangedEvent StateChanged { get => stateChangedEvent; }
    public static PowerChangedEvent PowerChanged { get => powerChangedEvent; }
    public static ColorChangedEvent ColorChanged { get => colorChangedEvent; }
    public static ManualKickEvent ManualKick { get => playerKickedBallEvent; }
    public static PickedUpBallEvent PickedUpBall { get => playerPickedUpBallEvent; }

    public static State State {
        get { return gameState; }
        set { gameState = value; stateChangedEvent.Invoke(value); }
    }
    public static int Power {
        get { return playerPower; }
        set { playerPower = value; powerChangedEvent.Invoke(value); }
    }
    public static Bowling.Color Color {
        get { return playerColor; }
        set { playerColor = value; colorChangedEvent.Invoke(value); }
    }

    void Awake() {
        instance = this;
        stateChangedEvent = new StateChangedEvent();
        powerChangedEvent = new PowerChangedEvent();
        colorChangedEvent = new ColorChangedEvent();
        playerKickedBallEvent = new ManualKickEvent();
        playerPickedUpBallEvent = new PickedUpBallEvent();

        for (int i = 0; i < levels.Length; i++) {
            levels[i].reference = Instantiate(levels[i].prefab, null);
            levels[i].reference.SetActive(false);
        }

        StateChanged.AddListener(StateSetup);
        PickedUpBall.AddListener(PickUpBall);
        PowerChanged.AddListener(SetPowerPlaySpacePercent);
    }

    void Start() {
        level = 1;
        State = State.StartMenu;
    }

    void StateSetup(State state) {
        switch (state) {
            case State.StartMenu:
                for (int i = 0; i < levels.Length; i++) {
                    levels[i].reference.SetActive(false);
                }
                levels[level - 1].reference.SetActive(true);
                levels[level - 1].reference.GetComponent<LevelData>().ResetLevel();

                Power = powerGrantedByNonColorMatch;
                Color = levels[level - 1].reference.GetComponent<LevelData>().StartingColor;
                hasPenalty = false;

                levelText.text = "Level " + level.ToString();

                startCanvas.SetActive(true);
                finishCanvas.SetActive(false);
                pinsToppledCanvas.SetActive(false);
                hudCanvas.SetActive(false);
                break;
            case State.Running:
                hudCanvas.SetActive(true);
                startCanvas.SetActive(false);
                break;
            case State.FirstPinMoved:
                pinsToppledCanvas.SetActive(true);
                hudCanvas.SetActive(false);
                StartCoroutine(UpdatePinsToppledText());
                break;
            case State.FinishScreen:
                finishCanvas.SetActive(true);

                if (level == levels.Length) {
                    gameFinishedText.text = "Game Finished!";
                    gameFinishedButtonText.text = "Restart";
                }
                else {
                    gameFinishedText.text = "Level Completed!";
                    gameFinishedButtonText.text = "Next";
                }

                level++;
                if (level > levels.Length)
                    level = 1;
                break;
        }
    }

    void PickUpBall(Bowling.Color ballColor) {
        float playSpace = maxPlayerPower - powerGrantedByNonColorMatch;
        float ballRows = levels[level - 1].reference.GetComponent<LevelData>().BallRows;
        int powerGrantedByColorMatch = Mathf.CeilToInt(playSpace / ballRows);

        if (Color == ballColor) {
            if (hasPenalty) {
                hasPenalty = false;
                AddPower(powerGrantedByNonColorMatch);
            }
            else {
                AddPower(powerGrantedByColorMatch);
            }
        }
        else {
            hasPenalty = true;
            if (Power == powerGrantedByNonColorMatch) {
                AddPower(powerGrantedByNonColorMatch);
            }
            else {
                AddPower(-powerGrantedByNonColorMatch);
            }
        }
    }

    void AddPower(int power) {
        int addedPower = Power + power;
        if (addedPower > maxPlayerPower) {
            addedPower = maxPlayerPower;
        }
        else if (addedPower < powerGrantedByNonColorMatch) {
            addedPower = powerGrantedByNonColorMatch;
        }
        Power = addedPower;
    }

    void SetPowerPlaySpacePercent(int power) {
        float adjustedPower = power - powerGrantedByNonColorMatch;
        float adjustedMax = maxPlayerPower - powerGrantedByNonColorMatch;
        powerPlaySpacePercent = adjustedPower / adjustedMax;
    }

    IEnumerator UpdatePinsToppledText() {
        while (State == State.FirstPinMoved) {
            float numberToShow = PinsManager.MovedPins * scoreMultiplierForPinsScreen;
            pinsToppledtext.text = numberToShow.ToString();
            yield return null;
        }
    }
}