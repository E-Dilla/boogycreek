using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BoggyController : MonoBehaviour {
	public float time;
	public TimeSpan currenttime;
	public Transform SunTransform;
	//public Text timetext;
	public Light Sun;
	public int days;

	public GameObject _boggyMonster;
	public float intensity;
	public Color fogday = Color.grey;
	public Color fognight = Color.black;

	public int speed;
	// Use this for initialization
	void Start () {
		_boggyMonster.GetComponent<BoggyMonster_Day> ();
	}
	
	// Update is called once per frame
	void Update () {
		ChangeTime ();
		boggyChangeState ();
	}
	public void ChangeTime(){

		time += Time.deltaTime * speed;
		if(time  > 6400)
		{
			days += 1;
			time = 0;

		}
		currenttime = TimeSpan.FromSeconds (time);
		//string[] temptime = currenttime.ToString ().Split (":" [0]);
		//timetext.text = temptime [0] + ":" + temptime [1];
		SunTransform.rotation = Quaternion.Euler (new Vector3 ((time - 1600)/6400*360,0,0));
		if (time < 3200)
			intensity = 1 - (3200 - time) / 3200;
		else
			intensity = 1 - ((3200 - time) / 3200 * -1);
		RenderSettings.fogColor = Color.Lerp (fogday,fognight,  intensity * intensity);
		Sun.intensity = intensity;
	}
	public void boggyChangeState(){

		if (time >= 1180) {
			_boggyMonster.GetComponent<Boggy_Monster_AISM> ().enabled = true;
			_boggyMonster.GetComponent<BoggyMonster_Day> ().enabled = false;

		}
		if (time <= 1180) {
			_boggyMonster.GetComponent<Boggy_Monster_AISM> ().enabled = false;
			_boggyMonster.GetComponent<BoggyMonster_Day> ().enabled = true;

		}

	}
}

