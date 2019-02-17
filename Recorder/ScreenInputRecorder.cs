using System.Collections.Generic;
using GGS.OpenInput.Utils;
using UnityEngine;


namespace GGS.OpenInput
{
    public class ScreenInputRecorder
    {
        private List<MockScreenInput.Frame> _recordedFrames = new List<MockScreenInput.Frame>();
        private Vector3 _currentDirection = Vector3.zero;
        private Vector3 _previousPos;
        private Vector3 _currentOffset;
        private bool _currentClicking = false;
        private float _startTime;

        private IScreenInput _driver;

        public void StartRecording(IScreenInput driver, float time)
        {
            if (driver == null)
            {
                Debug.LogError("Driver cannot be null");
            }

            Clear();
            _driver = driver;
            _driver.Updated += OnUpdate;
            _startTime = time;
        }


        public void StopRecording()
        {
            if (_driver != null)
            {
                _driver.Updated -= OnUpdate;
                _driver = null;
            }
        }


        public List<MockScreenInput.Frame> RecordedKeyFrames()
        {
            return _recordedFrames;
        }


        public void Clear()
        {
            _recordedFrames.Clear();
            _currentDirection = Vector3.zero;
            _previousPos = Vector3.zero;
            _currentOffset = Vector3.zero;
            _currentClicking = false;
        }


        private void OnUpdate()
        {
            bool isKeyFrame = false;
            float time = _driver.Time - _startTime;
            Vector3 position = _driver.PointerPosition;
            bool clicking = _driver.IsPointerDown;

            _currentOffset = position - _previousPos;
            Vector3 dir = _currentOffset.Sign();
            if (dir != _currentDirection)
            {
                _currentDirection = dir;
                isKeyFrame = true;
            }
            //TODO - check for speed change
            if (clicking != _currentClicking)
            {
                _currentClicking = clicking;
                isKeyFrame = true;
            }
            if (isKeyFrame)
            {
                _recordedFrames.Add(CreateMove(position, time, clicking));
            }
            _previousPos = position;
        }


        private MockScreenInput.Frame CreateMove(Vector3 pos, float time, bool clicking)
        {
            MockScreenInput.Frame newMove = new MockScreenInput.Frame();
            newMove.Position = pos;
            newMove.Time = time;
            newMove.Clicking = clicking;
            return newMove;
        }



    }
}