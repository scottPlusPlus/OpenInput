using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGS.IInput.Example
{
    public class ExampleGlue : MonoBehaviour
    {
        [SerializeField] private UnityInputWrapper _unityInputWrapper;
        [SerializeField] private KeyCode _toggleRecordKey;
        [SerializeField] private KeyCode _playbackKey;

        private ScreenInputService _screenInputService = new ScreenInputService();

        private bool _recording;
        private ScreenInputRecorder _recorder = new ScreenInputRecorder();
        private ScreenInputMocker _playbackRunner;


        // Use this for initialization
        void Start()
        {
            _unityInputWrapper.Setup(_screenInputService);
        }

        void Update()
        {
            //toggle recording
            if (Input.GetKeyDown(_toggleRecordKey))
            {
                if (!_recording)
                {
                    Debug.Log("Start Recording");
                    StartRecording();
                }
                else
                {
                    StopRecording();
                    Debug.LogFormat("Finished recording with {0} moves", _recorder.RecordedKeyFrames().Count);
                }
            }

            //playback
            if (Input.GetKeyDown(_playbackKey))
            {
                StartPlayBack();
            }

            //run
            if (_recording)
            {
                _recorder.AppendFrame(Input.mousePosition,
                    Input.GetMouseButton(0),
                    Time.time);
            }
            if (_playbackRunner != null)
            {
                _playbackRunner.Update(Time.time);
            }
        }


        public void StartRecording()
        {
            if (_recording)
            {
                Debug.LogWarning("Was already recording");
            }
            _recorder.StartRecording(Time.time);
            _recording = true;
        }


        public void StopRecording()
        {
            if (!_recording)
            {
                Debug.LogWarning("Was not recording.");
                return;
            }
            _recording = false;
        }


        public void StartPlayBack()
        {
            if (_playbackRunner != null)
            {
                Debug.LogWarning("Already in playback");
                return;
            }
            MockScreenInput msi = new MockScreenInput();
            msi.Frames = _recorder.RecordedKeyFrames();
            Debug.LogFormat("Start Playback with {0} frames", msi.Frames.Count);
            _screenInputService.DrivePointers = true;
            _playbackRunner = new ScreenInputMocker(_screenInputService, msi, PlayBackComplete, Time.time);
        }

        private void PlayBackComplete()
        {
            Debug.Log("Playback complete");
            _screenInputService.DrivePointers = false;
            _playbackRunner = null;
        }

    }
}

