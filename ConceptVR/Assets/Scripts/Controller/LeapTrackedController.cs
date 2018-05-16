using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using Leap.Unity;
using Leap;

#region Structs and Event Handlers
public struct FrameInformation
{
    public bool grabHeld;
    public bool pinchHeld;
    public HandInformation hand;
    public FingerInformation thumb;
    public FingerInformation index;
    public FingerInformation middle;
    public FingerInformation ring;
    public FingerInformation pinky;
}
public struct FingerInformation
{
    public bool isExtended;
    public Vector3 direction;
    public Vector3 padDirection;
    public Vector3 tipPosition;
    public Vector3 tipVelocity;
}
public struct HandInformation
{
    public int velocityChange;
    public Vector3 palmVelocity;
    public Vector3 palmPosition;
    public Vector3 palmNormal;
    public Vector3 direction;
    public float pitch;
    public float yaw;
    public float roll;
    public Quaternion rotation;
}
public enum ThumbStatus
{
    Up = 0,
    Neutral = 1,
    Down = 2
}

public delegate void GestureEventHandler();
public delegate void GesturePositionEventHandler(Vector3 position);
#endregion
public class LeapTrackedController : MonoBehaviour
{
    #region Hand/frame info
    private Leap.Hand hand;
    private HandsUtil util;
    public Vector3 position;
    public string handedness;
    private LeapServiceProvider leapProvider;
    public Queue<FrameInformation> frameQueue;
    public FrameInformation currentFrame;
    #endregion
    #region input booleans
    public bool pinchHeld = false;
    public bool pinchWait = false;
    public int pinchCounter = 5;
    bool flippedPinch = false;
    public bool pinchInput = false;
    public bool gripInput = false;
    bool flippedGrab = false;
    public bool grabHeld = false;
    public bool swipe = false;
    public bool forming = false;
    public bool hudAnchor = true;
    #endregion
    #region cooldowns
    public static float cooldown = 2f;
    public static float tapCooldown = .25f;
    public float swipeCooldownTime;
    public float tapCooldownTime;
    #endregion
    #region Event Handlers
    public event GestureEventHandler pinchMade;
    public event GestureEventHandler pinchGone;
    public event GestureEventHandler dualPinchMade;
    public event GestureEventHandler dualPinchGone;
    public event GestureEventHandler grabMade;
    public event GestureEventHandler grabGone;
    public event GestureEventHandler swipeMade;
    public event GestureEventHandler freeForm;
    public event GestureEventHandler freeFormEnd;
    public event GestureEventHandler freeFormFailure;
    public event GestureEventHandler fireGun;
    public event GesturePositionEventHandler tapMade;
    #endregion
    #region Forming Specific Members
    public PalmDirectionDetector palmDirectorRight;
    public PalmDirectionDetector palmDirectorLeft;
    public PalmDirectionDetector palmDirectorRightUp;
    public PalmDirectionDetector palmDirectorLeftUp;
    public bool rightStart = false;
    public bool leftStart = false;
    public int rFrameCount = 0;
    public int lFrameCount = 0;
    public bool formingEnabled = false;     //IMPORTANT THING THAT I ADDED EXPLICITLY TO MAKE THIS NOT WORK RIGHT NOW - CHRIS
    #endregion
    public int heldFrames = 75;
    public int playerID;
    #region Thumbs Up 
    public ThumbStatus thumbStatusR;
    public ThumbStatus thumbStatusL;
    public DetectorLogicGate thumbUpR;
    public DetectorLogicGate thumbDownR;
    public DetectorLogicGate thumbUpL;
    public DetectorLogicGate thumbDownL;
    #endregion
    #region Pinch Detector
    private PinchDetect rightPinchDetect;
    private PinchDetect leftPinchDetect;
    private DetectorLogicGate dualPinchGate;
    private PinchDetect rightMidPinchDetect;
    private PinchDetect rightRingPinchDetect;
    private PinchDetect rightPinkyPinchDetect;
    #endregion
    #region Teleport Gun Detectors
    public DetectorLogicGate gunReady;
    public DetectorLogicGate gunFire;
    bool cocked = false;
    #endregion
    // Use this for initialization
    void Start()
    {
        util = new HandsUtil();
        frameQueue = new Queue<FrameInformation>();
        currentFrame = new FrameInformation();
        tapCooldownTime = Time.time;
        swipeCooldownTime = Time.time;
        forming = false;
        if (handedness == "Right")
        {
            palmDirectorRight.OnActivate.AddListener(freeFormRight);
            palmDirectorLeft.OnActivate.AddListener(freeFormLeft);
            palmDirectorRightUp.OnActivate.AddListener(freeFormEndRight);
            palmDirectorLeftUp.OnActivate.AddListener(freeFormEndLeft);
            PinchDetectorInitRight();
            gunReady.OnActivate.AddListener(CockGun);
            gunFire.OnActivate.AddListener(FireGun);
        }
    }

