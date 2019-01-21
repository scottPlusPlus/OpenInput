using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace GGS.IInput
{
    public delegate void PointerEventHandler(object sender, PointerEventData pointerEventData);


    public interface IScreenInput
    {
        event PointerEventHandler PointerUp;
        event PointerEventHandler PointerDown;
        Vector3 PointerPosition { get; }
    }


    public interface IMouseDownHandler
    {
        void OnMouseDown();
    }


    public interface IMouseUpHandler
    {
        void OnMouseUp();
    }

    public interface IMouseEnterHandler
    {
        void OnMouseEnter();
    }


    public interface IMouseExitHandler
    {
        void OnMouseExit();
    }
}