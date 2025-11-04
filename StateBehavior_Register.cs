#if UNITY_EDITOR

using UnityEditor;

using MyCompany.ModuleSymbols;

namespace Marmary.StateBehavior

{

    // Registro de símbolos para StateBehavior
    // Este registro solo se ejecutará si MODULE_SYMBOLS_SYSTEM_ENABLED está definido

#if MODULE_SYMBOLS_SYSTEM_ENABLED

    [InitializeOnLoad]

    public static class StateBehavior_Register

    {

        static StateBehavior_Register()

        {

            var desc = new ModuleSymbolDescriptor

            {

                moduleName = "State Behavior",

                options = new SymbolOption[]

                {

                    new SymbolOption { symbol = "STATE_BEHAVIOR_ENABLED", description = "Habilita el sistema de State Behavior para máquinas de estado y elementos interactivos", enabledByDefault = false }

                }

            };

            ModuleSymbolRegistry.Register(desc);

        }

    }

#endif

}

#endif

