using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardButton : HUDButton {

    public string text;

	// Use this for initialization
	void Start () {
        base.Start();
	}
    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void OnPress()
    {
        // remove current red underscore marker before editing the text
        string textBoxText = transform.parent.Find("Textbox").GetComponent<TextMesh>().text;
        textBoxText = textBoxText.Substring(0, (textBoxText.Length - 20));
        transform.parent.Find("Textbox").GetComponent<TextMesh>().text = textBoxText;
        string redUnderscore = "<color=red>_</color>";

        if (text == "backspace")
        {
            if (textBoxText != "")
            {
                Debug.Log(textBoxText + " is text");
                // remove last character from string
                textBoxText = textBoxText.Substring(0, (textBoxText.Length - 1));
                transform.parent.Find("Textbox").GetComponent<TextMesh>().text = textBoxText;
            }
        }else if (text == "enter")
        {
            transform.parent.Find("Textbox").GetComponent<TextMesh>().text += '\n';
        }
        else
        {
            transform.parent.Find("Textbox").GetComponent<TextMesh>().text += text;
        }

        // re-add the red underscore market to the text after editing it.
        transform.parent.Find("Textbox").GetComponent<TextMesh>().text += redUnderscore;
        base.OnPress();
    }

}
