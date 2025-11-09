#if UNITY_EDITOR

using MyCompany.ModuleSymbols;
using UnityEditor;

//TODO: Check namespace name rule
namespace Marmary.StateBehavior
{
#if MODULE_SYMBOLS_SYSTEM_ENABLED

    /// <summary>
    /// Provides functionality for symbol registration in the StateBehavior system.
    /// </summary>
    /// <remarks>
    /// This class is responsible for handling the registration of symbols specific to the
    /// StateBehavior functionality. It ensures that relevant symbols are registered when
    /// the required compilation symbols (e.g., MODULE_SYMBOLS_SYSTEM_ENABLED) are defined.
    /// The functionality is executed during the initialization phase in Unity Editor.
    /// </remarks>
    [InitializeOnLoad]
    public static class StateBehaviorRegister

    {
        #region Constructors and Injected

        static StateBehaviorRegister()

        {
            var desc = new ModuleSymbolDescriptor

            {
                moduleName = "State Behavior",

                options = new[]

                {
                    new SymbolOption
                    {
                        symbol = "STATE_BEHAVIOR_ENABLED",
                        description =
                            "Habilita el sistema de State Behavior para máquinas de estado y elementos interactivos",
                        enabledByDefault = false
                    }
                }
            };

            ModuleSymbolRegistry.Register(desc);
        }

        #endregion
    }

#endif
}

#endif