using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marmary.StateBehavior.Runtime.Menu
{
    /// <inheritdoc cref="UnityEngine.MonoBehaviour" />
    /// <summary>
    ///     Menu is the class that represents the menu,
    ///     it contains the menu elements and the logic of showing and hiding the menu
    /// </summary>
    [DisallowMultipleComponent]
    public class Menu : MonoBehaviour
    {
        #region Serialized Fields

        #region Selectable Fields

        /// <summary>
        ///     First selectable of the menu when it is activated
        /// </summary>
        [SerializeField]
        [RequiredIn(PrefabKind.InstanceInScene)]
        [ChildGameObjectsOnly]
        [BoxGroup("Options")]
        [PropertySpace(SpaceAfter = 10)]
        public Selectable firstSelected;

        /// <summary>
        ///     Default down button of the menu when the event system is null and down button is pressed
        /// </summary>
        [SerializeField] [ChildGameObjectsOnly] [BoxGroup("Options")]
        public Selectable defaultDownButton;

        /// <summary>
        ///     Default up button of the menu when the event system is null and up button is pressed
        /// </summary>
        [SerializeField] [ChildGameObjectsOnly] [BoxGroup("Options")]
        public Selectable defaultUpButton;

        /// <summary>
        ///     Default left button of the menu when the event system is null and left button is pressed
        /// </summary>
        [SerializeField] [ChildGameObjectsOnly] [BoxGroup("Options")]
        public Selectable defaultLeftButton;

        /// <summary>
        ///     Default right button of the menu when the event system is null and right button is pressed
        /// </summary>
        [SerializeField] [ChildGameObjectsOnly] [BoxGroup("Options")] [PropertySpace(SpaceAfter = 10, SpaceBefore = 0)]
        public Selectable defaultRightButton;

        #endregion

        #region Menu Events

        /// <summary>
        ///     Event called when the menu is shown
        /// </summary>
        [SerializeField] [FoldoutGroup("Events", false, 1)]
        public UnityEvent onShow;

        /// <summary>
        ///     Event called when the menu is hidden
        /// </summary>
        [SerializeField] [FoldoutGroup("Events")]
        public UnityEvent onHide;

        /// <summary>
        /// Event that is triggered when the hiding animation or process of the menu has completed.
        /// </summary>
        [SerializeField] [FoldoutGroup("Events")]
        public UnityEvent onHideComplete;

        /// <summary>
        /// Event triggered upon the completion of the menu's show animation or process.
        /// </summary>
        [SerializeField] [FoldoutGroup("Events")]
        public UnityEvent onShowComplete;

        #endregion

        #region Options

        /// <summary>
        ///     Indicates if the menu has a deactivation delay
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useHideAnimation")]
        public bool useHideAnimation;

        /// <summary>
        ///     Time to wait before deactivating the menu
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useHideAnimation")] [ReadOnly]
        private float delayBeforeDeactivating = 0.5f;

        /// <summary>
        ///     Indicates if the menu has an extra deactivation delay
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useExtraDelayBeforeDeactivating")]
        private bool useExtraDelayBeforeDeactivating;

        /// <summary>
        ///     Extra time to wait before deactivating the menu
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useExtraDelayBeforeDeactivating")]
        private float extraDelayBeforeDeactivating;

        /// <summary>
        ///     Indicates if the menu has an activation delay
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useShowAnimation")]
        public bool useShowAnimation = true;

        /// <summary>
        ///     Time to wait before activating the menu
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useShowAnimation")] [ReadOnly]
        private float delayBeforeActivating;

        /// <summary>
        ///     Indicates if the menu has an activation delay
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useExtraActivationDelay")]
        private bool useExtraActivationDelay;

        /// <summary>
        ///     Time to wait before activating the menu
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useExtraActivationDelay")]
        private float extraDelayBeforeActivating = 0.5f;

        /// <summary>
        ///     Indicates if the menu has an activation delay before selecting the first selectable
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useSelectionTime")]
        private bool useSelectionTime;

        /// <summary>
        ///     Time to wait before selecting the first selectable
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useSelectionTime")]
        private float timeToSelect;

        /// <summary>
        ///     Indicates the separation between elements
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [TitleGroup("Options/Utilities")]
        private float separation;

        #endregion


        /// <summary>
        ///     The sequencer that controls the order of menu elements.
        /// </summary>
        [OdinSerialize] [NonSerialized] public MenuSequencer Sequencer;

        #endregion

        [Button]
        public void Hide()
        {
            InactivateMenu().Forget();
        }

        #region Methods

        /// <summary>
        ///     Activate the menu
        ///     1- Activate the menu
        ///     2- Show the menu elements (animations)
        ///     3- Make all selectables in the menu interactable
        ///     4- Select the first selectable if there is one and event system is null
        /// </summary>
        internal async UniTask ActivateMenu()
        {
            gameObject.SetActive(true);
            onShow.Invoke();
            //Show the menu elements (animations) and wait for them to finish for selecting the first selectable
            if (useExtraActivationDelay)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(extraDelayBeforeActivating));
                await Sequencer.Show();
            }
            else
            {
                if (useShowAnimation)
                    await Sequencer.Show();
                else
                    InstantShow();
            }

            await SelectFirstDelay();

            ActiveInteractables();
            onShowComplete.Invoke();
        }

        /// <summary>
        ///     Inactivate the menu
        ///     1- Make all selectables in the menu uninteractable
        ///     2- Hide the menu elements (animations)
        ///     3- Deactivate the menu after the animations
        /// </summary>
        internal async UniTask InactivateMenu()
        {
            DisabledInteractables();
            onHide.Invoke();

            if (useHideAnimation)
            {
                if (useExtraDelayBeforeDeactivating)
                    await UniTask.Delay(TimeSpan.FromSeconds(extraDelayBeforeDeactivating));
                await Sequencer.Hide();


                //Deactivate the menu after the animations
                gameObject.SetActive(false);
            }
            else
            {
                InstantHide();
            }

            onHideComplete.Invoke();
        }

        /// <summary>
        ///     Make all selectables in the menu interactable
        /// </summary>
        internal void ActiveInteractables()
        {
            //Get all selectables in the menu
            var selectables = GetComponentsInChildren<Selectable>();
            //Interactable = true
            foreach (var selectable in selectables) selectable.interactable = true;
        }


        /// <summary>
        ///     Make all selectables in the menu uninteractable
        /// </summary>
        internal void DisabledInteractables()
        {
            //Get all seleccatables in the menu and make them uninteractable
            var selectables = GetComponentsInChildren<Selectable>();
            foreach (var selectable in selectables) selectable.interactable = false;
        }

        /// <summary>
        ///     Set the initial state of the menu elements (animations)
        /// </summary>
        internal void InstantShow()
        {
            gameObject.SetActive(true);
            Sequencer.InstantShow();
            onShow.Invoke();
        }

        /// <summary>
        ///     Set the final state of the menu elements for all animations/actions
        /// </summary>
        internal void InstantHide()
        {
            Sequencer.InstantHide();
            onHide.Invoke();
            gameObject.SetActive(false);
        }

        /// <summary>
        ///     Select the first selectable if there is one and event system is null, after a delay
        /// </summary>
        private async UniTask SelectFirstDelay()
        {
            if (useSelectionTime)
                await UniTask.Delay(TimeSpan.FromSeconds(timeToSelect));

            if (firstSelected && !EventSystem.current.currentSelectedGameObject)
                firstSelected.Select();
        }

        #endregion
    }
}