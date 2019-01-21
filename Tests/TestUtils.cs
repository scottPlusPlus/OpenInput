using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGS.IInput.Test
{
    public class TestState
    {
        public ScreenInputService ScreenInputService;
        public ScreenInputMocker MockInputRunner;
        public MockScreenInput Input;
        public bool Complete;

        public void MarkComplete()
        {
            Complete = true;
        }

        public float EndTime { get { return Input.Frames.Last().Time; } }

        public void RunToEndInEditor(int fps = 30)
        {
            float timePerTick = 1f / fps;
            float time = 0f;
            while (time <= EndTime + timePerTick)
            {
                MockInputRunner.Update(time);
                time += timePerTick;
            }
        }

        public IEnumerator RunToEndInPlay()
        {
            while(!Complete)
            {
                MockInputRunner.Update(Time.time);
                yield return new WaitForEndOfFrame();
            }
        }

        public TestState (MockScreenInput input)
        {
            ScreenInputService = new ScreenInputService();
            ScreenInputService.DrivePointers = true;
            Input = input;
            MockInputRunner = new ScreenInputMocker(ScreenInputService, input, MarkComplete, 0f);
        }
    }
}