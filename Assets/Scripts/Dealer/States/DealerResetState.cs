using UnityEngine;

public class DealerResetState : State<DealerAI, DealerStateFactory>
{
    public DealerResetState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }
    protected override void OnEnter()
    {
    }
    protected override void OnUpdate()
    {
        CheckSwitchState();
    }
    protected override void OnExit()
    {

    }
    protected override void CheckSwitchState()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchState(_stateFactory.PreFlopState);
        }
    }
}
