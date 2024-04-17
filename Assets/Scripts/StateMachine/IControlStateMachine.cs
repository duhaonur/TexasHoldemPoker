public interface IControlStateMachine
{
    void SetNewState<T, U>(State<T, U> newState)
        where T : IControlStateMachine
        where U : StateFactory;
}
