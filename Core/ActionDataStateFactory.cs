using System;

namespace Scrips.StateBehavior
{
    public class ActionDataStateFactory
    {
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

    }
}