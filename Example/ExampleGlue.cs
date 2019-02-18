using UnityEngine;


namespace GGS.OpenInput.Example
{
    public class ExampleGlue : MonoBehaviour
    {
        [SerializeField] private UnityInputWrapper _unityInputWrapper;
        [SerializeField] private KeyCode _toggleRecordKey;
        [SerializeField] private KeyCode _playbackKey;

        private InputService _screenInputService = new InputService();

        private bool _recording;
        private bool _playing;
        private InputRecorder _recorder = new InputRecorder();
        private MockInputDriver _playbackRunner = new MockInputDriver();


        // Use this for initialization
        void Start()
        {
            _screenInputService.SetDriver(_unityInputWrapper, false);
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
            if (_playing)
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
            _recorder.StartRecording(_unityInputWrapper, Time.time);
            _recording = true;
        }


        public void StopRecording()
        {
            if (!_recording)
            {
                Debug.LogWarning("Was not recording.");
                return;
            }
            _recorder.StopRecording();
            _recording = false;
        }


        public void StartPlayBack()
        {
            if (_playing)
            {
                Debug.LogWarning("Already in playback");
                return;
            }
            _playing = true;
            MockScreenInput data = new MockScreenInput();
            data.Frames = _recorder.RecordedKeyFrames();
            Debug.LogFormat("Start Playback with {0} frames", data.Frames.Count);
            _screenInputService.SetDriver(_playbackRunner, true);
            _playbackRunner.PlayInput(data, PlayBackComplete, Time.time);
        }


        private void PlayBackComplete()
        {
            Debug.Log("Playback complete");
            _screenInputService.SetDriver(_unityInputWrapper, false);
            _playing = false;
        }
        

    }
}

