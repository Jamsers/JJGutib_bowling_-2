using UnityEngine;
using UnityEngine.Events;

namespace Bowling {
    [System.Serializable]
    public enum State {
        StartMenu,
        Running,
        KickingBall,
        BallKicked,
        FirstPinMoved,
        FinishScreen
    }

    [System.Serializable]
    public enum Color {
        Green,
        Yellow,
        Red
    }

    [System.Serializable]
    public struct Level {
        public GameObject prefab;
        [HideInInspector] public GameObject reference;
    }

    public class StateChangedEvent : UnityEvent<State> { }
    public class PowerChangedEvent : UnityEvent<int> { }
    public class ColorChangedEvent : UnityEvent<Color> { }
    public class ManualKickEvent : UnityEvent { }
    public class PickedUpBallEvent : UnityEvent<Color> { }
}