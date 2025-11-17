#if STATE_BEHAVIOR_ENABLED
using System;

namespace Marmary.StateBehavior.Core
{
    /// <summary>
    ///     Factory responsible for creating <see cref="ActionDataState{TValue}" /> instances based on behavior types.
    /// </summary>
    public class ActionDataStateFactory
    {
        #region Methods

        /// <summary>
        ///     Creates a state configuration that matches the supplied behavior type.
        /// </summary>
        /// <typeparam name="TValue">Type of the value driven by the action.</typeparam>
        /// <param name="type">Behavior type requested.</param>
        /// <param name="isDefaultState">Indicates whether the resulting configuration is for the default enum state.</param>
        /// <returns>The instantiated configuration or <c>null</c> if no concrete implementation exists.</returns>
        public static ActionDataState<TValue> Create<TValue>(BehaviorActionTypes type, bool isDefaultState)
        {
            switch (type)
            {
                case BehaviorActionTypes.None:
                    break;
                case BehaviorActionTypes.Simple:
                    return new ActionDataSimpleState<TValue>(isDefaultState);
                case BehaviorActionTypes.Looping:
                    break;
                case BehaviorActionTypes.Sequencer:
                    break;
                case BehaviorActionTypes.Instant:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return null;
        }

        #endregion
    }
}
#endif