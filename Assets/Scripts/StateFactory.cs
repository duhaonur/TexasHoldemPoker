// StateFactory class responsible for creating states
public class StateFactory
{
    // Reference to the state machine
    protected readonly IControlStateMachine _stateMachine;

    // Constructor to initialize the state machine
    public StateFactory(IControlStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
}
