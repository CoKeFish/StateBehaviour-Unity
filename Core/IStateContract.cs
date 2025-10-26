using UnityEngine;

namespace Scrips.StateBehavior
{
    public interface IStateContract<TState>
    {
        void Setup(GameObject value);

        public void Set(TState state);
    }
}