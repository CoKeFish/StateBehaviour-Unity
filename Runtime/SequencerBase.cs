using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

#if STATE_BEHAVIOR_ENABLED
namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    /// Base class for sequencers that determine the order of elements based on a specified sequencing criterion.
    /// </summary>
    /// <typeparam name="TState">The type representing the state enumeration.</typeparam>
    /// <typeparam name="TTrigger">The type representing the trigger enumeration.</typeparam>
    /// <typeparam name="TValue">The type of elements being sequenced.</typeparam>
    [Serializable]
    public abstract class SequencerBase<TState, TTrigger, TValue> where TState : Enum
        where TTrigger : Enum
        where TValue : Element<TState, TTrigger>
    {
        #region Fields

        /// <summary>
        /// Represents the sequencing criterion used by the sequencer to determine the order of elements.
        /// This criterion defines how the elements are sorted and can be customized to implement various ordering strategies.
        /// </summary>
        [FormerlySerializedAs("_criterion")] [SerializeField] [OdinSerialize] [SerializeReference]
        public ISequencingCriterion<TValue> criterion;

        #endregion


        #region Methods

        /// <summary>
        ///     Sorts the elements according to the sequencing criterion.
        /// </summary>
        /// <param name="elements">The elements to sort.</param>
        /// <returns>Sorted, read-only list of elements.</returns>
        protected List<TValue> GetSortedElements(List<TValue> elements)
        {
            if (elements == null)
                return new List<TValue>();


            if (criterion == null || elements.Count == 0)
                return elements;

            return criterion.GetSortValues(elements);
        }


        /// <summary>
        /// Sets up the sequencer by initializing necessary components and configuring elements based on the specific implementation.
        /// </summary>
        public abstract void Setup();

        #endregion
    }
}
#endif