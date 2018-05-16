using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealTime : MonoBehaviour {

    TextMesh realTimeText;
    public float seconds, minutes;
	// Use this for initialization
	void Start () {
        realTimeText = GetComponent<TextMesh>() as TextMesh;
	}
	
	// Update is called once per frame
	void Update () {
		// this can be useful, it counts the total game time
        // minutes = (int)(Time.time / 60f);
        // seconds = (int)(Time.time % 60);
        // realTimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        string time = System.DateTime.Now.ToString("HH:mm dd MM, yyyy");
        string hour = time.Substring(0,2);
        string minute = time.Substring(3,2);
        string meridian = " AM";
        int intHour = int.Parse(hour);
        // normalize hours from 'military time'
        if (intHour > 12){
        	meridian = " PM";
            intHour -= 12;
            hour = intHour.ToString();
        }

        time = hour + ':' + minute + meridian;
        realTimeText.text = time;
    }
}
