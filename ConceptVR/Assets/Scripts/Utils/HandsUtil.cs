using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
public class HandsUtil {
    private static float dist = .025f;
    //public DebugGrapher grapher = GameObject.Find("HandUtilDebugGrapher").GetComponent<DebugGrapher>();

    //Gets the position of the fingers on a hand
    public Vector3 getIndexPos(Leap.Hand hand)
    {
        return Hands.GetIndex(hand).TipPosition.ToVector3();
    }
    public Vector3 getMiddlePos(Leap.Hand hand)
    {
        return Hands.GetMiddle(hand).TipPosition.ToVector3();
    }
    public Vector3 getRingPos(Leap.Hand hand)
    {
        return Hands.GetRing(hand).TipPosition.ToVector3();
    }
    public Vector3 getPinkyPos(Leap.Hand hand)
    {
        return Hands.GetPinky(hand).TipPosition.ToVector3();
    }
    public Vector3 getThumbPos(Leap.Hand hand)
    {
        return Hands.GetThumb(hand).TipPosition.ToVector3();
    }

    //Check pinch and grab pose
    public bool IsPinching(Leap.Hand hand)
    {
        if (Hands.GetIndex(hand).IsExtended)
            return false;
        //return hand.PinchStrength > .9f;
        //first and foremost we need the index to be pinched, if it it is we have a true
        bool pinch = checkPinchOfFinger(hand, "index");
        //if we are in pinch with index check to see if other fingers are not.
        if (pinch)
        {
            string[] fings = { "ring", "pinky" };
            foreach(string f in fings)
            {
                //If any other finger is in pinch return false
                if (checkPinchOfFinger(hand, f))
                    return false;
            }
        }
        return pinch;
    }
    public bool IsGrabbing(Leap.Hand hand)
    {
        //return hand.GrabStrength > .9f;
        foreach(Leap.Finger f in hand.Fingers)
        {
            if (f.Type == Leap.Finger.FingerType.TYPE_THUMB)
                continue;
            if (!checkFingerGrip(f))
                return false;
        }
        return true;
    }
    //Determines if the fingers are extended and the palm is facing up
    public bool IsFlatHand(Leap.Hand hand)
    {
        float angle = Vector3.Angle(hand.PalmNormal.ToVector3(), Vector3.up);
        //Debug.Log(angle);
        return (hand.GrabAngle <= 1f) && (angle<120f);
    }
    //Depricated checks for a specific grab angle
    public bool IsGrabbingAngle(Leap.Hand hand)
    {
        return hand.GrabAngle > Mathf.PI * 2f / 3f;
    }
    //returns a true if a finger is pinching with the thumb, false if not
    public bool checkPinchOfFinger(Leap.Hand hand, string finger)
    {
        switch (finger)
        {
            case "index":
                if (getPinchDistance(hand.GetIndex(), hand.GetThumb()) < dist)
                    return true;
                break;
            case "middle":
                if (getPinchDistance(hand.GetMiddle(), hand.GetThumb()) < dist)
                    return true;
                break;
            case "ring":
                if (getPinchDistance(hand.GetRing(), hand.GetThumb()) < dist)
                    return true;
                break;
            case "pinky":
                if (getPinchDistance(hand.GetPinky(), hand.GetThumb()) < dist)
                    return true;
                break;
            default:
                break;
        }
        return false;
    }
    //Gets the distance between the thumb and a finger. Has leapmotion leniency
    public float getPinchDistance(Leap.Finger finger, Leap.Finger thumb)
    {
        // Gets the distance between two fingers, determines if they are close in any direction.
        Vector3 diff = thumb.TipPosition.ToVector3() - finger.TipPosition.ToVector3();
        float dist = diff.magnitude;
        Vector3 padDirection = Vector3.Cross(finger.Direction.ToVector3(), Vector3.Cross(finger.Direction.ToVector3(), finger.bones[1].Direction.ToVector3()));
        return dist * Mathf.Sign(Vector3.Dot(diff, padDirection));
        //return dist;
    }
    //Functions to recieve bone position of a finger
    public Vector3 getMetacarpalPosition(Leap.Finger finger)
    {
        return finger.Bone(Leap.Bone.BoneType.TYPE_METACARPAL).Center.ToVector3();
    }
    public Vector3 getProximalPosition(Leap.Finger finger)
    {
        return finger.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Center.ToVector3();
    }
    public Vector3 getIntermediatePosition(Leap.Finger finger)
    {
        return finger.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).Center.ToVector3();
    }
    public Vector3 getDistalPosition(Leap.Finger finger)
    {
        return finger.Bone(Leap.Bone.BoneType.TYPE_DISTAL).Center.ToVector3();
    }
    //Checks the grip angle of a finger to see if it is part of a grab pose
    public bool checkFingerGrip(Leap.Finger finger)
    {
        Vector3 distal = getDistalPosition(finger);
        Vector3 proximal = getProximalPosition(finger);
        Vector3 intermediate = getIntermediatePosition(finger);
        Vector3 metacarpal = getMetacarpalPosition(finger);
        //Fetch the angle that is created between bones.
        float mpAngle = 180 - Vector3.Angle(proximal-metacarpal, intermediate-proximal);
        float pdAngle = 180 - Vector3.Angle(intermediate-proximal, distal-intermediate);
        //If a proper angle return tru.
        if (mpAngle < 120f && pdAngle < 120f)
            return true;
        return false;
    }
    //Needs tested. Checks for a thumbs up pose.
    public bool checkThumbsUp(Leap.Hand hand)
    {
        foreach(Leap.Finger f in hand.Fingers)
        {
            if (f.Type == Leap.Finger.FingerType.TYPE_THUMB)
            {
                if (f.IsExtended && checkDirectionUp(f))
                    continue;
                else return false;
            }
            else
            {
                if (f.IsExtended && checkDirectionUp(f))
                    return false;
                else continue;
            }
        }
        return true;
    }
    //next two untested
    bool checkDirectionUp(Leap.Finger finger)
    {
        Vector3 direction = finger.Direction.ToVector3();
        return (direction.x > .95f && direction.y > .95f && direction.z > .95f);
    }
    public bool checkFingerGun(Leap.Hand hand)
    {
        if (!checkThumbsUp(hand))
            return false;
        if (!Hands.GetIndex(hand).IsExtended)
            return false;
        return true;
    }
    //Checks for a tap gesture, uses previous frame information.
    public bool checkTap(Queue<FrameInformation> frameQueue, Leap.Hand hand)
    {
        //Do not want ring or pinky extended when tapping.
        if (Hands.GetRing(hand).IsExtended || Hands.GetPinky(hand).IsExtended)
            return false;
            int count = frameQueue.Count;
        if (count < 11)
            return false;

        FrameInformation[] fArr = frameQueue.ToArray();
        float[] sharpness = new float[count];
        float[] padvel = new float[count];
        float[] accelMag = new float[count];
        for (int i = 1; i < count-1; ++i)
        {
            FingerInformation p = fArr[i - 1].index;
            FingerInformation n = fArr[i + 1].index;
            FingerInformation v = fArr[i].index;
            //Vector3 jerk = (n.tipVelocity - v.tipVelocity) - (v.tipVelocity - p.tipVelocity);
            //jerk = Vector3.Project(jerk, fArr[i].index.padDirection);

            //Fetch magnitude of acceleration of recent frames as well as pad velocity and sharpness
            Vector3 accel = (n.tipVelocity - p.tipVelocity);
            Vector3.Project(accel, fArr[i].index.padDirection);
            accelMag[i] = Vector3.Project(accel, fArr[i].index.padDirection).magnitude;
            accelMag[i] *= Mathf.Sign(Vector3.Dot(accel, fArr[i].index.padDirection));
            padvel[i] = Vector3.Project(v.tipVelocity, fArr[i].index.padDirection).magnitude;
            padvel[i] *= Mathf.Sign(Vector3.Dot(v.tipVelocity, fArr[i].index.padDirection));
            sharpness[i] = Vector3.Angle(v.tipPosition - p.tipPosition, n.tipPosition - v.tipPosition) * accel.sqrMagnitude;
        }

        /*if (sharpness[count - 2] > 20f && sharpness[count - 1] < sharpness[count - 2] && sharpness[count - 3] < sharpness[count - 2]
            && fArr[count-2].index.isExtended
            && accelMag[count-2] > 0)*/
        //Checks to see if a tap existed in this frame.
        if (accelMag[count-2] < -.5f && accelMag[count - 3] > accelMag[count - 2] && accelMag[count - 1] > accelMag[count - 2])
            return true;
        else
            return false;
    }
    //Checks to see if a swipe has occured recently.
    //Uses recent frame information and the users hand
    public bool isSwiping(Leap.Hand hand, Queue<FrameInformation> framesQueue)
    {
        if (framesQueue.Count < 50) return false;
        FrameInformation[] frames = framesQueue.ToArray();
        List<float> accelMag = new List<float>();
        //if not enough fingers extended no swipe
        if (Extended(hand.Fingers) >= 4)
        {
            //Fetch the accel magnitude of recent and current frames
            Vector3 accel = (hand.PalmVelocity.ToVector3() - frames[frames.Length - 2].hand.palmVelocity);
            accelMag.Add(Vector3.Project(accel, hand.PalmNormal.ToVector3()).magnitude);
            accelMag[0] *= Mathf.Sign(Vector3.Dot(accel, hand.PalmNormal.ToVector3()));
            for (int i = frames.Length-1; i > frames.Length-7; --i)
            {
                accel = (frames[i].hand.palmVelocity - frames[frames.Length - 2].hand.palmVelocity);
                accelMag.Add(Vector3.Project(accel, frames[i].hand.palmNormal).magnitude);
                accelMag[accelMag.Count-1] *= Mathf.Sign(Vector3.Dot(accel, frames[i].hand.palmNormal));
            }
            //Grab the roll of the hand
            float handRoll = Mathf.Abs(hand.PalmNormal.Roll);
            return (handRoll > 1.35 && accelMag[1] < -0.3f && accelMag[0] < accelMag[1] && accelMag[2] > accelMag[1]) ? true : false;
        }
        return false;
    }
    //Returns how may fingers are extended on a hand
    public int Extended(List<Leap.Finger> fingers)
    {
        int count = 0;
        foreach (Leap.Finger f in fingers)
            if (f.IsExtended)
                count++;
        return count;
    }
    //returns the right or left hand when asked for!
    public Leap.Hand getHandByHandedness(string handedness)
    {
        if (handedness == "Right")
            return Hands.Right;
        else if (handedness == "Left")
            return Hands.Left;
        else
            return null;
    }

    public Vector3 weightedPos(Leap.Hand hand)
    {
        return getThumbPos(hand) * 75f + getIndexPos(hand) * .25f;
    }
    public int getHandCount()
    {
        Leap.Frame frame = new LeapServiceProvider().CurrentFrame;
        return frame.Hands.Count;
    }
    public bool checkFreeForm()
    {
        Leap.Hand left = Hands.Left;
        Leap.Hand right = Hands.Right;

        if (left == null || right == null)
            return false;

        //if (!(Extended(left.Fingers) >= 3 && Extended(right.Fingers) >= 3))
          //  return false;
        if (Vector3.Distance(left.PalmPosition.ToVector3(), right.PalmPosition.ToVector3()) > .25f)
            return false;
        if (left.PalmNormal.ToVector3().y < .9f && right.PalmNormal.ToVector3().y < .9f)
            return false;
        return true;
    }
    public bool checkEndFreeForm()
    {
        Leap.Hand left = Hands.Left;
        Leap.Hand right = Hands.Right;
        if (left == null || right == null)
            return false;

        //if (!(Extended(left.Fingers) >= 3 && Extended(right.Fingers) >= 3))
          //  return false;
        if (Vector3.Distance(left.PalmPosition.ToVector3(), right.PalmPosition.ToVector3()) > .25f)
            return false;
        if (left.PalmNormal.ToVector3().y > .9f && right.PalmNormal.ToVector3().y > .9f)
            return false;
        return true;
    }
    public float checkHandsDist()
    {
        return Vector3.Distance(Hands.Left.PalmPosition.ToVector3(), Hands.Right.PalmPosition.ToVector3());
    }
}
