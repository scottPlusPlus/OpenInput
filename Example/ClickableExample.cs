using UnityEngine;
using UnityEngine.EventSystems;

namespace GGS.IInput.Example
{
    [RequireComponent(typeof(Collider))]
    public class ClickableExample : MonoBehaviour, IMouseDownHandler, IMouseUpHandler
    {

        public event System.Action Clicked;

        private bool _down;


        public void OnMouseDown()
        {
            _down = true;
        }

        public void OnMouseUp()
        {
            if (_down)
            {
                _down = false;
                if (Clicked != null)
                {
                    Clicked.Invoke();
                }
            }
        }

    }
}
