using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace Scrips.StateBehavior
{
    public abstract class Element<TState> : SerializedMonoBehaviour where TState : Enum
    {
        [SerializeReference] protected List<IStateContract<TState>> actions;

        [SerializeReference] [OdinSerialize] [NonSerialized]
        public Dictionary<TState, UnityEvent> events = new();
        
    }
}