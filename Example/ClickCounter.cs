using UnityEngine;
using UnityEngine.UI;


namespace GGS.OpenInput.Example
{
    public class ClickCounter : MonoBehaviour
    {

        public int ClickCount { get; private set; }

        void Awake()
        {
            ClickableExample clickable = GetComponent<ClickableExample>();
            if (clickable != null)
            {
                clickable.Clicked += Click;
            }

            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(Click);
            }
        }

        // Use this for initialization
        public void Click()
        {
            ClickCount++;
        }

    }
}