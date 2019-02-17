using UnityEngine;


namespace GGS.OpenInput.Example
{
    [RequireComponent(typeof(Collider))]
    public class ClickableExample : MonoBehaviour, IMouseDownHandler, IMouseUpHandler
    {

        public event System.Action Clicked;

        private bool _down;


        public int Downs;
        public int Ups;

        public void OnMouseDown()
        {
            Debug.Log("Clickable.OnMouseDown!");
            _down = true;
            Downs++;
        }

        public void OnMouseUp()
        {
            Debug.Log("Clickable.OnMouseUp!");
            Ups++;
            if (_down)
            {
                _down = false;
                if (Clicked != null)
                {
                    Clicked.Invoke();
                } else
                {
                    Debug.Log("Clicked is null...");
                }
            }
        }

    }
}
