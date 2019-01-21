using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGS.IInput
{
    /// <summary>
    /// Wraps UnityEngine.Input to drive a ScreenInputService
    /// </summary>
    public class UnityInputWrapper : MonoBehaviour
    {
        private ScreenInputService _screenInput;
        private Vector3 _currentPos;


        public void Setup(ScreenInputService screenInput)
        {
            _screenInput = screenInput;
        }


        void Update()
        {
            if (Input.mousePosition != _currentPos)
            {
                _screenInput.SetPosition(Input.mousePosition);
            }
            _screenInput.Update();
            if (Input.GetMouseButtonDown(0))
            {
                _screenInput.SetPointerDown();
            }
            if (Input.GetMouseButtonUp(0))
            {
                _screenInput.SetPointerUp();
            }
            _currentPos = Input.mousePosition;
        }

    }
}
