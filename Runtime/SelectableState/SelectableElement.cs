#if STATE_BEHAVIOR_ENABLED
using Marmary.Utils.Runtime;
using Marmary.Utils.Runtime.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marmary.StateBehavior.Runtime.SelectableState
{
    /// <summary>
    ///     Represents an abstract base class for a selectable UI element.
    ///     Inherits from <c>Element</c> with a <c>SelectableState</c> and implements various Unity event interfaces.
    /// </summary>
    /// <remarks>
    ///     This class defines behavior for handling UI element interactions such as pointer events, selection, clicks,
    ///     cancellation, and state transitions.
    /// </remarks>
    [RequireComponent(typeof(Selectable))]
    public abstract class SelectableElement : Element<SelectableState, SelectableTrigger>
        , IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler,
        IPointerUpHandler, ISubmitHandler, ICancelHandler, IUnPressedHandler
    {
        #region Serialized Fields

        /// <summary>
        ///     Determines whether mouse input should be ignored for specific interactions.
        /// </summary>
        [SerializeField] private bool ignoreMouse;

        /// <summary>
        ///     Represents the event triggered when the UI element is clicked.
        ///     This event is invoked to handle click interactions and associated logic for the selectable element.
        /// </summary>
        public UnityEvent onClick = new();

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes the SelectableElement by creating and configuring the internal SelectableStateMachine instance.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            stateMachine = new SelectableStateMachine(SelectableState.Normal, actions, this, time, executeInstantly, onClick);
        }

        /// <summary>
        /// Invoked when the MonoBehaviour becomes active and is started.
        /// This method initializes the SelectableElement by calling the <c>Initialize</c> method.
        /// </summary>
        /// <remarks>
        /// The <c>Start</c> method is marked with the <c>IgnoreUnityLifecycle</c> attribute to indicate
        /// that its execution should not be treated as part of the standard Unity lifecycle for custom purposes.
        /// </remarks>
        [IgnoreUnityLifecycle]
        protected void Start()
        {
            Initialize();
        }

        #endregion

        #region ICancelHandler Members

        /// <inheritdoc />
        /// <summary>
        ///     Executes the cancel operation in response to the cancel event.
        /// </summary>
        /// <param name="eventData">The data associated with the cancel event.</param>
        public void OnCancel(BaseEventData eventData)
        {
            TriggerState(SelectableTrigger.Cancel);
        }

        #endregion

        #region IDeselectHandler Members

        /// <inheritdoc />
        /// <summary>
        ///     Execute the deselect function
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDeselect(BaseEventData eventData)
        {
            TriggerState(SelectableTrigger.Deselect);
        }

        #endregion

        #region IPointerClickHandler Members

        /// <inheritdoc />
        /// <summary>
        ///     Handles the click interaction when the pointer is clicked on the selectable element.
        /// </summary>
        /// <param name="eventData">The data associated with the pointer click event.</param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            TriggerState(SelectableTrigger.PointerClick);
        }

        #endregion

        #region IPointerDownHandler Members

        /// <inheritdoc />
        /// <summary>
        ///     Press the selectable when the pointer down
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            TriggerState(SelectableTrigger.PointerDown);
        }

        #endregion

        #region IPointerEnterHandler Members

        /// <inheritdoc />
        /// <summary>
        ///     Select the selectable when the pointer enter
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (ignoreMouse) return;

            gameObject.GetComponent<Selectable>().Select();
        }

        #endregion

        #region IPointerExitHandler Members

        /// <inheritdoc />
        /// <summary>
        ///     Executes the action when the pointer exits the selectable element.
        /// </summary>
        /// <param name="eventData">The data associated with the pointer event.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (ignoreMouse) return;
            //TODO: verificar si usar el mouse y el teclado al tiempo puede causar problemas
            EventSystem.current.SetSelectedGameObject(null);
        }

        #endregion

        #region IPointerUpHandler Members

        /// <inheritdoc />
        /// <summary>
        ///     Unpress the selectable when the pointer up
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            TriggerState(SelectableTrigger.PointerUp);
        }

        #endregion

        #region ISelectHandler Members

        /// <summary>
        ///     Executes the select function.
        /// </summary>
        /// <param name="eventData">The event data associated with the select event.</param>
        public virtual void OnSelect(BaseEventData eventData)
        {
            TriggerState(SelectableTrigger.Select);
        }

        #endregion

        #region ISubmitHandler Members

        /// <summary>
        ///     Execute OnPressed
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            TriggerState(SelectableTrigger.Submit);
        }

        #endregion

        #region IUnPressedHandler Members

        /// <inheritdoc />
        /// <summary>
        ///     Execute the unpressed functionality.
        /// </summary>
        /// <param name="eventData">The event data associated with the pointer action.</param>
        public void OnUnPressed(PointerEventData eventData)
        {
            TriggerState(SelectableTrigger.UnPressed);
        }

        #endregion
    }
}
#endif