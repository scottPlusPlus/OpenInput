using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


namespace GGS.OpenInput.Test
{
    public class InputServiceTest
    {

        [UnityTest]
        public IEnumerator InputService_ClicksOnClickable_CountGoesUp()
        {
            Camera camera = new GameObject().AddComponent<Camera>();
            camera.transform.position = new Vector3(0f, 1f, -10f);

            GameObject go = new GameObject();
            go.AddComponent<BoxCollider>();
            TestClickable clickable = go.AddComponent<TestClickable>();
            clickable.transform.position = camera.ScreenToWorldPoint(Vector3.one);

            TestCounter counter = new TestCounter();
            clickable.Clicked += counter.Increment;

            Assert.AreEqual(0, counter.Count);

            //wait for Unity to fully create and init the objects
            yield return new WaitForEndOfFrame();

            MockScreenInput mockInput = new MockScreenInput();
            mockInput.Frames = new List<MockScreenInput.Frame>();
            mockInput.Frames.Add(new MockScreenInput.Frame(0f, Vector3.zero, false));
            mockInput.Frames.Add(new MockScreenInput.Frame(0.1f, Vector3.one, false));
            mockInput.Frames.Add(new MockScreenInput.Frame(0.2f, Vector3.one, true));
            mockInput.Frames.Add(new MockScreenInput.Frame(0.3f, Vector3.one, false));

            TestHelper test = new TestHelper(mockInput);
            yield return test.RunToEndInPlay();

            Assert.AreEqual(1, counter.Count);

            //cleanup
            GameObject.Destroy(camera.gameObject);
            GameObject.Destroy(go);
        }

    }
}