using UnityEngine;


namespace GGS.OpenInput
{
    /// <summary>
    /// Wraps UnityEngine.Input in an IScreenInput
    /// </summary>
    public class UnityInputWrapper : MonoBehaviour, IInput
    {
        public event System.Action Updated = delegate { };

        public float Time { get; private set; }
        public float DeltaTime { get; private set; }
        public Vector3 PointerPosition { get; private set; }
        public bool IsPointerDown { get; private set; }


        //called by Unity via Reflection
        private void Update()
        {
            PointerPosition = Input.mousePosition;
            IsPointerDown = Input.GetMouseButtonDown(0);
            Time = UnityEngine.Time.time;
            DeltaTime = UnityEngine.Time.deltaTime;

            Updated.Invoke();
        }

    }
}
