using System.Collections.Generic;
using UnityEngine;


namespace GGS.OpenInput
{
    [System.Serializable]
    public class MockScreenInput
    {
        [System.Serializable]
        public struct Frame
        {
            public float Time;
            public Vector2 Position;
            public bool Clicking;

            public Frame(float time, Vector2 pos, bool clicking)
            {
                this.Time = time;
                this.Position = pos;
                this.Clicking = clicking;
            }

        }

        public List<Frame> Frames;

        public MockScreenInput Clone()
        {
            MockScreenInput clone = new MockScreenInput();
            clone.Frames = new List<Frame>(Frames);
            return clone;
        }
    }
}

