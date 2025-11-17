using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DTT.ExtendedDebugLogs;
using Marmary.HellmenRaaun.Infrastructure;
using Marmary.StateBehavior.Menu;
using Marmary.StateBehavior.SwitchState;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marmary.Libraries.UI.Menu.Core
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
        ///     The menu elements represent the elements of the menu that will be animated
        ///     It not necessarily has to be the children of the menu, it can be any game object
        /// </summary>
        [SerializeField] [BoxGroup("Menu")] private SwitchElement[] menuElements;


        /// <summary>
        ///     Indicates the separation between elements
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [TitleGroup("Options/Utilities")]
        private float separation;

        /// <summary>
        ///     The sequencer that controls the order of menu elements.
        /// </summary>
        private MenuSequencer _sequencer;

        /// <summary>
        ///     The criterion used to determine the order of menu elements.
        ///     If null, a default height-based criterion will be used.
        /// </summary>
        [SerializeField] 
        [BoxGroup("Options")] 
        [TitleGroup("Options/Utilities")]
        [InfoBox("The sequencer will use a height-based criterion by default. Custom criteria can be set programmatically.")]
        private bool useCustomCriterion;

        #endregion


        /// <summary>
        ///     Sets up all the menu elements and initializes the sequencer.
        /// </summary>
        internal void Setup()
        {

            // Initialize sequencer with criterion
            ISequencingCriterion<RectTransform, SwitchState> criterion;
            if (useCustomCriterion && menuElements != null && menuElements.Length > 0)
            {
                // TODO: Allow custom criterion assignment via inspector
                // For now, default to height-based criterion
                criterion = new RectTransformHeightCriterion();
            }
            else
            {
                // Default to height-based criterion
                criterion = new RectTransformHeightCriterion();
            }

            _sequencer = new MenuSequencer(criterion, separation);
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
            //Show the menu elements (animations) and wait for them to finish for selecting the first selectable
            if (useExtraActivationDelay)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(extraDelayBeforeActivating));
                await ActivateMenuAnimation();
            }
            else
            {
                if (useShowAnimation)
                    await ActivateMenuAnimation();
                else
                    InstantShow();
            }


            await SelectFirstDelay();

            ActiveInteractables();

            onShow.Invoke();
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
        ///     Set the final state of the menu elements for all animations/actions
        /// </summary>
        internal void InstantHide()
        {
            menuElements.ForEach(menu => menu.InstantHide());

            onHide.Invoke();
            gameObject.SetActive(false);
        }

        /// <summary>
        ///     Set the initial state of the menu elements (animations)
        /// </summary>
        internal void InstantShow()
        {
            gameObject.SetActive(true);
            foreach (var menuElement in menuElements) menuElement.InstantShow();

            onShow.Invoke();
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


            if (useHideAnimation)
            {
                //Hide the menu elements (animations) using the sequencer order

                if (_sequencer != null && menuElements != null && menuElements.Length > 0)
                {
                    var sortedElements = _sequencer.SortElements(menuElements);
                    var tasks = sortedElements
                        .Select(menuElement => menuElement.Hide())
                        .ToArray();

                    await UniTask.WhenAll(tasks);
                }

                if (useExtraDelayBeforeDeactivating)
                    await UniTask.Delay(TimeSpan.FromSeconds(extraDelayBeforeDeactivating));


                //Deactivate the menu after the animations
                gameObject.SetActive(false);
                onHide.Invoke();
            }
            else
            {
                InstantHide();
            }
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
        ///     Select the first selectable if there is one and event system is null, after a delay
        /// </summary>
        private async UniTask SelectFirstDelay()
        {
            if (useSelectionTime)
                await UniTask.Delay(TimeSpan.FromSeconds(timeToSelect));

            if (firstSelected && !EventSystem.current.currentSelectedGameObject)
                firstSelected.Select();
        }

        /// <summary>
        ///     Show the menu elements (animations) using the sequencer order.
        /// </summary>
        private async UniTask ActivateMenuAnimation()
        {
            if (_sequencer == null || menuElements == null || menuElements.Length == 0)
                return;

            var sortedElements = _sequencer.SortElements(menuElements);
            var tasks = sortedElements
                .Select(menuElement => menuElement.Show())
                .ToArray();

            await UniTask.WhenAll(tasks);
        }

        #endregion

#if UNITY_EDITOR

        #region ISelfValidator Members

        /// <summary>
        ///     Called when the script is loaded or a value is
        ///     changed in the inspector (Called in the editor only).
        /// </summary>
        public void Validate(SelfValidationResult result)
        {
            //Verificamos si el objecto esta en una escena
            if (string.IsNullOrEmpty(gameObject.scene.name) ||
                string.Equals(gameObject.scene.name, gameObject.name)) return;
            GetAllMenuElements();
            CalculateTimes();
        }

        #endregion

        /// <summary>
        ///     Get all menu elements
        /// </summary>
        [Button(ButtonSizes.Large)]
        [BoxGroup("Menu")]
        public void GetAllMenuElements()
        {
            menuElements = GetComponentsInChildren<SwitchElement>();

            EditorUtility.SetDirty(this);
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
            delayBeforeDeactivating = 0f;
            delayBeforeActivating = 0f;

            if (menuElements == null) return;

            // TODO: Implement duration calculation based on SwitchElement actions
            // This will require accessing the ActionDataSimpleState duration values
            // For now, this is a placeholder that maintains the structure

            EditorUtility.SetDirty(this);
        }

        /// <summary>
        ///     Sets the sequencing criterion to use height-based ordering (top to bottom).
        ///     This uses the RectTransformHeightCriterion.
        /// </summary>
        [Button(ButtonSizes.Medium)]
        [BoxGroup("Options", order: 1)]
        [TitleGroup("Options/Utilities", order: 1)]
        [PropertySpace(SpaceAfter = 0, SpaceBefore = 0)]
        public void SetHeightBasedCriterion()
        {
            // Note: In a real implementation, you might want to create a ScriptableObject
            // instance for the criterion. For now, this is a placeholder that shows
            // how the sequencer would be configured.
            var criterion = new RectTransformHeightCriterion();
            _sequencer = new MenuSequencer(criterion, separation);
            
            EditorUtility.SetDirty(this);
        }

#endif
    }
}