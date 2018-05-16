using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SettingsManager : MonoBehaviour {
    public static SettingsManager sm;

    const float SCALE_FACTOR = 2.75f;

    public float gridSnap = 1f/64f;
    public float rotationSnap = 15f;
    public bool snapEnabled = false;
    public GameObject scaleSlider;
    [HideInInspector]
    public float playerScale = 1f;
    public bool anchored = true;

    // analog or digital clock
    bool isAnalogClock = true;
    public GameObject clock;

    // tutorial video player
    [HideInInspector]
    public bool tutorialMode = false;
    public GameObject tutorialVideoPlayer;

    GameObject LMHeadMountedRig;
    public GameObject hand;
    handController controller;

    
    void Start () {
        sm = this;

        LMHeadMountedRig = GameObject.Find("LMHeadMountedRig");
        playerScale = 1f;
        isAnalogClock = true;
        tutorialMode = false;
        controller = hand.GetComponent<handController>();
    }

    void Update () {
        if (anchored)
        {
            playerScale = scaleSlider.GetComponent<HUDSlider>().value * SCALE_FACTOR + 1f;
            LMHeadMountedRig.transform.localScale = new Vector3(playerScale, playerScale, playerScale);
        }
    }

    public Vector3 snapToGrid(Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x / gridSnap) * gridSnap, 
            Mathf.Round(v.y / gridSnap) * gridSnap, 
            Mathf.Round(v.z / gridSnap) * gridSnap);
    }

    public void toggleTutorialMode()
    {
        tutorialMode = !tutorialMode;

        if (tutorialMode)
        {
            this.tutorialVideoPlayer.gameObject.SetActive(true);
            this.clock.gameObject.SetActive(false);
        }
        else
        {
            this.tutorialVideoPlayer.gameObject.SetActive(false);
            this.clock.gameObject.SetActive(true);
        }
    }

    public void changeClock()
    {
        if (!tutorialMode)
        {
            this.isAnalogClock = !this.isAnalogClock;
            if (this.isAnalogClock)
            {
                clock.transform.Find("AnalogClock").gameObject.SetActive(true);
                clock.transform.Find("DigitalClock").gameObject.SetActive(false);
            }
            else
            {
                clock.transform.Find("AnalogClock").gameObject.SetActive(false);
                clock.transform.Find("DigitalClock").gameObject.SetActive(true);
            }
        }
    }

    public void updateTutorialVideoClip(VideoClip clip)
    {
        StartCoroutine(playVideo(clip));

    }
    public IEnumerator playVideo(VideoClip clip)
    {
        GameObject existing = tutorialVideoPlayer.gameObject;
        GameObject Bob = new GameObject();
        VideoPlayer vid = Bob.AddComponent<VideoPlayer>();
        vid.playOnAwake = false;
        vid.source = VideoSource.VideoClip;
        vid.audioOutputMode = VideoAudioOutputMode.None;
        vid.waitForFirstFrame = false;
        vid.isLooping = true;
        vid.playbackSpeed = 1;
        vid.renderMode = VideoRenderMode.MaterialOverride;
        vid.targetMaterialRenderer = existing.GetComponent<MeshRenderer>();
        vid.targetMaterialProperty = "_MainTex";
        vid.clip = clip;
        vid.Prepare();
        while (!vid.isPrepared)
        {
            yield return null;
        }
        existing.GetComponent<VideoHolder>().newPlayer(vid);
        yield break;
    }
}
