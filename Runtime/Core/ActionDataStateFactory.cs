#if STATE_BEHAVIOR_ENABLED
using System;

namespace Marmary.StateBehavior.Core
{
    public class ActionDataStateFactory
    {
        #region Methods

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
                case BehaviorActionTypes.OneShot:
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