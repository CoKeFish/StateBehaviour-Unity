#if STATE_BEHAVIOR_ENABLED
using System;
using Ardalis.GuardClauses;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Marmary.StateBehavior.Core
{
    /// <summary>
    ///     Base action that binds a <see cref="ActionData{TState, TValue}" /> asset and exposes creation utilities in-editor.
    /// </summary>
    [Serializable]
    public abstract class Action<TState, TValue> where TState : Enum
    {
        
        /// <summary>
        ///     Captured value before applying action data.
        /// </summary>
        protected TValue OriginalValue;

        /// <summary>
        ///     Tween instance reused across state changes.
        /// </summary>
        protected Tweener Tweener;
        
        #region Serialized Fields

        /// <summary>
        ///     Scriptable object containing the configuration for each selectable state.
        /// </summary>
        [FormerlySerializedAs("Data")] [InlineEditor]
        public ActionData<TState, TValue> data;

        #endregion


        /// <summary>
        /// Sets the behavior action for the specified state and applies the corresponding data to the existing tweener.
        /// </summary>
        /// <param name="state">The state for which the behavior action is to be configured.</param>
        public virtual void Set(TState state)
        {
            Guard.Against.Null(Tweener);
            BehaviorActionFactory.Set(data.StateActionDataContainers[state].behaviorActionType, Tweener);
            data.StateActionDataContainers[state].BehaviorActionData.ApplyData(Tweener, OriginalValue).Restart();
        }


        /// <summary>
        /// Sets up the action by initializing the starting value and creating a reusable tweener for state transitions.
        /// </summary>
        /// <param name="gameObject">Game object hosting the component.</param>
        public void Setup(GameObject gameObject)
        {
            InitializeStartValue(gameObject);
            Tweener = CreateTweener(gameObject);
        }
        
        /// <summary>
        ///     Creates the tweener that will be reused for state transitions.
        /// </summary>
        /// <param name="gameObject">Game object hosting the component.</param>
        /// <returns>Newly created tweener.</returns>
        protected abstract Tweener CreateTweener(GameObject gameObject);

        /// <summary>
        ///     Captures the starting value from the component prior to running animations.
        /// </summary>
        /// <param name="gameObject">Game object hosting the component.</param>
        protected abstract void InitializeStartValue(GameObject gameObject);


        /// <summary>
        ///     Applies the behaviour for the supplied state instantly without animation.
        /// </summary>
        /// <param name="state">State to activate.</param>
        public void InstantSet(TState state)
        {
            Guard.Against.Null(Tweener);
            BehaviorActionFactory.Set(BehaviorActionTypes.Instant, Tweener);
            data.StateActionDataContainers[state].BehaviorActionData.ApplyData(Tweener, OriginalValue).Restart();
        }
        
        
#if UNITY_EDITOR
        /// <summary>
        ///     Creates the backing <see cref="ScriptableObject" /> used by this action.
        /// </summary>
        /// <returns>Newly created scriptable object.</returns>
        protected abstract ScriptableObject CreateInstanceScriptableObject();


        /// <summary>
        ///     Opens a save dialog and creates a scriptable object asset for the action data.
        /// </summary>
        [Button("Crear ScriptableObject")]
        private void CreateScriptableObject()
        {
            // Crear instancia
            var asset = CreateInstanceScriptableObject();

            // Pedir ubicación al usuario
            var path = EditorUtility.SaveFilePanelInProject(
                "Guardar ActionData",
                typeof(ActionData<TState, TValue>).Name + ".asset",
                "asset",
                "Selecciona dónde guardar el ScriptableObject"
            );

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"ActionData creado en: {path}");
            }
        }
#endif
    }
}
#endif