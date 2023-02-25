namespace StateMachines
{
    //Defines a StateMachine
    public class StateMachine<T>
    {
        //The state that the machine is currently running in
        public State<T> CurrentState { get; private set; }
        public State<T> PreviousState { get; private set; }
        //The object that owns/runs the state.
        public T Owner;

        //Construct
        public StateMachine(T owner)
        {
            Owner = owner;
            CurrentState = null;
            PreviousState = null;
        }

        public void ChangeState(State <T> newState)
        {
            if (newState == CurrentState) return;
            if (CurrentState != null)
            {
                //Run the exit code for the current state
                CurrentState.ExitState();
            }
            //Changes the current state to the new one and runs the enter state code.
            PreviousState = CurrentState;
            CurrentState = newState;
            CurrentState.EnterState();
        }

        public void Update()
        {
            //Run the update state of the current state, so long as current state exists.
            if (CurrentState != null)
            {
                CurrentState.UpdateState();
            }
        }

        public void FixedUpdate()
        {
            //Run the update state of the current state, so long as current state exists.
            if (CurrentState != null)
            {
                CurrentState.FixedUpdateState();
            }
        }
    }

    //Generic State class.
    public abstract class State<T>
    {
        protected T owner;
        protected State(T o)
        {
            owner = o;
        }
        public abstract void EnterState();
        //Triggers right before leaving a state, used to destroy instance of a state.
        public abstract void ExitState();
        //Occurs every frame, runs the code of the state.
        public abstract void UpdateState();
        //Occurs during fixedUpdate
        public abstract void FixedUpdateState();
    }
}