using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;


namespace GGS.IInput
{
    public class ScreenInputService : IScreenInput, IDisposable
    {
        #region Interface

        public event PointerEventHandler PointerDown;
        public event PointerEventHandler PointerUp;
        public Vector3 PointerPosition { get { return _pos; } }

        #endregion

        public bool DrivePointers { get; set; }

        private Vector3 _pos;
        private bool _pointerDown;

        private Ray _ray;
        private RaycastHit _hit;
        private GameObject _currentObj;
        private List<Camera> _activeCameras = new List<Camera>();
        private List<KeyValuePair<GameObject, int>> _foundUIItemsAndSorting = new List<KeyValuePair<GameObject, int>>();

        IPointerUpHandler[] _upHandlers = new IPointerUpHandler[0];
        IPointerDownHandler[] _downHandlers = new IPointerDownHandler[0];
        IPointerExitHandler[] _exitHandlers = new IPointerExitHandler[0];
        IPointerClickHandler[] _clickHandlers = new IPointerClickHandler[0];
        IMouseDownHandler[] _mouseDownHandlers = new IMouseDownHandler[0];
        IMouseUpHandler[] _mouseUpHandlers = new IMouseUpHandler[0];
        IMouseExitHandler[] _mouseExitHandlers = new IMouseExitHandler[0];


        public void Dispose()
        {
            _upHandlers = null;
            _downHandlers = null;
            _exitHandlers = null;
            _clickHandlers = null;
            _mouseDownHandlers = null;
            _mouseUpHandlers = null;
            _mouseExitHandlers = null;
        }

        
        public void Update()
        {
            GameObject foundObj = null;
            EnsureActiveCameras();
            _foundUIItemsAndSorting.Clear();
            foundObj = ObjectFromUI();
            if (foundObj == null)
            {
                foundObj = ObjectFromWorld();
            }
            UpdateCurrentObj(foundObj);
        }


        private GameObject ObjectFromWorld()
        {
            foreach (Camera cam in _activeCameras)
            {
                _ray = cam.ScreenPointToRay(_pos);
                if (Physics.Raycast(_ray, out _hit))
                {
                    return _hit.collider.gameObject;
                }
            }
            return null;
        }


        private GameObject ObjectFromUI()
        {
            GraphicRaycaster[] raycasters = GameObject.FindObjectsOfType<GraphicRaycaster>();
            raycasters = raycasters.OrderBy(o => o.sortOrderPriority).ToArray();

            foreach (GraphicRaycaster raycaster in raycasters)
            {
                EventSystem eventSystem = raycaster.GetComponent<EventSystem>();

                //Set up the new Pointer Event
                PointerEventData pointerEventData = new PointerEventData(eventSystem);
                //Set the Pointer Event Position to that of the mouse position
                pointerEventData.position = _pos;

                //Create a list of Raycast Results
                List<RaycastResult> results = new List<RaycastResult>();

                //Raycast using the Graphics Raycaster and mouse click position
                raycaster.Raycast(pointerEventData, results);

                //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
                foreach (RaycastResult result in results)
                {
                    _foundUIItemsAndSorting.Add(new KeyValuePair<GameObject, int>(result.gameObject, raycaster.sortOrderPriority));
                    //Debug.LogFormat("Hit {0} at sorting {1}", result.gameObject.name, raycaster.sortOrderPriority);
                }
            }

            if (_foundUIItemsAndSorting.Count > 0)
            {
                KeyValuePair<GameObject, int> best = new KeyValuePair<GameObject, int>(null, int.MinValue);
                foreach (KeyValuePair<GameObject, int> kvp in _foundUIItemsAndSorting)
                {
                    if (kvp.Value > best.Value)
                    {
                        best = kvp;
                    }
                }
                return best.Key;
            }
            return null;
        }


        private void EnsureActiveCameras()
        {
            _activeCameras.Clear();
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (Camera c in cameras)
            {
                if (c.isActiveAndEnabled)
                {
                    _activeCameras.Add(c);
                }
            }
        }


        public void SetPosition(Vector3 pos)
        {
            _pos = pos;
            Update();
        }


        private void UpdateCurrentObj(GameObject go)
        {
            if (go == _currentObj)
            {
                return;
            }

            PointerEventData ev = new PointerEventData(null);

            //update old
            if (DrivePointers)
            {
                foreach (IPointerExitHandler exit in _exitHandlers)
                {
                    exit.OnPointerExit(ev);
                }
                foreach (IMouseExitHandler exit in _mouseExitHandlers)
                {
                    exit.OnMouseExit();
                }
            }


            //update new
            _currentObj = go;
            if (_currentObj == null)
            {
                _upHandlers = new IPointerUpHandler[0];
                _downHandlers = new IPointerDownHandler[0];
                _exitHandlers = new IPointerExitHandler[0];
                _clickHandlers = new IPointerClickHandler[0];
                _mouseDownHandlers = new IMouseDownHandler[0];
                _mouseUpHandlers = new IMouseUpHandler[0];
                _mouseExitHandlers = new IMouseExitHandler[0];
                return;
            }

            if (DrivePointers)
            {
                foreach (IPointerEnterHandler enter in _currentObj.GetComponents<IPointerEnterHandler>())
                {
                    enter.OnPointerEnter(ev);
                }
                foreach (IMouseEnterHandler enter in _currentObj.GetComponents<IMouseEnterHandler>())
                {
                    enter.OnMouseEnter();
                }
            }

            _upHandlers = _currentObj.GetComponents<IPointerUpHandler>();
            _downHandlers = _currentObj.GetComponents<IPointerDownHandler>();
            _exitHandlers = _currentObj.GetComponents<IPointerExitHandler>();
            _clickHandlers = _currentObj.GetComponents<IPointerClickHandler>();
            _mouseUpHandlers = _currentObj.GetComponents<IMouseUpHandler>();
            _mouseDownHandlers = _currentObj.GetComponents<IMouseDownHandler>();
            _mouseExitHandlers = _currentObj.GetComponents<IMouseExitHandler>();
        }


        public void SetPointerDown()
        {
            UpdatePointer(true);
        }

        public void SetPointerUp()
        {
            UpdatePointer(false);
        }

        private void UpdatePointer(bool state)
        {
            if (_pointerDown == state)
            {
                return;
            }

            _pointerDown = state;
            PointerEventData ev = new PointerEventData(null);

            if (_pointerDown)
            {
                if (DrivePointers)
                {
                    foreach (IPointerDownHandler down in _downHandlers)
                    {
                        down.OnPointerDown(ev);
                    }
                    foreach (IMouseDownHandler down in _mouseDownHandlers)
                    {
                        down.OnMouseDown();
                    }
                    //TODO - does Click mean it needs both a down + up?
                    foreach (IPointerClickHandler click in _clickHandlers)
                    {
                        click.OnPointerClick(ev);
                    }
                }

                if (PointerDown != null)
                {
                    PointerDown.Invoke(this, ev);
                }
            }
            else
            {
                if (DrivePointers)
                {
                    foreach (IPointerUpHandler up in _upHandlers)
                    {
                        up.OnPointerUp(ev);
                    }
                    foreach (IMouseUpHandler up in _mouseUpHandlers)
                    {
                        up.OnMouseUp();
                    }
                }
                if (PointerUp != null)
                {
                    PointerUp.Invoke(this, ev);
                }
            }
        }




    }
}