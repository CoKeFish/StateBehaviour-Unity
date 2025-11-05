using UnityEngine;

namespace Marmary.StateBehavior.Core
{
    public interface IStateContract<TState>
    {
        void Setup(GameObject value);

        #region Methods

        public void Set(TState state);

        #endregion
    }
}