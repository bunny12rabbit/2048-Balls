using UnityEngine;

namespace Interfaces
{
    public interface IInputHandler
    {
        void OnPress(Vector3 position);
        void OnDrag(Vector3 position);
        void OnRelease();
    }
}