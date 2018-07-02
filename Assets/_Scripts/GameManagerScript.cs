using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {

    public static bool isDayTime;
    public static bool isNightTime;

    public float clockTime;

    public Text hourText;
    public float hourCount;
    public float hourPerSecond;
    


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        
        if (hourCount >= 12.5)
        {
            hourCount = 1;
        }

        hourCount += hourPerSecond * Time.deltaTime;
        hourText.text = " " + Mathf.Round(hourCount);

        SunTime();

	}

    void SunTime()
    {
        clockTime += Time.deltaTime;

        if (clockTime >= 360)
        {
            clockTime = 0;
        }

        if (clockTime <= 180f)
        {
            isDayTime = true;
        }
        else
        {
            isDayTime = false;
        }
    }
}
