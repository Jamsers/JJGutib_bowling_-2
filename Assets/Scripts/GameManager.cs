using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public static GameState state = GameState.StartMenu;

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject hudMenu;
    [SerializeField] GameObject pinsToppledMenu;
    [SerializeField] GameObject finishScreen;

    public enum GameState {
        StartMenu,
        Running,
        KickingBall,
        BallKicked,
        FirstPinMoved,
        FinishScreen
    }

    void Awake() {
        if (Instance != null)
            Debug.LogError("Error! There is more than one GameManager in the scene!");
        else
            Instance = this;
    }

    public void SetGameState(int state) {
        SetGameState((GameState) state);
    }
    public void SetGameState(GameState state) {
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
            hudMenu.SetActive(false);
            finishScreen.SetActive(true);
        }

        GameManager.state = state;
        Debug.Log(state.ToString());
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
