using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour {

    public float hourRotation = -5f;
    public float minuteRotation = -7f;
    public GameObject hourHand;
    public GameObject minuteHand;

	// Update is called once per frame
	void Update () {
        hourHand.transform.Rotate(0, 0, hourRotation);
        minuteHand.transform.Rotate(0, 0, minuteRotation);
    }
}
