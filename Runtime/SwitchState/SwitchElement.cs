#if STATE_BEHAVIOR_ENABLED
using Marmary.StateBehavior.Core;

namespace Marmary.StateBehavior.SwitchState
{
    /// <summary>
    ///     Base component that orchestrates switch animations for show and hide transitions.
    /// </summary>
    public abstract class SwitchElement : Element<SwitchState, SwitchTrigger>
    {
        #region Unity Event Functions

        /// <summary>
        ///     Builds the switch state machine.
        /// </summary>
        protected override void Awake()
        {
            stateMachine = new SwitchStateMachine(SwitchState.Show, gameObject, actions, this);
            base.Awake();
        }

        #endregion

        #region Methods

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