using System.Collections;
using System.Collections.Generic;
using GGS.OpenInput.Example;
using UnityEngine;
using UnityEngine.UI;

public class ClickCounter : MonoBehaviour {

    public int ClickCount { get; private set; }
    public ClickableExample ex;

    void Awake()
    {
        ClickableExample clickable = GetComponent<ClickableExample>();    
        if (clickable != null)
        {
            ex = clickable;
            clickable.Clicked += Click;
        }

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(Click);
        }
    }

    // Use this for initialization
    public void Click ()
    {
        ClickCount++;
        Debug.Log("ClickCount == " + ClickCount);
    }
	
}
