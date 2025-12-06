using System;
using System.Collections.Generic;
using DTT.ExtendedDebugLogs;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if STATE_BEHAVIOR_ENABLED
namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    ///     Base class for sequencers that determine the order of elements based on a specified sequencing criterion.
    /// </summary>
    /// <typeparam name="TState">The type representing the state enumeration.</typeparam>
    /// <typeparam name="TTrigger">The type representing the trigger enumeration.</typeparam>
    /// <typeparam name="TValue">The type of elements being sequenced.</typeparam>
    public static class ElementSequencer<TState, TTrigger, TValue> where TState : Enum
        where TTrigger : Enum
        where TValue : Element<TState, TTrigger>
    {
        #region Methods

        /// <summary>
        ///     Sorts the elements according to the specified sequencing criterion and applies a separation factor.
        /// </summary>
        /// <param name="elements">The list of elements to be sorted.</param>
        /// <param name="criterion">The sequencing criterion used to determine the order of elements.</param>
        /// <param name="separation">The separation factor applied during sequencing.</param>
        public static void SetSortedElements(List<TValue> elements,
            ISequencingCriterion<TValue> criterion,
            float separation)
        {
            if (elements.IsNullOrEmpty())
            {
                DebugEx.LogWarning("Cannot sort elements: the elements list is null or empty.");
                return;
            }

            if (criterion == null)
            {
                DebugEx.LogWarning("Cannot sort elements without a criterion.");
                return;
            }

            var sortElements = criterion.GetSortValues(elements);
            CalculateTimes(sortElements, separation);
        }


        /// <summary>
        ///     Calculates the delay times for a list of elements and updates their respective properties using a separation
        ///     interval.
        /// </summary>
        /// <param name="menuElements">The list of elements for which delay times are calculated and updated.</param>
        /// <param name="separation">The interval used to determine the delay progression between consecutive elements.</param>
        private static void CalculateTimes(List<TValue> menuElements, float separation)
        {
            var delayBeforeDeactivating = 0f;

            if (menuElements == null) return;

            foreach (var menuElement in menuElements)
            {
                menuElement.time.useCustomDelay = true;
                menuElement.time.customDelay = delayBeforeDeactivating;
                delayBeforeDeactivating += separation;
#if UNITY_EDITOR
                EditorUtility.SetDirty(menuElement);
                PrefabUtility.RecordPrefabInstancePropertyModifications(menuElement);

#endif
            }
        }

        #endregion
    }
}
#endif