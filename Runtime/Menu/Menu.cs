using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Marmary.StateBehavior.Runtime.Criterions;
using Marmary.StateBehavior.Runtime.SwitchState;
using Marmary.Utils.Runtime;
using Marmary.Utils.Runtime.Structure.FlowControl;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
    public class Menu : SerializedMonoBehaviour, IInitialize, ISetup
    {
        #region Serialized Fields

        /// <summary>
        ///     The menu elements represent the elements of the menu that will be animated
        ///     It not necessarily has to be the children of the menu, it can be any game object
        /// </summary>
        [SerializeField] [BoxGroup("Menu")] private List<MenuElement> menuElements;


        /// <summary>
        ///     The time delay, in seconds, between the sequential animation of menu elements.
        ///     This value determines the interval at which each menu element is shown or hidden,
        ///     ensuring a smooth and timed transition sequence.
        /// </summary>
        public float separation;

        /// <summary>
        ///     First selectable of the menu when it is activated.
        ///     Ignored (and hidden in the inspector) while <see cref="initialFocus" /> is assigned,
        ///     since that one takes priority.
        /// </summary>
        [SerializeField]
        [HideIf(nameof(initialFocus))]
        [ChildGameObjectsOnly]
        [BoxGroup("Options")]
        [PropertySpace(SpaceAfter = 10)]
        public Selectable firstSelected;

        /// <summary>
        ///     Optional element that receives the menu's initial focus in its own way (e.g. a list
        ///     widget that focuses its selected item instead of its frame). Takes priority over
        ///     <see cref="firstSelected" /> when assigned.
        /// </summary>
        [OdinSerialize] [ShowInInspector] [BoxGroup("Options")]
        public IInitialFocus initialFocus;


        /// <summary>
        ///     A dictionary that maps positions (e.g., Top, Bottom, Left, Right) to their corresponding default selectable
        ///     elements.
        ///     This allows specific UI elements to be associated with directional positions within the menu, enabling customized
        ///     navigation behavior.
        /// </summary>
        [OdinSerialize] [ShowInInspector] [BoxGroup("Options")]
        public Dictionary<Position, Selectable> DefaultSelectables = new();


        /// <summary>
        ///     Indicates if the menu has a deactivation delay
        /// </summary>
        [SerializeField] [BoxGroup("Options")] [ToggleGroup("Options/useHideAnimation")]
        public bool useHideAnimation;

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
        ///     Indicates whether the menu elements should be recalculated at runtime.
        ///     When enabled, dynamically adjusts elements such as sorting and positioning
        ///     based on runtime criteria like spacing or layout.
        /// </summary>
        [SerializeField] [BoxGroup("Options")] private bool recalculateInRuntime;

        #endregion

        #region Fields

        /// <summary>
        ///     Input gate of the menu. Obtained (or added) at runtime in <see cref="Initialize" /> so
        ///     existing prefabs and scenes do not need a manual CanvasGroup.
        /// </summary>
        private CanvasGroup _canvasGroup;

        /// <summary>
        ///     Cancels pending activation/deactivation delays when the menu is destroyed.
        /// </summary>
        private CancellationToken _destroyToken;

        #endregion

        #region Methods

        /// <summary>
        ///     Activate the menu
        ///     1- Activate the menu game object (input gated unless <paramref name="blockInputUntilComplete" /> is false)
        ///     2- Show the menu elements (animations)
        ///     3- Select the first selectable if there is one and nothing is selected
        ///     4- Open the input gate
        /// </summary>
        /// <param name="blockInputUntilComplete">
        ///     If true the input gate stays closed until the show sequence finishes; if false it opens immediately.
        /// </param>
        internal async UniTask ActivateMenu(bool blockInputUntilComplete = true)
        {
            gameObject.SetActive(true);
            SetInteractable(!blockInputUntilComplete);
            onShow.Invoke();

            if (useShowAnimation)
            {
                if (useExtraActivationDelay)
                    await UniTask.Delay(TimeSpan.FromSeconds(extraDelayBeforeActivating),
                        cancellationToken: _destroyToken);
                await ShowElements();
            }
            else
            {
                InstantShowElements();
            }

            await SelectFirstDelay();

            SetInteractable(true);
            onShowComplete.Invoke();
        }

        /// <summary>
        ///     Inactivate the menu
        ///     1- Close the input gate
        ///     2- Hide the menu elements (animations)
        ///     3- Deactivate the menu game object
        /// </summary>
        internal async UniTask InactivateMenu()
        {
            SetInteractable(false);
            onHide.Invoke();

            if (useHideAnimation)
            {
                if (useExtraDelayBeforeDeactivating)
                    await UniTask.Delay(TimeSpan.FromSeconds(extraDelayBeforeDeactivating),
                        cancellationToken: _destroyToken);
                await HideElements();
            }
            else
            {
                InstantHideElements();
            }

            gameObject.SetActive(false);
            onHideComplete.Invoke();
        }

        /// <summary>
        ///     Opens or closes the input gate of the menu. Closing it blocks pointer raycasts and, combined with
        ///     <see cref="SelectableState.SelectableElement" /> respecting interactability, submit/keyboard input too.
        /// </summary>
        internal void SetInteractable(bool value)
        {
            EnsureCanvasGroup();
            _canvasGroup.interactable = value;
            _canvasGroup.blocksRaycasts = value;
        }

        /// <summary>
        ///     Gives this menu the initial focus. Order: <see cref="initialFocus" /> (an element
        ///     that knows how to receive the focus itself) wins; otherwise
        ///     <see cref="firstSelected" />; otherwise the first active and interactable
        ///     Selectable — so focus-based navigation (keyboard/gamepad) always has an entry point.
        /// </summary>
        internal void SelectFirst()
        {
            if (initialFocus != null)
            {
                initialFocus.Focus();
                return;
            }

            if (firstSelected)
            {
                firstSelected.Select();
                return;
            }

            foreach (var selectable in GetComponentsInChildren<Selectable>())
            {
                if (!selectable.IsInteractable()) continue;

                selectable.Select();
                return;
            }
        }

        /// <summary>
        ///     Gets the CanvasGroup used as input gate, adding one at runtime if the menu has none.
        /// </summary>
        private void EnsureCanvasGroup()
        {
            if (_canvasGroup) return;
            if (!TryGetComponent(out _canvasGroup)) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        /// <summary>
        ///     Set the initial state of the menu elements (animations)
        /// </summary>
        internal void InstantShow()
        {
            gameObject.SetActive(true);
            InstantShowElements();
            onShow.Invoke();
        }

        /// <summary>
        ///     Set the final state of the menu elements for all animations/actions
        /// </summary>
        internal void InstantHide()
        {
            InstantHideElements();
            onHide.Invoke();
            gameObject.SetActive(false);
        }

        /// <summary>
        ///     Get all menu elements
        /// </summary>
        private void GetAllMenuElements()
        {
            menuElements = GetComponentsInChildren<MenuElement>().ToList();
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);

#endif
        }

        /// <summary>
        ///     Initiates the hiding process for all menu elements and waits for their animations or tasks to complete.
        /// </summary>
        /// <returns>
        ///     A UniTask that completes when all menu element tasks are finished.
        /// </returns>
        private UniTask HideElements()
        {
            var tasks = new List<UniTask>(menuElements.Count);
            foreach (var menuElement in menuElements)
            {
                menuElement.OnHide();
                tasks.Add(menuElement.WhenTaskCompleted());
            }

            return UniTask.WhenAll(tasks);
        }

        /// <summary>
        ///     Triggers the "show" behavior for all menu elements asynchronously and waits for all animations to complete.
        /// </summary>
        /// <return>
        ///     A UniTask that completes when all menu element tasks have finished executing.
        /// </return>
        private UniTask ShowElements()
        {
            var tasks = new List<UniTask>(menuElements.Count);
            foreach (var menuElement in menuElements)
            {
                menuElement.OnShow();
                tasks.Add(menuElement.WhenTaskCompleted());
            }

            return UniTask.WhenAll(tasks);
        }


        /// <summary>
        ///     Set the initial state of the menu elements (animations)
        /// </summary>
        private void InstantShowElements()
        {
            foreach (var menuElement in menuElements) menuElement.OnShowInstant();
        }

        /// <summary>
        ///     Set the final state of the menu elements for all animations/actions
        /// </summary>
        private void InstantHideElements()
        {
            foreach (var menuElement in menuElements) menuElement.OnHideInstant();
        }

        /// <summary>
        ///     Select the first selectable if there is one and event system is null, after a delay
        /// </summary>
        private async UniTask SelectFirstDelay()
        {
            if (useSelectionTime)
                await UniTask.Delay(TimeSpan.FromSeconds(timeToSelect), cancellationToken: _destroyToken);

            // Keep a selection the user can actually use; a missing selection — or one stranded in
            // an input-gated menu (e.g. the menu behind a popup) — is replaced by this menu's first.
            if (CurrentSelectionIsUsable()) return;

            SelectFirst();
        }

        /// <summary>
        ///     Whether the EventSystem currently has a selection that can receive input.
        /// </summary>
        private static bool CurrentSelectionIsUsable()
        {
            var eventSystem = EventSystem.current;
            if (!eventSystem || !eventSystem.currentSelectedGameObject) return false;

            var selectable = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
            return selectable && selectable.IsInteractable();
        }

        #endregion

        #region Editor

        /// <summary>
        ///     Hides the menu.
        ///     1- Deactivates the menu functionality.
        ///     2- Initiates the menu inactivation process asynchronously.
        /// </summary>
        [Button]
        public void Hide()
        {
            InactivateMenu().Forget();
        }

        /// <summary>
        ///     Displays the menu by activating it.
        ///     1- Triggers the activation process for the menu.
        ///     2- Ensures that all necessary elements and animations related to showing the menu are executed.
        ///     3- Calls the internal method to handle asynchronous activation logic.
        /// </summary>
        [Button]
        public void Show()
        {
            ActivateMenu().Forget();
        }

        #endregion

        #region IInitialize Members

        /// <summary>
        ///     Initializes the Menu by performing the following operations:
        ///     1. Retrieves all menu elements using the GetAllMenuElements method.
        ///     2. If the recalculateInRuntime flag is true, sorts and arranges the menu elements
        ///     using the ElementSequencer with criteria based on height and a specified separation value.
        /// </summary>
        public void Initialize()
        {
            _destroyToken = this.GetCancellationTokenOnDestroy();
            EnsureCanvasGroup();
            GetAllMenuElements();

            if (recalculateInRuntime)
                ElementSequencer<SwitchState.SwitchState, SwitchTrigger, MenuElement>.SetSortedElements(
                    menuElements.ToList(), new RectTransformHeightCriterion<MenuElement>(),
                    separation);

            foreach (var menuElement in menuElements) menuElement.Initialize();
        }

        #endregion

        #region ISetup Members

        /// <summary>
        ///     Configures the menu by initializing its sequencer.
        ///     1- Calls the sequencer's setup method with the current menu instance.
        ///     2- Invokes the default setup method of the sequencer for any additional initialization.
        /// </summary>
        [Button]
        public void Setup()
        {
            GetAllMenuElements();
            ElementSequencer<SwitchState.SwitchState, SwitchTrigger, MenuElement>.SetSortedElements(
                menuElements.ToList(), new RectTransformHeightCriterion<MenuElement>(),
                separation);
        }

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
        ///     Event that is triggered when the hiding animation or process of the menu has completed.
        /// </summary>
        [SerializeField] [FoldoutGroup("Events")]
        public UnityEvent onHideComplete;

        /// <summary>
        ///     Event triggered upon the completion of the menu's show animation or process.
        /// </summary>
        [SerializeField] [FoldoutGroup("Events")]
        public UnityEvent onShowComplete;

        #endregion
    }
}