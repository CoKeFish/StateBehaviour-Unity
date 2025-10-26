using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scrips.StateBehavior
{
    [Serializable]
    public class ActionData<TState, TValue> : SerializedScriptableObject
        where TState : Enum
    {
        [DictionaryDrawerSettings(KeyLabel = "State", ValueLabel = "Data",
            DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        public Dictionary<TState, ActionDataStateWrapper<TValue>> StateActionDataContainers = new();

        private void OnValidate()
        {
            var states = Enum.GetValues(typeof(TState)).Cast<TState>().ToArray();

            // 🔹 Agregar solo estados distintos de 0
            foreach (var state in states)
            {
                if (Convert.ToInt32(state) == 0)
                {
                    if (!StateActionDataContainers.ContainsKey(state))
                        StateActionDataContainers[state] = new ActionDataStateWrapper<TValue>(true);
                    continue; // ignorar el "estado por defecto"
                }

                if (!StateActionDataContainers.ContainsKey(state))
                    StateActionDataContainers[state] = new ActionDataStateWrapper<TValue>();
            }

            // 🔹 Eliminar claves que ya no existen o son 0
            var keysToRemove = StateActionDataContainers.Keys
                .Where(k =>  !states.Contains(k))
                .ToList();

            foreach (var key in keysToRemove)
                StateActionDataContainers.Remove(key);
        }
    }
}