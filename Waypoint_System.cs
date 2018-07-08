using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint_System : MonoBehaviour {
	
	public List<Transform> waypoints = new List <Transform> ();

	int index = 0;
	public bool disable;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if(!disable){
			Transform[] tem = GetComponentsInChildren<Transform> ();
		if (tem.Length > 0) {
			waypoints.Clear ();    // waypoints invisible

			index = 0;
			foreach (Transform t in tem) {
					if (t != transform) {
						index++;
						t.name = "Way_" + index.ToString ();
						waypoints.Add (t);
						index++;
					}
				}
			}
		}
	}

	void OnDrawGizmos(){
		if (waypoints.Count > 0) {
			Gizmos.color = Color.green;  // color of sphere

			foreach (Transform t in waypoints)   // reaches through waypoint
				Gizmos.DrawSphere (t.position, 1f);//  draws a sphere


			Gizmos.color = Color.red;   // color of line drawn

			for (int a = 0; a < waypoints.Count - 1; a++)        // for loop to draw line depending on waypoint count
				Gizmos.DrawLine (waypoints [a].position, waypoints [a + 1].position);
		}
	}


}
