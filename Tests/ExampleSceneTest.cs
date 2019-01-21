using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GGS.IInput.Example;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;


namespace GGS.IInput.Test
{
    public class ExampleSceneTest
    {

        [UnityTest]
        //NOTE: This test requires the "ExampleScene" is added to your 'Scenes In Build'
        //Loads the example scene, simulates clicking on each element, checks the click-counts match
        public IEnumerator TestExampleScene()
        {
            SceneManager.LoadScene("ExampleScene");
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            ClickableExample[] clickables = GameObject.FindObjectsOfType<ClickableExample>();
            Assert.AreNotEqual(0, clickables.Length);

            MockScreenInput mockInput = new MockScreenInput();
            mockInput.Frames = new List<MockScreenInput.Frame>();
            mockInput.Frames.Add(new MockScreenInput.Frame(0f, Vector3.zero, false));

            foreach (ClickableExample clickable in clickables)
            {
                ClickCounter counter = clickable.GetComponent<ClickCounter>();
                Assert.AreEqual(0, counter.ClickCount);
                Vector3 pos = Camera.main.WorldToScreenPoint(clickable.transform.position);
                float time = mockInput.Frames.Last().Time + 0.1f;
                mockInput.Frames.Add(new MockScreenInput.Frame(time, pos, false));
                mockInput.Frames.Add(new MockScreenInput.Frame(time + 0.1f, pos, true));
                mockInput.Frames.Add(new MockScreenInput.Frame(time + 0.2f, pos, false));
            }

            Button button = GameObject.FindObjectOfType<Button>();
            Assert.AreNotEqual(null, button);
            float timeB = mockInput.Frames.Last().Time + 0.1f;
            mockInput.Frames.Add(new MockScreenInput.Frame(timeB, button.transform.position, false));
            mockInput.Frames.Add(new MockScreenInput.Frame(timeB + 0.1f, button.transform.position, true));
            mockInput.Frames.Add(new MockScreenInput.Frame(timeB + 0.2f, button.transform.position, false));

            TestState test = new TestState(mockInput);
            yield return test.RunToEndInPlay();

            foreach (ClickableExample clickable in clickables)
            {
                Assert.AreEqual(1, clickable.GetComponent<ClickCounter>().ClickCount);
            }

            Assert.AreEqual(1, button.GetComponent<ClickCounter>().ClickCount);
        }
    }
}