using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyType { TEXT, NETINPUT, INPUT };
public class PlaceTextButton : HUDButton {

    public GameObject textBox;
    public KeyType type = KeyType.TEXT;
    bool submit = true;
    string addr;
    NetworkStarter netStarter;
    
	// Use this for initialization
	void Start () {
        netStarter = GameObject.Find("NetworkStarter").GetComponent<NetworkStarter>();
    }
	
	// Update is called once per frame
	void Update () {
        TextMesh btnText = this.gameObject.transform.Find("Text").GetComponent<TextMesh>();
        if (type == KeyType.TEXT)
            btnText.text = "Place\nText";
        else if (type == KeyType.NETINPUT)
            if (submit)
                btnText.text = "Next";
            else
                btnText.text = "Submit";

	}

    public override void OnPress()
    {
        if (type == KeyType.TEXT)
            TextSpawn();
        else if (type == KeyType.NETINPUT)
            NetworkConnect();

    }
    public void TextSpawn()
    {
        if (textBox.GetComponent<TextMesh>().text != null)
        {
            // remove red underscore before generating the text
            string textBoxText = textBox.GetComponent<TextMesh>().text;
            textBoxText = textBoxText.Substring(0, (textBoxText.Length - 20));
            textBox.GetComponent<TextMesh>().text = textBoxText;

            GameObject go = Instantiate(ItemBase.itemBase.TextPrefab, textBox.transform.position, textBox.transform.rotation);
            go.GetComponent<TextItem>().updateText(textBox.GetComponent<TextMesh>().text);
            go.transform.localScale = textBox.transform.localScale * 1.25f;
            ItemBase.itemBase.Add(go.GetComponent<TextItem>());
            // clear the current text in the textbox
            textBox.GetComponent<TextMesh>().text = "<color=red>_</color>";
        }
    }
    public void NetworkConnect()
    {
        string textBoxText = textBox.GetComponent<TextMesh>().text;
        textBoxText = textBoxText.Substring(0, (textBoxText.Length - 20));
        if (submit)
        {
            netStarter.setNetAddress(textBoxText);
            textBox.GetComponent<TextMesh>().text = "<color=red>_</color>";
        }
        else
        {
            netStarter.setPort(textBoxText);
            netStarter.connectToHost();
            textBox.GetComponent<TextMesh>().text = "<color=red>_</color>";
        }
        submit = !submit;


    }


}
