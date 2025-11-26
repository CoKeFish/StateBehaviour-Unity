#if STATE_BEHAVIOR_ENABLED
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Marmary.StateBehavior.SwitchState;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Marmary.StateBehavior.Menu
{
    /// <summary>
    ///     Controls the order and timing of menu element animations using a sequencing criterion.
    ///     The sequencer determines the order in which elements should be shown or hidden based on a criterion.
    /// </summary>
    [Serializable]
    public class MenuSequencer : SequencerBase<SwitchState.SwitchState, SwitchTrigger, SwitchElement>
    {
        /// <summary>
        /// Specifies the operation of hiding elements in a sequence within the MenuSequencer.
        /// Used to make menu elements invisible or removed from view during sequencing.
        /// </summary>
        public enum SequencerOperation
        {
            /// <summary>
            /// Represents the operation to display menu elements in a sequence.
            /// When applied, the sequencer transitions the elements to a visible state.
            /// </summary>
            Show,


            /// <summary>
            /// Represents the operation of hiding elements in a sequence within the MenuSequencer.
            /// This operation is used to make menu elements invisible or removed from view during the sequencing process.
            /// </summary>
            Hide
        }

        #region Fields

        /// <summary>
        ///     The menu elements represent the elements of the menu that will be animated
        ///     It not necessarily has to be the children of the menu, it can be any game object
        /// </summary>
        [SerializeField] [BoxGroup("Menu")] private SwitchElement[] menuElements;

        /// <summary>
        /// Represents a list of asynchronous tasks that are executed as part of the menu sequencing operations,
        /// such as showing or hiding menu elements.
        /// This collection is used to aggregate tasks and ensure their completion before progressing further.
        /// </summary>
        private List<UniTask> _tasks = new();


        /// <summary>
        /// The time delay, in seconds, between the sequential animation of menu elements.
        /// This value determines the interval at which each menu element is shown or hidden,
        /// ensuring a smooth and timed transition sequence.
        /// </summary>
        [FormerlySerializedAs("_separation")] public float separation;

        /// <summary>
        /// Stores a reference to a Unity `Component` that is utilized by the menu sequencer for performing various operations
        /// such as retrieving child objects or manipulating game objects related to menu functionality.
        /// </summary>
        private Component _component;

        #endregion

        public MenuSequencer(Component component)
        {
            this._component = component;
        }


        /// <summary>
        /// Initiates the hiding process for all menu elements and waits for their animations or tasks to complete.
        /// </summary>
        /// <returns>
        /// A UniTask that completes when all menu element tasks are finished.
        /// </returns>
        public UniTask Hide()
        {
            foreach (var menuElement in menuElements)
            {
                menuElement.OnHide();
                _tasks.Add(menuElement.WhenTaskCompleted());
            }

            return UniTask.WhenAll(_tasks);
        }

        /// <summary>
        /// Triggers the "show" behavior for all menu elements asynchronously and waits for all animations to complete.
        /// </summary>
        /// <return>
        /// A UniTask that completes when all menu element tasks have finished executing.
        /// </return>
        public UniTask Show()
        {
            foreach (var menuElement in menuElements)
            {
                menuElement.OnShow();
                _tasks.Add(menuElement.WhenTaskCompleted());
            }

            return UniTask.WhenAll(_tasks);
        }


        /// <summary>
        ///     Set the initial state of the menu elements (animations)
        /// </summary>
        public void InstantShow()
        {
            foreach (var menuElement in menuElements)
            {
                menuElement.OnShowInstant();
            }
        }

        /// <summary>
        ///     Set the final state of the menu elements for all animations/actions
        /// </summary>
        public void InstantHide()
        {
            foreach (var menuElement in menuElements)
            {
                menuElement.OnHideInstant();
            }
        }

        /// <summary>
        /// Sets up the menu sequencer by initializing all menu elements and calculating their animation times.
        /// </summary>
        public override void Setup()
        {
            GetAllMenuElements();
            CalculateTimes();
        }


        /// <summary>
        ///     Get all menu elements
        /// </summary>
        [Button(ButtonSizes.Large)]
        [BoxGroup("Menu")]
        public void GetAllMenuElements()
        {
            menuElements = _component.GetComponentsInChildren<SwitchElement>();
#if UNITY_EDITOR
            EditorUtility.SetDirty(_component);
#endif
        }

        /// <summary>
        ///     Reset the times of the menu elements and calculate them again.
        ///     Note: This method is kept for compatibility but may need to be updated
        ///     based on how durations are stored in SwitchElement actions.
        /// </summary>
        [Button(ButtonSizes.Large)]
        [BoxGroup("Options")]
        [PropertySpace(SpaceAfter = 0, SpaceBefore = 20)]
        public void CalculateTimes()
        {
            var delayBeforeDeactivating = 0f;
            var delayBeforeActivating = 0f;

            if (menuElements == null) return;

            foreach (var menuElement in menuElements)
            {
                menuElement.defaultHideAfter = delayBeforeDeactivating;
                menuElement.defaultShowAfter = delayBeforeActivating;
                delayBeforeDeactivating += separation;
                delayBeforeActivating += separation;
#if UNITY_EDITOR
                EditorUtility.SetDirty(_component);
#endif
            }
        }
    }
}
#endif