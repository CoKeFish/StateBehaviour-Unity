using System;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Scrips.StateBehavior
{
    [Serializable]
    public abstract class Action<TState, TValue> where TState : Enum
    {
        [InlineEditor] public ActionData<TState, TValue> Data;
        
#if UNITY_EDITOR
        protected abstract ScriptableObject CreateInstanceScriptableObject();


        [Button("Crear ScriptableObject")]
        private void CreateScriptableObject()
        {

            
            // Crear instancia
            var asset = CreateInstanceScriptableObject();

            // Pedir ubicación al usuario
            string path = EditorUtility.SaveFilePanelInProject(
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