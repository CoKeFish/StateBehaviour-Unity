using System;
using Stateless;
using Stateless.Graph;

namespace Scrips.StateBehavior
{
    /// <summary>
    ///     Base class for state machines
    /// </summary>
    /// <typeparam name="TState"> State enum </typeparam>
    /// <typeparam name="TTrigger"> Trigger enum </typeparam>
    public abstract class StateMachineBase<TState, TTrigger> where TState : Enum where TTrigger : Enum
    {
        #region Fields

        /// <summary>
        ///     State machine
        /// </summary>
        protected readonly StateMachine<TState, TTrigger> StateMachine;

        #endregion


        /// <summary>
        ///     State machine basic configuration
        /// </summary>
        /// <param name="initialState"></param>
        protected StateMachineBase(TState initialState)
        {
            StateMachine = new StateMachine<TState, TTrigger>(initialState);
        }

        #region Methods

        /// <summary>
        ///     Generates a DOT graph representation of the state machine.
        ///     Useful for visualizing the state machine's structure.
        /// </summary>
        /// <returns>A string containing the DOT graph representation of the state machine.</returns>
        public string ToDotGraph()
        {
            return UmlDotGraph.Format(StateMachine.GetInfo());
        }


        /// <summary>
        ///     Configures the state machine
        /// </summary>
        protected abstract void ConfigureStateMachine();

        /// <summary>
        ///     Fires a trigger to transition the state machine to the next state.
        /// </summary>
        /// <param name="trigger">The trigger to fire.</param>
        public virtual void FireTrigger(TTrigger trigger)
        {
            StateMachine.Fire(trigger);
        }

        #endregion
    }
}