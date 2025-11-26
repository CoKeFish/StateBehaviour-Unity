using System;
using System.Collections.Generic;
using System.Linq;
using Marmary.StateBehavior.Core;
using Marmary.StateBehavior.Menu;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

#if STATE_BEHAVIOR_ENABLED
namespace Marmary.StateBehavior
{
    /// <summary>
    ///     Base class for sequencers that control the order of elements using a criterion.
    /// </summary>
    /// <typeparam name="TValue">Type of value used by the criterion.</typeparam>
    /// <typeparam name="TState">Type of state enum.</typeparam>
    [Serializable]
    public class SequencerBase<TValue, TState> where TState : Enum
    {
        #region Fields

        /// <summary>
        /// Represents the sequencing criterion used by the sequencer to determine the order of elements.
        /// This criterion defines how the elements are sorted and can be customized to implement various ordering strategies.
        /// </summary>
        [FormerlySerializedAs("_criterion")] [SerializeField] [OdinSerialize] [SerializeReference]
        public ISequencingCriterion<TValue, TState> criterion;

        #endregion


        #region Methods

        /// <summary>
        ///     Sorts the elements according to the sequencing criterion.
        /// </summary>
        /// <param name="elements">The elements to sort.</param>
        /// <returns>Sorted, read-only list of elements.</returns>
        protected IReadOnlyList<Element<TState>> GetSortedElements(List<Element<TState>> elements)
        {
            if (elements == null)
                return Array.Empty<Element<TState>>();


            if (criterion == null || elements.Count == 0)
                return elements;

            return criterion.GetSortValues(elements);
        }

        #endregion
    }
}
#endif