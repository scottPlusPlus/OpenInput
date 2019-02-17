using System.Collections.Generic;
using System.Linq;
using GGS.OpenInput.Utils;
using NUnit.Framework;
using UnityEngine;


namespace GGS.OpenInput.Test
{
    public class ScreenInputMockerTest
    {
        [Test]
        public void MockInputRunnerCompletes()
        {
            TestState testState = new TestState(MockInputA());
            testState.RunToEndInEditor();
            Assert.AreEqual(true, testState.Complete);
        }

        [Test]
        public void MockInputDrivesScreenInput()
        {
            TestState testState = new TestState(MockInputA());
            testState.RunToEndInEditor();

            Vector3 state = testState.ScreenInputService.PointerPosition;
            Vector3 data = testState.Data.Frames.Last().Position;
            Assert.AreEqual(true, state.Approximately(data, 0.001f));

            testState = new TestState(MockInputB());
            testState.RunToEndInEditor();

            state = testState.ScreenInputService.PointerPosition;
            data = testState.Data.Frames.Last().Position;
            Assert.AreEqual(true, state.Approximately(data, 0.001f));
        }


        private MockScreenInput MockInputA()
        {
            MockScreenInput mock = new MockScreenInput();
            mock.Frames = new List<MockScreenInput.Frame>();
            mock.Frames.Add(new MockScreenInput.Frame(0f, Vector3.zero, false));
            mock.Frames.Add(new MockScreenInput.Frame(1f, Vector3.one, false));
            return mock;
        }

        private MockScreenInput MockInputB()
        {
            MockScreenInput mock = new MockScreenInput();
            mock.Frames = new List<MockScreenInput.Frame>();
            mock.Frames.Add(new MockScreenInput.Frame(0f, Vector3.zero, false));
            mock.Frames.Add(new MockScreenInput.Frame(1f, Vector3.one, false));
            mock.Frames.Add(new MockScreenInput.Frame(2f, Vector3.one * -1, false));
            mock.Frames.Add(new MockScreenInput.Frame(3f, Vector3.one * 2, false));
            return mock;
        }
    }
}