    // Update is called once per frame
    void Update()
    {
        removeExtraHands(); //Recent test found that a third hand can enter scene. Gets that outta there
        if (handedness == "Right"){
            hand = Hands.Right;
        }
        else
            hand = Hands.Left;
        if(hand == null){
            OnPinchGone();
        }
        position = util.getIndexPos(hand);
        #region  FreeForm Logic
        //I really hate this logic section.
        //end case
        if (formingEnabled)
        {
            if (pinchWait)
            {
                pinchCounter--;
                if (pinchCounter <= 0)
                    OnPinchGone();
            }
            if (!hudAnchor)
            {
                if (!leftStart && !rightStart && forming && util.checkHandsDist() < .1f)
                {
                    Debug.Log("Ending");
                    forming = false;
                    freeFormEnd();
                    rFrameCount = 0;
                    lFrameCount = 0;
                }
                //continue
                if (forming)
                {
                    return;
                }
                //start
                if (rightStart && leftStart && util.checkHandsDist() < .2f)
                {
                    forming = true;
                    freeForm();
                    rFrameCount = 0;
                    lFrameCount = 0;
                }
            }
        }
        #endregion
        swipe = false;
        #region Pinch and Grab Logic
        /*bool grab = checkGrab();
        if (!grab)
        {
            grab = checkRecentGrabData();
            if (grab)
                flippedGrab = true;
        }
        //begin checking pinch logic
        bool pinch = false;
        swipe = false;
        if (!grab)
        {
            //Debug.Log("Not Grabbing");
            OnGrabGone();
            pinch = checkPinch();
            //if pinch is false, chyeck recent frames to determine if its a leap motion hiccup
            if (!pinch)
            {
                pinch = checkRecentPinchData();
                if (pinch) flippedPinch = true;
            }
        }
        if (!pinch) OnPinchGone();
        //Final grab check
        if (grab && !grabHeld && !flippedGrab)
        {
            Debug.Log("Grabbing");
            OnGrabHeld();
        }
        //Final pinch check
        else if (pinch && !pinchHeld && !flippedPinch)
        {
            OnPinchHeld();
        }*/
        #endregion

        #region Swipe and Tap Logic
        //Begin swipe check
        if (checkSwipe())
        {
            //We have a cooldown so swipes cannot be spammed
            if (Time.time > swipeCooldownTime)
            {
                swipe = true;
                swipeMade();
                swipeCooldownTime = Time.time + cooldown;
            }
            else
                Debug.Log("Cooldown!!");
        }
        if (checkTap() && !swipe)
        {
            if (Time.time > tapCooldownTime)
            {
                tapMade(frameQueue.ToArray()[frameQueue.Count - 2].index.tipPosition);
                tapCooldownTime = Time.time + tapCooldown;
            }
        }
        #endregion

        #region Additional Pinch and Grab logic
        if (flippedPinch)
        {
            pinchHeld = false;
            flippedPinch = false;
            pinchInput = true;
        }
        else
            pinchInput = false;
        if (flippedGrab)
        {
            grabHeld = false;
            flippedPinch = false;
            gripInput = true;
        }
        else
            gripInput = false;
        #endregion
        if (hand != null)
        {
            currentFrame = fetchFrameInformation();
            frameQueue.Enqueue(currentFrame);
            if (frameQueue.Count > heldFrames)
                frameQueue.Dequeue();
        }

    }//end Update()

