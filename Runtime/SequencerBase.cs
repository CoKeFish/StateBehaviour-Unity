using System;
using System.Collections.Generic;
using Marmary.StateBehavior.Core;
using Marmary.StateBehavior.Menu;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

#if STATE_BEHAVIOR_ENABLED
namespace Marmary.StateBehavior
{
    /// <summary>
    /// Base class for sequencers that determine the order of elements based on a specified sequencing criterion.
    /// </summary>
    /// <typeparam name="TState">The type representing the state enumeration.</typeparam>
    /// <typeparam name="TTrigger">The type representing the trigger enumeration.</typeparam>
    [Serializable]
    public class SequencerBase<TState, TTrigger> where TState : Enum where TTrigger : Enum
    {
        #region Fields

        /// <summary>
        /// Represents the sequencing criterion used by the sequencer to determine the order of elements.
        /// This criterion defines how the elements are sorted and can be customized to implement various ordering strategies.
        /// </summary>
        [FormerlySerializedAs("_criterion")] [SerializeField] [OdinSerialize] [SerializeReference]
        public ISequencingCriterion<TState, TTrigger> criterion;

        #endregion


        #region Methods

        /// <summary>
        ///     Sorts the elements according to the sequencing criterion.
        /// </summary>
        /// <param name="elements">The elements to sort.</param>
        /// <returns>Sorted, read-only list of elements.</returns>
        protected IReadOnlyList<Element<TState, TTrigger>> GetSortedElements(List<Element<TState, TTrigger>> elements)
        {
            if (elements == null)
                return Array.Empty<Element<TState, TTrigger>>();


            if (criterion == null || elements.Count == 0)
                return elements;

            return criterion.GetSortValues(elements);
        }

        #endregion
    }
}
#endif