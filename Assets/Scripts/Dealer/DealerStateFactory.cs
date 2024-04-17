public class DealerStateFactory : StateFactory
{
    public DealerIdleState IdleState { get; private set; }
    public DealerPreFlopState PreFlopState { get; private set; }
    public DealerFlopState FlopState { get; private set; }
    public DealerTurnState TurnState { get; private set; }
    public DealerRiverState RiverState { get; private set; }
    public DealerShowdownState ShowdownState { get; private set; }
    public DealerAIGivePlayerTurnState GivePlayerTurnState { get; private set; }

    public DealerStateFactory(IControlStateMachine stateMachine) : base(stateMachine)
    {
        // Instantiate each state with a reference to the state machine and this factory
        IdleState = new DealerIdleState(_stateMachine as DealerAI, this);
        PreFlopState = new DealerPreFlopState(_stateMachine as DealerAI, this);
        FlopState = new DealerFlopState(_stateMachine as DealerAI, this);
        TurnState = new DealerTurnState(_stateMachine as DealerAI, this);
        RiverState = new DealerRiverState(_stateMachine as DealerAI, this);
        ShowdownState = new DealerShowdownState(_stateMachine as DealerAI, this);
        GivePlayerTurnState = new DealerAIGivePlayerTurnState(_stateMachine as DealerAI, this);
    }
}
