public class PlayerAIStateFactory : StateFactory
{
    // Define properties for each state
    public PlayerAIIdleState IdleState { get; private set; }
    public PlayerAIPreFlopState PreFlopState { get; private set; }
    public PlayerAIFlopState FlopState { get; private set; }
    public PlayerAITurnState TurnState { get; private set; }
    public PlayerAIRiverState RiverState { get; private set; }
    public PlayerAISmallBlindState SmallBlindState { get; private set; }
    public PlayerAIBigBlindState BigBlindState { get; private set; }

    // Constructor
    public PlayerAIStateFactory(IControlStateMachine stateMachine) : base(stateMachine)
    {
        // Initialize each state with the current state machine and this factory
        IdleState = new PlayerAIIdleState(_stateMachine as PlayerAI, this);
        PreFlopState = new PlayerAIPreFlopState(_stateMachine as PlayerAI, this);
        FlopState = new PlayerAIFlopState(_stateMachine as PlayerAI, this);
        TurnState = new PlayerAITurnState(_stateMachine as PlayerAI, this);
        RiverState = new PlayerAIRiverState(_stateMachine as PlayerAI, this);
        SmallBlindState = new PlayerAISmallBlindState(_stateMachine as PlayerAI, this);
        BigBlindState = new PlayerAIBigBlindState(_stateMachine as PlayerAI, this);
    }
}
