#if STATE_BEHAVIOR_ENABLED
using Marmary.StateBehavior.Core;
using UnityEngine;

namespace Marmary.StateBehavior.SwitchState
{
    /// <summary>
    ///     Base component that orchestrates switch animations for show and hide transitions.
    /// </summary>
    public abstract class SwitchElement : Element<SwitchState>
    {


        #region Fields

        private SwitchStateMachine _stateMachine;
        
        
        #endregion

        #region Unity Event Functions

        /// <summary>
        ///     Builds the switch state machine.
        /// </summary>
        protected virtual void Awake()
        {
            _stateMachine = new SwitchStateMachine(SwitchState.Show, gameObject, actions, this);
        }



        #endregion

        #region Methods

        /// <summary>
        ///     Fires the show trigger.
        /// </summary>
        public void OnShow()
        {
            _stateMachine?.FireTrigger(SwitchTrigger.OnShow);
        }

        /// <summary>
        ///     Fires the hide trigger.
        /// </summary>
        public void OnHide()
        {
            _stateMachine?.FireTrigger(SwitchTrigger.OnHide);
        }

        /// <summary>
        ///     Alias for <see cref="OnShow" /> for semantic clarity when used externally.
        /// </summary>
        public void Show() => OnShow();

        /// <summary>
        ///     Alias for <see cref="OnHide" /> for semantic clarity when used externally.
        /// </summary>
        public void Hide() => OnHide();

        /// <summary>
        ///     Instantly sets the element to the hide state without animation.
        /// </summary>
        public void InstantHide()
        {
            if (actions == null) return;
            foreach (var action in actions)
                action.InstantSet(SwitchState.Hide);
            
            if (Events.ContainsKey(SwitchState.Hide))
                Events[SwitchState.Hide]?.Invoke();
        }

        /// <summary>
        ///     Instantly sets the element to the show state without animation.
        /// </summary>
        public void InstantShow()
        {
            if (actions == null) return;
            foreach (var action in actions)
                action.InstantSet(SwitchState.Show);
            
            if (Events.ContainsKey(SwitchState.Show))
                Events[SwitchState.Show]?.Invoke();
        }

        #endregion
    }
}
#endif

