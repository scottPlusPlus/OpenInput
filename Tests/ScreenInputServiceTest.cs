using System.Collections;
using System.Collections.Generic;
using GGS.OpenInput.Example;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


namespace GGS.OpenInput.Test
{
    public class ScreenInputServiceTest
    {

        [UnityTest]
        public IEnumerator ClicksOnClickableExamples()
        {
            Debug.Log("UnityTest: ClickOnClickableExamples");
            Camera camera = new GameObject().AddComponent<Camera>();
            camera.transform.position = new Vector3(0f, 1f, -10f);

            var go = new GameObject();
            go.AddComponent<BoxCollider>();
            ClickableExample clickable = go.AddComponent<ClickableExample>();
            clickable.transform.position = camera.ScreenToWorldPoint(Vector3.one);
            ClickCounter counter = clickable.gameObject.AddComponent<ClickCounter>();
            Assert.AreEqual(0, counter.ClickCount);

            yield return new WaitForEndOfFrame();

            MockScreenInput mockInput = new MockScreenInput();
            mockInput.Frames = new List<MockScreenInput.Frame>();
            mockInput.Frames.Add(new MockScreenInput.Frame(0f, Vector3.zero, false));
            mockInput.Frames.Add(new MockScreenInput.Frame(0.1f, Vector3.one, false));
            mockInput.Frames.Add(new MockScreenInput.Frame(0.2f, Vector3.one, true));
            mockInput.Frames.Add(new MockScreenInput.Frame(0.3f, Vector3.one, false));

            TestState test = new TestState(mockInput);
            yield return test.RunToEndInPlay();

            Assert.AreEqual(1, counter.ClickCount);

            //cleanup
            GameObject.Destroy(camera.gameObject);
            GameObject.Destroy(clickable.gameObject);
        }

    }
}