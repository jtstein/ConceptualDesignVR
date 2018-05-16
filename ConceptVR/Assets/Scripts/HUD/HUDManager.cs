using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

public class HUDManager : MonoBehaviour
{
    // active or inactive hud
    bool hudActive = false;


    // current tool selected
    string toolSelected = "";

    Color HUDColor;
    public Stack<HUDFrame> frameStack;
    List<HUDView> viewList;
    HandsUtil util;
    [HideInInspector] public GameObject frames;
    [HideInInspector] public HUDFrame mainFrame;
    Leap.Controller leapcontroller;
    Leap.Frame frame;
    Leap.Hand lHand;
    public GameObject LeapHandController;
    public GameObject HUDObject;
    public AnchorButton anchor;
    bool placed;

    public static HUDManager hudManager;

    // Use this for initialization
    void Start()
    {

        leapcontroller = new Leap.Controller();
        frame = leapcontroller.Frame();

        util = new HandsUtil();

        frames = HUDObject.transform.Find("Frames").gameObject;
        mainFrame = frames.transform.Find("MainFrame").GetComponent<HUDFrame>();

        // create the frameStack and initialize it with the mainFrame
        frameStack = new Stack<HUDFrame>();
        frameStack.Push(mainFrame);

        hudActive = true;

        // initialize HUDColor to gray
        HUDColor = new Color(0.345f, 0.3568f, 0.3804f, 1.0f);
        anchor.Anchor += Placement;
        placed = false;

        hudManager = this;


    }

    // Update is called once per frame
    void Update()
    {
        if (placed)
            return;
        frame = leapcontroller.Frame();
        lHand = Hands.Left;
        // if there are hands visible in the view.
        if (lHand != null)
        {
            if (util.IsFlatHand(lHand))
                HUDObject.SetActive(true);
            else
            {
                HUDObject.SetActive(false);
            }
            
        }
        else
        {
            // if no hands are visible in the view, set the HUD inactive
            HUDObject.SetActive(false);
        }
    }

    public void Push(HUDFrame hudframe) {

        // Super hackey solution to minor problem.
        if (this.frameStack.Peek().gameObject.name == "MaterialsFrame" && hudframe.gameObject.name == "DoodleFrame" ||
            this.frameStack.Peek().gameObject.name == "DoodleFrame" && hudframe.gameObject.name == "MaterialsFrame")
        {
            this.Pop();
        }

        if (this.frameStack.Count >= 4)
        {
            this.Pop();
        }
        // deactivate current top level HUDframe
        if (!hudframe.isSubFrame)
        {
            this.frameStack.Peek().transform.gameObject.SetActive(false);
        }

        // activate new top level HUDframe
        hudframe.gameObject.SetActive(true);
        updateColor(hudframe);

        this.frameStack.Push(hudframe);
    }


    public void Pop() {

        HUDFrame removedFrame = this.frameStack.Peek();
        /*foreach (SwapToolButton b in removedFrame.gameObject.GetComponentsInChildren<SwapToolButton>())
            if (b.toggled && b != this)
            {
                b.OnPress();
            }*/
        // never pop the mainFrame
        if (removedFrame != mainFrame)
        {
            // deactivate top level HUDFrame
            removedFrame.gameObject.SetActive(false);
            // pop top level HUDFrame from stack
            this.frameStack.Pop();
            // activate new top level HUDframe.
            this.frameStack.Peek().transform.gameObject.SetActive(true);
            updateColor(this.frameStack.Peek());
        }
    }

    public void popAll()
    {
        while (this.frameStack.Peek() != mainFrame)
        {
            this.Pop();
        }
    }

    void updateColor(HUDFrame topFrame) {

        foreach (Transform child in topFrame.GetComponentsInChildren<Transform>()) {
            bool isFrameObj = false;
            bool isButtonObj = false;
            bool isTextObj = false;

        }
    }

    public Color getHUDColor() {
        return HUDColor;
    }

    public void setHUDColor(Color color) {
        this.HUDColor = color;

        updateColor(this.frameStack.Peek());
    }
    

    void Placement()
    {
        LeapTrackedController ltc = GameObject.Find("LoPoly_Rigged_Hand_Right").GetComponent<LeapTrackedController>();
        if (placed) {
            GameObject lHand = GameObject.Find("RigidRoundHand_L");
            if (lHand != null)
            {
                GameObject parent = lHand.transform.Find("palm").gameObject;
                GameObject HUD = GameObject.Find("HandsUpDisplay");
                HUD.transform.SetParent(parent.transform);
                HUD.transform.localPosition = new Vector3(0, 0, 0);
                HUD.transform.localRotation = new Quaternion(180, 0, 0, 0);
                placed = false;
                HUDObject.SetActive(false);
                ltc.hudAnchor = true;
                this.gameObject.GetComponent<SettingsManager>().anchored = true;
            }
            else
            {
                //TODO: signal to the player they need to have their left hand visible
            }
        }
        else {
            GameObject.Find("HandsUpDisplay").transform.parent = null;
            placed = true;
            HUDObject.SetActive(true);
            ltc.hudAnchor = false;
            this.gameObject.GetComponent<SettingsManager>().anchored = false;
        }
    }
}
