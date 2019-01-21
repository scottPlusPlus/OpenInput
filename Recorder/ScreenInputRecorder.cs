using System.Collections.Generic;
using GGS.IInput.Utils;
using UnityEngine;


namespace GGS.IInput
{
    public class ScreenInputRecorder
    {
        private List<MockScreenInput.Frame> _recordedFrames = new List<MockScreenInput.Frame>();
        private Vector3 _currentDirection = Vector3.zero;
        private Vector3 _previousPos;
        private Vector3 _currentOffset;
        private bool _currentClicking = false;
        private float _startTime;


        public void StartRecording(float time)
        {
            Clear();
            _startTime = time;
        }


        public void AppendFrame(Vector3 position, bool clicking, float time)
        {
            time = time - _startTime;
            bool isKeyFrame = false;
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
    }
}