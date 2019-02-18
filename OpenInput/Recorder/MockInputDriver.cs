using GGS.OpenInput.Utils;
using UnityEngine;


namespace GGS.OpenInput
{
    public class MockInputDriver : IInput
    {
        #region Interface

        public event System.Action Updated = delegate { };
   
        public float Time { get; private set; }
        public float DeltaTime { get; private set; }
        public bool IsPointerDown { get; private set; }
        public Vector3 PointerPosition { get; private set; }

        #endregion

        private MockScreenInput _data;
        private MockScreenInput.Frame _previousFrame;
        private MockScreenInput.Frame _nextFrame;

        private bool _running;
        private System.Action _playCompleteCallback;
        private float _startTime;


        public void PlayInput(MockScreenInput mockInput, System.Action playCompleteCallback, float startTime)
        {
            if (mockInput == null)
            {
                Debug.LogError("Mock Input cannot be null");
                return;
            }

            _data = mockInput.Clone();
            _startTime = startTime;
            _nextFrame = new MockScreenInput.Frame(0f, Vector2.zero, false);
            _playCompleteCallback = playCompleteCallback;
            _running = true;

            StartNextMove();
        }


        public void Kill()
        {
            _running = false;
            _data = null;
            if (_playCompleteCallback != null)
            {
                _playCompleteCallback.Invoke();
                _playCompleteCallback = null;
            }
            Debug.Log("Finished playing input");
        }


        private void StartNextMove()
        {
            if (_data.Frames.Count <= 0)
            {
                Kill();
                return;
            }
            _previousFrame = _nextFrame;
            _nextFrame = _data.Frames[0];
            _data.Frames.RemoveAt(0);
            IsPointerDown = _nextFrame.Clicking;
        }


        public void Update(float time)
        {
            if (!_running)
            {
                return;
            }

            DeltaTime = time - Time;
            Time = time;

            time = time - _startTime;
            float progress = time.ReMap(_previousFrame.Time, _nextFrame.Time, 0, 1);
            progress = Mathf.Clamp01(progress);
            if (float.IsNaN(progress)) //will usually be NAN if starting time of sequence == 0;
            {
                progress = 1;
            }

            PointerPosition = Vector2.Lerp(_previousFrame.Position, _nextFrame.Position, progress);
            if (progress >= 1 || _previousFrame.Time >= _nextFrame.Time)
            {
                StartNextMove();
            }

            Updated.Invoke();
        }

    }
}