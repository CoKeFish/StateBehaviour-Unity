#if STATE_BEHAVIOR_ENABLED
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.Criterions
{
    /// <summary>
    ///     Criterion that orders menu elements by the Y position (height) of their RectTransform.
    ///     Elements with higher Y values come first (top to bottom).
    /// </summary>
    public class RectTransformHeightCriterion<TElement> : ISequencingCriterion<TElement> where TElement : Component
    {
        #region ISequencingCriterion<TElement> Members

        /// <summary>
        ///     Sorts a list of menu elements by the Y position (height) of their RectTransform in descending order.
        ///     Elements with higher Y values are ordered first.
        /// </summary>
        /// <param name="elements">List of menu elements to be sorted based on their RectTransform Y position.</param>
        /// <returns>A list of menu elements sorted from top to bottom by their RectTransform Y position.</returns>
        public List<TElement> GetSortValues(List<TElement> elements)
        {
            if (elements == null || elements.Count == 0)
                return new List<TElement>();

            return elements
                .OrderByDescending(e => e.GetComponent<RectTransform>().position.y)
                .ToList();
        }

        #endregion
    }
}
#endif