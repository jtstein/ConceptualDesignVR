using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoHolder : MonoBehaviour {

    public VideoPlayer playing;
    public void newPlayer(VideoPlayer player)
    {
        if (!player.isPrepared)
        { 
            return;
        }
        if(playing != null)
        {
            playing.Stop();
        }
        playing = player;
        playing.Play();
    }
}
