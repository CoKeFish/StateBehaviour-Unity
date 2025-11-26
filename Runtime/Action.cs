#if STATE_BEHAVIOR_ENABLED
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using Ardalis.GuardClauses;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    ///     Base action that binds a <see cref="ActionData{TState, TValue}" /> asset and exposes creation utilities in-editor.
    /// </summary>
    [Serializable]
    public abstract class Action<TState, TValue>:IStateContract<TState> where TState : Enum
    {
        
        /// <summary>
        ///     Captured value before applying action data.
        /// </summary>
        protected TValue originalValue;

        /// <summary>
        ///     Tween instance reused across state changes.
        /// </summary>
        protected Tweener tweener;
        
        /// <summary>
        ///     Determines whether the action needs an ActionData asset.
        /// </summary>
        private bool NeedsActionData => data == null;
        
        #region Serialized Fields

        /// <summary>
        ///     Scriptable object containing the configuration for each selectable state.
        /// </summary>
        [FormerlySerializedAs("Data")] [InlineEditor]
        public ActionData<TState, TValue> data;

        #endregion


        /// <summary>
        /// Sets up the action by initializing the starting value and creating a reusable tweener for state transitions.
        /// </summary>
        /// <param name="gameObject">Game object hosting the component.</param>
        public void Setup(GameObject gameObject)
        {
            InitializeStartValue(gameObject);
            tweener = CreateTweener(gameObject);
        }

        /// <summary>
        /// Sets the behavior action for the specified state and applies the corresponding data to the existing tweener.
        /// </summary>
        /// <param name="state">The state for which the behavior action is to be configured.</param>
        public virtual UniTask Set(TState state)
        {
            Guard.Against.Null(tweener);
            BehaviorActionFactory.Set(data.StateActionDataContainers[state].behaviorActionType, tweener);
            data.StateActionDataContainers[state].BehaviorActionData.ApplyData(tweener, originalValue).Restart();
            return AwaitTweenerCompletion();
        }


        /// <summary>
        ///     Applies the behaviour for the supplied state instantly without animation.
        /// </summary>
        /// <param name="state">State to activate.</param>
        public virtual void InstantSet(TState state)
        {
            Guard.Against.Null(tweener);
            BehaviorActionFactory.Set(BehaviorActionTypes.Instant, tweener);
            data.StateActionDataContainers[state].BehaviorActionData.ApplyData(tweener, originalValue).Restart();
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
        ///     Waits for the currently configured tweener to complete.
        /// </summary>
        /// <returns>A UniTask that completes when the tween finishes.</returns>
        protected UniTask AwaitTweenerCompletion()
        {
            if (tweener == null || !tweener.IsActive())
                return UniTask.CompletedTask;

            var completionTask = tweener.AsyncWaitForCompletion();
            return completionTask.AsUniTask();
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
        [ShowIf(nameof(NeedsActionData))]
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
                
                // Asignar automáticamente el ScriptableObject creado al campo data
                if (asset is ActionData<TState, TValue> actionData)
                {
                    // Registrar el cambio en el sistema de Undo y marcar el objeto como modificado
                    var selectedObject = Selection.activeGameObject;
                    if (selectedObject != null)
                    {
                        var element = selectedObject.GetComponent<MonoBehaviour>();
                        if (element != null)
                        {
                            Undo.RecordObject(element, "Assign ActionData");
                            data = actionData;
                            EditorUtility.SetDirty(element);
                        }
                        else
                        {
                            // Si no encontramos el componente, simplemente asignar el valor
                            data = actionData;
                        }
                    }
                    else
                    {
                        // Si no hay objeto seleccionado, simplemente asignar el valor
                        data = actionData;
                    }
                }
                
                Debug.Log($"ActionData creado en: {path}");
            }
            else
            {
                // Si el usuario cancela, destruir la instancia temporal
                Object.DestroyImmediate(asset);
            }
        }
#endif
    }
}
#endif