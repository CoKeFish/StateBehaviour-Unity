#if STATE_BEHAVIOR_ENABLED
namespace Marmary.StateBehavior.Runtime.SwitchState
{
    /// <summary>
    ///     Base component that orchestrates switch animations for show and hide transitions.
    /// </summary>
    public abstract class SwitchElement : Element<SwitchState, SwitchTrigger>
    {
        #region Methods

        /// <summary>
        ///     Builds the switch state machine.
        /// </summary>
        public override void Initialize()
        {
            stateMachine = new SwitchStateMachine(SwitchState.Show, actions, this, time);
            base.Initialize();
        }

        #endregion

        #region Event Functions

        /// <summary>
        ///     Fires the show trigger.
        /// </summary>
        public void OnShow()
        {
            TriggerState(SwitchTrigger.OnShow);
        }

        /// <summary>
        ///     Fires the hide trigger.
        /// </summary>
        public void OnHide()
        {
            TriggerState(SwitchTrigger.OnHide);
        }

        /// <summary>
        ///     Fires the show trigger forcing instant execution.
        /// </summary>
        public void OnShowInstant()
        {
            TriggerState(SwitchTrigger.OnShow, true);
        }

        /// <summary>
        ///     Fires the hide trigger forcing instant execution.
        /// </summary>
        public void OnHideInstant()
        {
            TriggerState(SwitchTrigger.OnHide, true);
        }

        #endregion
    }
}
#endif