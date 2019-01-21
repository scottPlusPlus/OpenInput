using System.Collections.Generic;
using System.Linq;
using GGS.IInput.Utils;
using UnityEngine;


namespace GGS.IInput
{
    public class ScreenInputMocker
    {

        private ScreenInputService _screenInput;

        private MockScreenInput _data;
        private MockScreenInput.Frame _previousFrame;
        private MockScreenInput.Frame _nextFrame;

        private bool _running;
        private System.Action _playCompleteCallback;
        private float _startTime;


        //NOTE: you'd probably want to wrap this in an IPromise or an IEnumerator, depending on your needs
        public ScreenInputMocker(ScreenInputService inputService, MockScreenInput mockInput, System.Action playCompleteCallback, float startTime)
        {
            //validation
            if (inputService == null)
            {
                Debug.LogError("Input Service null");
                return;
            }
            if (mockInput == null)
            {
                Debug.LogError("Mock Input null");
                return;
            }

            _screenInput = inputService;
            _data = mockInput.Clone();
            _startTime = startTime;
            _previousFrame = new MockScreenInput.Frame();
            _playCompleteCallback = playCompleteCallback;
            _running = true;

            _screenInput.SetPointerDown();
            _screenInput.SetPointerUp();

            StartNextMove();
        }


        public void Kill()
        {
            _running = false;
            _playCompleteCallback.Invoke();
            _playCompleteCallback = null;
            _screenInput = null;
            _data = null;
            Debug.Log("Finished playing input");
        }


        private void StartNextMove()
        {
            if (_data.Frames.Count <= 0)
            {
                Kill();
                return;
            }
            MockScreenInput.Frame nextFrame = _data.Frames[0];
            _data.Frames.RemoveAt(0);

            if (nextFrame.Clicking)
            {
                _screenInput.SetPointerDown();
            }
            else
            {
                _screenInput.SetPointerUp();
            }

            if (_screenInput == null)
            {
                Debug.LogError("Screen input null");
            }
            _previousFrame = _nextFrame;
            _nextFrame = nextFrame;
        }


        public void Update(float time)
        {
            if (_running)
            {
                time = time - _startTime;
                float progress = time.ReMap(_previousFrame.Time, _nextFrame.Time, 0, 1);
                Vector3 pos = Vector3.Lerp(_previousFrame.Position, _nextFrame.Position, progress);
                
                if (!pos.IsNAN())
                {
                    _screenInput.SetPosition(pos);
                }
                if (progress >= 1 || _previousFrame.Time >= _nextFrame.Time)
                {
                    StartNextMove();
                }
            }
        }

    }
}