public abstract class State<T, U>
    where T : IControlStateMachine
    where U : StateFactory
{
    protected T _stateMachine;
    protected U _stateFactory;

    public State(T stateMachineController, U stateFactory)
    {
        _stateMachine = stateMachineController;
        _stateFactory = stateFactory;
    }
    protected abstract void OnEnter();
    protected abstract void OnUpdate();
    protected abstract void OnExit();
    protected abstract void CheckSwitchState();
    public void UpdateState()
    {
        OnUpdate();
    }
    public void EnterState()
    {
        OnEnter();
    }
    protected void SwitchState(State<T, U> newState)
    {
        OnExit();

        _stateMachine.SetNewState(newState);

        newState.OnEnter();


    }
}