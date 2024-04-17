// Abstract class representing a state
public abstract class State<T, U>
    where T : IControlStateMachine
    where U : StateFactory
{
    // Reference to the state machine controller
    protected T _stateMachine;

    // Reference to the state factory
    protected U _stateFactory;

    // Constructor to initialize the state with the state machine and state factory
    public State(T stateMachineController, U stateFactory)
    {
        _stateMachine = stateMachineController;
        _stateFactory = stateFactory;
    }

    // Method called when entering the state
    protected abstract void OnEnter();

    // Method called during each update of the state
    protected abstract void OnUpdate();

    // Method called when exiting the state
    protected abstract void OnExit();

    // Method to check if the state needs to switch to another state
    protected abstract void CheckSwitchState();

    // Method to update the state
    public void UpdateState()
    {
        OnUpdate();
    }

    // Method to enter the state
    public void EnterState()
    {
        OnEnter();
    }

    // Method to switch to a new state
    protected void SwitchState(State<T, U> newState)
    {
        // Exit the current state
        OnExit();

        // Set the new state in the state machine
        _stateMachine.SetNewState(newState);

        // Enter the new state
        newState.OnEnter();
    }
}
