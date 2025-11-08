#if STATE_BEHAVIOR_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace Marmary.StateBehavior.Core
{
    [Serializable]
    public class ActionData<TState, TValue> : ActionDataBase
        where TState : Enum
    {
        #region Serialized Fields

        [DictionaryDrawerSettings(KeyLabel = "State", ValueLabel = "Data",
            DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        public Dictionary<TState, ActionDataStateWrapper<TValue>> StateActionDataContainers = new();

        #endregion

        #region Unity Event Functions

        private void OnEnable() => EnsureStateActionDataContainers();

        private void OnValidate() => EnsureStateActionDataContainers();

        private void EnsureStateActionDataContainers()
        {
            var states = Enum.GetValues(typeof(TState)).Cast<TState>().ToArray();

            foreach (var state in states)
            {
                if (Convert.ToInt32(state) == 0)
                {
                    if (!StateActionDataContainers.ContainsKey(state))
                        StateActionDataContainers[state] = new ActionDataStateWrapper<TValue>(true);
                    continue;
                }

                if (!StateActionDataContainers.ContainsKey(state))
                    StateActionDataContainers[state] = new ActionDataStateWrapper<TValue>();
            }

            var keysToRemove = StateActionDataContainers.Keys
                .Where(k => !states.Contains(k))
                .ToList();

            foreach (var key in keysToRemove)
                StateActionDataContainers.Remove(key);
        }

        #endregion
    }
}
#endif