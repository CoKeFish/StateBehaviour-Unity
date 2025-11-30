#if STATE_BEHAVIOR_ENABLED
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    ///     Defines the lifecycle for actions that respond to state transitions.
    /// </summary>
    public interface IStateContract<in TState>
    {
        /// <summary>
        ///     Performs any setup required before the action can respond to state changes.
        /// </summary>
        /// <param name="value">Game object hosting the action.</param>
        void Setup(GameObject value);

        #region Methods

        /// <summary>
        ///     Assigns a value or performs an operation to modify the state or behavior of an object or property asynchronously.
        /// </summary>
        /// <param name="state">The new state to be set or applied to the object or property.</param>
        /// <param name="timeWrapper">An instance containing timing information for the operation.</param>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        public UniTask Set(TState state, TimeWrapper timeWrapper);

        /// <summary>
        ///     Applies the specified state immediately, bypassing any animations or transitions.
        /// </summary>
        /// <param name="state">The state to be applied.</param>
        public void InstantSet(TState state);

        #endregion
    }
}
#endif