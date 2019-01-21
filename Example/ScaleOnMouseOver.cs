using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGS.IInput.Example
{
    public class ScaleOnMouseOver : MonoBehaviour, IMouseEnterHandler, IMouseExitHandler
    {

        public void OnMouseEnter()
        {
            transform.localScale = Vector3.one * 2f;
        }

        public void OnMouseExit()
        {
            transform.localScale = Vector3.one;
        }

    }
}
