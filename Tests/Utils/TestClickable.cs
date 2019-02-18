using UnityEngine;


namespace GGS.OpenInput.Test
{
    [RequireComponent(typeof(Collider))]
    public class TestClickable : MonoBehaviour, IMouseDownHandler, IMouseUpHandler
    {
        public event System.Action Clicked = delegate { };
    
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
                Clicked.Invoke();
            }
        }


    }
}