    #region Pinch and Grab Event Handlers
    public void OnPinchHeld()
    {
        pinchCounter = 5;
        position = util.weightedPos(hand);
        pinchHeld = true;
        pinchMade();
    }
    public void PinchWaiter(){
        pinchWait = true;
    }
    public void OnPinchGone()
    { 
        pinchWait = false;
        pinchHeld = false;
        pinchGone();
    }
    public void OnDualPinch()
    {
        dualPinchMade();
    }
    public void OffDualPinch()
    {
        dualPinchGone();
    }
    #endregion

    #region Gesture Check Funtions
    public bool checkPinch()
    {
        //Determine which hand.
        if (handedness == "Right")
            hand = Hands.Right;
        else
            hand = Hands.Left;
        //Check if hand exists
        if (hand == null)
            return false;
        //Get position and value of Pinch
        bool detected = util.IsPinching(hand);
        if (detected)
            position = util.weightedPos(hand);
        return detected;
    }
    public bool checkGrab()
    {
        if (handedness == "Right")
            hand = Hands.Right;
        else
            hand = Hands.Left;
        if (hand == null)
            return false;
        position = util.getIndexPos(hand);
        return util.IsGrabbing(hand);

    }
    //in progress
    public bool checkThumbsUp()
    {
        if (handedness == "Right")
            hand = Hands.Right;
        else
            hand = Hands.Left;
        if (hand == null)
            return false;
        position = util.getThumbPos(hand);
        return util.checkThumbsUp(hand);
    }

    public bool checkTap()
    {
        if (handedness == "Right")
            hand = Hands.Right;
        else
            hand = Hands.Left;
        if (hand == null)
            return false;
        position = util.getIndexPos(hand);
        bool ret = util.checkTap(frameQueue, hand);
        return ret;
    }
    public bool checkSwipe()
    {
        if (handedness == "Right")
            hand = Hands.Right;
        else
            hand = Hands.Left;
        if (hand == null)
            return false;
        position = util.getIndexPos(hand);
        return util.isSwiping(hand, frameQueue);
    }
    #endregion

