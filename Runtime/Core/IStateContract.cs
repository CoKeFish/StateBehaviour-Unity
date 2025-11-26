#if STATE_BEHAVIOR_ENABLED
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Marmary.StateBehavior.Core
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
        ///     Applies the behaviour for the supplied state.
        /// </summary>
        /// <param name="state">State to activate.</param>
        public UniTask Set(TState state);

        /// <summary>
        ///     Applies the behaviour for the supplied state instantly without animation.
        /// </summary>
        /// <param name="state">State to activate.</param>
        public void InstantSet(TState state);

        #endregion
    }
}
#endif