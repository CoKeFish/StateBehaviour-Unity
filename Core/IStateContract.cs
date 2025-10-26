using UnityEngine;

namespace Marmary.StateBehavior.Core
{
    public interface IStateContract<TState>
    {
        void Setup(GameObject value);

        public void Set(TState state);
    }
}