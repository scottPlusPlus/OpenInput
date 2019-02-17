using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace GGS.OpenInput
{
    /// <summary>
    /// ScreenInputService wraps an IScreenInput as a driver,
    /// then uses the input it provides to trigger colliders and UI elements in the scene
    /// calling OnMouseDown, OnPointerEnter, etc.
    /// </summary>
    public class ScreenInputService : IScreenInput, IDisposable
    {
        #region Interface

        public event System.Action Updated = delegate { };
        public event PointerEventHandler PointerDown = delegate { };
        public event PointerEventHandler PointerUp = delegate { };

        public float Time { get; private set; }
        public float DeltaTime { get; private set; }
        public bool IsPointerDown { get; private set; }
        public Vector3 PointerPosition { get; private set; }

        #endregion

        private IScreenInput _driver;

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

        private bool _driveColliders;

        public void Dispose()
        {
            _upHandlers = null;
            _downHandlers = null;
            _exitHandlers = null;
            _clickHandlers = null;
            _mouseDownHandlers = null;
            _mouseUpHandlers = null;
            _mouseExitHandlers = null;

            if (_driver != null)
            {
                _driver.Updated -= OnUpdate;
                _driver = null;
            }
        }


        /// <summary>
        /// Set the IScreenInput that will drive the ScreenInputService will wrap.
        /// </summary>
        /// <param name="driver">IScreenInput that the ScreenInputService will wrap</param>
        /// <param name="driveColliders">Should the ScreenInputService make calls to colliders and UI elements in the scene
        /// (OnMouseDown, OnPointerEnter, etc)</param>
        public void SetDriver(IScreenInput driver, bool driveColliders)
        {
            if (_driver != null)
            {
                _driver.Updated -= OnUpdate;
            }
            _driver = driver;
            if (_driver != null)
            {
                _driver.Updated += OnUpdate;
            }
            _driveColliders = driveColliders;
        }


        private void OnUpdate()
        {
            Time = _driver.Time;
            DeltaTime = _driver.DeltaTime;
            UpdatePosition(_driver.PointerPosition);
            UpdatePointerState(_driver.IsPointerDown);
        }


        #region UpdatePosition


        private void UpdatePosition(Vector3 pos)
        {
            if (PointerPosition == pos)
            {
                return;
            }

            PointerPosition = pos;

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
                _ray = cam.ScreenPointToRay(PointerPosition);
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
                pointerEventData.position = PointerPosition;

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


        private void UpdateCurrentObj(GameObject go)
        {
            if (go == _currentObj)
            {
                return;
            }

            PointerEventData ev = new PointerEventData(null);

            //update old
            if (_driveColliders)
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

            if (_driveColliders)
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


        #endregion
        #region UpdateState //clicking / not clicking


        private void UpdatePointerState(bool state)
        {
            if (IsPointerDown == state)
            {
                return;
            }

            IsPointerDown = state;
            PointerEventData ev = new PointerEventData(null);

            if (IsPointerDown)
            {
                if (_driveColliders)
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
                PointerDown.Invoke(this, ev);
            }
            else
            {
                if (_driveColliders)
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
                PointerUp.Invoke(this, ev);
            }
        }


        #endregion


    }
}