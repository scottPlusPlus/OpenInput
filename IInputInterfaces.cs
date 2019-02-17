using UnityEngine;
using UnityEngine.EventSystems;


namespace GGS.OpenInput
{
    public delegate void PointerEventHandler(object sender, PointerEventData pointerEventData);


    public interface IScreenInput
    {
        event System.Action Updated;

        float Time { get; }
        float DeltaTime { get; }
        Vector3 PointerPosition { get; }
        bool IsPointerDown { get; }
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