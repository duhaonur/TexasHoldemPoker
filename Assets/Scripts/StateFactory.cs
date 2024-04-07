public class StateFactory
{
    protected readonly IControlStateMachine _stateMachine;

    public StateFactory(IControlStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
}
