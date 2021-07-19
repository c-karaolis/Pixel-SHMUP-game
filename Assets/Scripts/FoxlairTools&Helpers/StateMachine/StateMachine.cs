using UnityEngine;

namespace Foxlair.Tools.StateMachine
{
    /// <summary>
    /// Responsible for controlling the flow of the states in child game objects.
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private bool ShowDebugCurrentState = false;
#endif
        [SerializeField]
        private State InitialState = null;
        public State[] AvailableStates;

        public GameObject CharacterStatesGameObject;
        /// <summary>
        /// Current state of this state machine
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// Change this state machine's current state. Pass in 'null' state to stop state machine.
        /// </summary>
        /// <param name="newState">New state to change to. 'null' to stop state machine. State must be a child to this state machine.</param>
        public void ChangeState(State newState)
        {
            if (newState.StateMachine != this)
            {
#if UNITY_EDITOR
                Debug.LogError($"Error occurred at <b>{gameObject.name}</b>: Not allowed to change state that doesn't belong to this state machine", this);
#endif
                return;
            }

            if (CurrentState != null)
            {
                CurrentState.OnStateExit();
            }

            CurrentState = newState;

            if (CurrentState == null) return;

#if UNITY_EDITOR
            if (ShowDebugCurrentState)
            {
                Debug.Log($"<b><color=#e74c3c>{gameObject.name}'s current state is </color><color=#f1c40f>{CurrentState.name}</color></b>", this);
            }
#endif

            CurrentState.OnStateEnter();
        }


        /// <summary>
        /// Change this state machine's current state. Pass in 'null' state to stop state machine.
        /// If condition is not met change to fallbackState instead
        /// </summary>
        /// <param name="newState">New state to change to. 'null' to stop state machine. State must be a child to this state machine.</param>
        public void ChangeState(State newState, State fallbackState, bool condition)
        {
            if (newState.StateMachine != this)
            {
#if UNITY_EDITOR
                Debug.LogError($"Error occurred at <b>{gameObject.name}</b>: Not allowed to change state that doesn't belong to this state machine", this);
#endif
                return;
            }

            if (CurrentState != null)
            {
                CurrentState.OnStateExit();
            }

            if (condition)
            {
                CurrentState = newState;
            }
            else
            {
                CurrentState = fallbackState;
            }

            if (CurrentState == null) return;

#if UNITY_EDITOR
            if (ShowDebugCurrentState)
            {
                Debug.Log($"<b><color=#e74c3c>{gameObject.name}'s current state is </color><color=#f1c40f>{CurrentState.name}</color></b>", this);
            }
#endif

            CurrentState.OnStateEnter();
        }





        public virtual void Start()
        {
            if (CharacterStatesGameObject == null)
            {
                AvailableStates = GetComponentsInChildren<State>();
            }
            else
            {
                AvailableStates = CharacterStatesGameObject.GetComponentsInChildren<State>();
            }
            for (int i = 0; i < AvailableStates.Length; i++)
            {
                AvailableStates[i].StateMachine = this;
            }

            ChangeState(InitialState);
        }

        public virtual void Update()
        {
            if (!CurrentState) return;
            CurrentState.OnStateExecute();
        }

        private void FixedUpdate()
        {
            if (!CurrentState) return;
            CurrentState.OnStatePhysicsExecute();
        }

        private void LateUpdate()
        {
            if (!CurrentState) return;
            CurrentState.OnStatePostExecute();
        }
    }

}