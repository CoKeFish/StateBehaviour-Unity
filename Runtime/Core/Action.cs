#if STATE_BEHAVIOR_ENABLED
using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Marmary.StateBehavior.Core
{
    /// <summary>
    ///     Base action that binds a <see cref="ActionData{TState, TValue}"/> asset and exposes creation utilities in-editor.
    /// </summary>
    [Serializable]
    public abstract class Action<TState, TValue> where TState : Enum
    {
        #region Serialized Fields

        /// <summary>
        ///     Scriptable object containing the configuration for each selectable state.
        /// </summary>
        [FormerlySerializedAs("Data")] [InlineEditor] public ActionData<TState, TValue> data;

        #endregion

#if UNITY_EDITOR
        /// <summary>
        ///     Creates the backing <see cref="ScriptableObject"/> used by this action.
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