    #region Frame Info and Recent Frame checks
    FrameInformation fetchFrameInformation()
    {
        FrameInformation frameInfo = new FrameInformation();
        frameInfo.grabHeld = grabHeld;
        frameInfo.pinchHeld = pinchHeld;
        foreach (Leap.Finger f in hand.Fingers)
        {
            FingerInformation fingerInfo = new FingerInformation();
            fingerInfo.isExtended = f.IsExtended;
            fingerInfo.direction = f.Direction.ToVector3();
            fingerInfo.padDirection = Vector3.Cross(fingerInfo.direction, Vector3.Cross(fingerInfo.direction, f.bones[1].Direction.ToVector3()));
            fingerInfo.tipPosition = f.TipPosition.ToVector3();
            fingerInfo.tipVelocity = f.TipVelocity.ToVector3();
            switch (f.Type)
            {
                case Leap.Finger.FingerType.TYPE_INDEX:
                    frameInfo.index = fingerInfo;
                    break;
                case Leap.Finger.FingerType.TYPE_MIDDLE:
                    frameInfo.middle = fingerInfo;
                    break;
                case Leap.Finger.FingerType.TYPE_RING:
                    frameInfo.ring = fingerInfo;
                    break;
                case Leap.Finger.FingerType.TYPE_PINKY:
                    frameInfo.pinky = fingerInfo;
                    break;
                case Leap.Finger.FingerType.TYPE_THUMB:
                    frameInfo.thumb = fingerInfo;
                    break;
            }
        }
        if (handedness == "Right")
            hand = Hands.Right;
        else
            hand = Hands.Left;
        HandInformation handInfo = new HandInformation();
        handInfo.direction = hand.Direction.ToVector3();
        handInfo.pitch = hand.Direction.Pitch;
        handInfo.roll = hand.Direction.Roll;
        handInfo.yaw = hand.Direction.Yaw;
        handInfo.palmPosition = hand.PalmPosition.ToVector3();
        handInfo.palmVelocity = hand.PalmVelocity.ToVector3();
        handInfo.palmNormal = hand.PalmNormal.ToVector3();
        handInfo.rotation = hand.Rotation.ToQuaternion();
        frameInfo.hand = handInfo;
        return frameInfo;

    }
    public bool checkRecentPinchData()
    {
        if (frameQueue.Count < 50) return false;
        FrameInformation[] frames = frameQueue.ToArray();
        for (int i = frames.Length - 1; i > frames.Length - 5; --i)
        {
            if (frames[i].pinchHeld)
                return true;
        }
        return false;
    }
    public bool checkRecentGrabData()
    {
        if (frameQueue.Count < 50) return false;
        FrameInformation[] frames = frameQueue.ToArray();
        for (int i = frames.Length - 1; i > frames.Length - 5; --i)
        {
            if (frames[i].grabHeld)
                return true;
        }
        return false;
    }
    #endregion
    public void removeExtraHands()
    {
        GameObject go = GameObject.Find("RigidRoundHand_L(Clone)");
        if (go != null) GameObject.Destroy(go);
        go = GameObject.Find("RigidRoundHand_R(Clone)");
        if (go != null) GameObject.Destroy(go);
        go = GameObject.Find("LoPoly_Rigged_Hand_Left(Clone)");
        if (go != null) GameObject.Destroy(go);
        go = GameObject.Find("LoPoly_Rigged_Hand_Right(Clone)");
        if (go != null) GameObject.Destroy(go);
    }
    #region FreeForm Event Handlers
    public void freeFormRight()
    {
        Debug.Log("Right Hand Start");
        rightStart = true;
    }
    public void freeFormLeft()
    {
        Debug.Log("Left Hand Start");
        leftStart = true;
    }

    
    public void freeFormEndRight()
    {
        rightStart = false;
    }
    public void freeFormEndLeft()
    {
        leftStart = false;
    }

    public void freeFormFailureHandler()
    {
        forming = false;
        freeFormFailure();
    }
    #endregion

    #region Thumbs Event Handlers

    #endregion

    #region Gun Event Handlers
    public void CockGun()
    {
        cocked = true;
    }
    public void FireGun()
    {
        cocked = false;
        fireGun();
    }
    #endregion

    public void PinchDetectorInitRight()
    {
        rightPinchDetect = GameObject.Find("Detectors").transform.Find("PinchDetector(R)").GetComponent<PinchDetect>();
        rightPinchDetect.OnActivate.AddListener(OnPinchHeld);
        rightPinchDetect.OnDeactivate.AddListener(OnPinchGone);
        leftPinchDetect = GameObject.Find("Detectors").transform.Find("PinchDetector(L)").GetComponent<PinchDetect>();
        leftPinchDetect.OnActivate.AddListener(OnDualPinch);
        leftPinchDetect.OnDeactivate.AddListener(OffDualPinch);
        /*dualPinchGate = GameObject.Find("Detectors").transform.Find("DualPinchGate").GetComponent<DetectorLogicGate>();
        dualPinchGate.OnActivate.AddListener(OnDualPinch);
        dualPinchGate.OnDeactivate.AddListener(OffDualPinch);*/
    }
}

