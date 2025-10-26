using Marmary.Libraries.UI.Events;
using Marmary.StateBehavior.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marmary.StateBehavior.SelectableState
{
    
    public abstract class SelectableElement : Element<SelectableState>
        , IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler,
        IPointerUpHandler, ISubmitHandler, ICancelHandler, IUnPressedHandler
    {
        /// <summary>
        ///     Ignore the mouse input for the animation
        /// </summary>
        [SerializeField] private bool ignoreMouse;

        /// <summary>
        /// Represents the state machine that manages the selectable animation element's
        /// various states and transitions based on user input events such as
        /// selection, deselection, pressing, and clicking.
        /// </summary>
        private SelectableStateMachine _selectableStateMachine;


        protected void Awake()
        {
            
            _selectableStateMachine = new SelectableStateMachine(SelectableState.Normal, gameObject, actions, this);
        }


        #region IDeselectHandler Members

        /// <inheritdoc />
        /// <summary>
        ///     Execute the deselect function
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDeselect(BaseEventData eventData)
        {
            _selectableStateMachine.FireTrigger(SelectableTrigger.Deselect);
        }

        #endregion

        #region IPointerClickHandler Members

        /// <inheritdoc />
        /// <summary>
        ///     Click the selectable when the pointer click
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            _selectableStateMachine.FireTrigger(SelectableTrigger.PointerClick);
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
            _selectableStateMachine.FireTrigger(SelectableTrigger.PointerDown);
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
        ///     Unslect the selectable when the pointer exit
        /// </summary>
        /// <param name="eventData"></param>
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
            _selectableStateMachine.FireTrigger(SelectableTrigger.PointerUp);
        }

        #endregion

        #region ISelectHandler Members

        /// <summary>
        ///     Execute the selec function
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSelect(BaseEventData eventData)
        {
            _selectableStateMachine.FireTrigger(SelectableTrigger.Select);
        }

        #endregion

        #region ISubmitHandler Members

        /// <summary>
        ///     Execute OnPressed
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            _selectableStateMachine.FireTrigger(SelectableTrigger.Submit);
        }

        #endregion

        #region IUnPressedHandler Members

        /// <summary>
        ///     Execute OnUnPressed
        /// </summary>
        /// <param name="eventData"></param>
        public void OnUnPressed(PointerEventData eventData)
        {
            _selectableStateMachine.FireTrigger(SelectableTrigger.UnPressed);
        }

        #endregion

        public void OnCancel(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}