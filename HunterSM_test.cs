using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterSM_test : MonoBehaviour {
	NavMeshAgent agent;
	Animator anim;
	//AudioSource audio;
	AudioClip _audio;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		//audio = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		FindClosestenemy ();
	}
	void FindClosestenemy()
	{
		float distanceToClosestEnemy = Mathf.Infinity;
		Enemy closestEnemy = null;
		Enemy[] allEnemies = GameObject.FindObjectsOfType<Enemy> ();

		foreach (Enemy currentEnemy in allEnemies) {
			float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;

			if (distanceToEnemy < distanceToClosestEnemy) {
				distanceToClosestEnemy = distanceToEnemy;
				closestEnemy = currentEnemy;
				//FindObjectOfType<AudioManager>().Play("Moving);
			}
		}

		Debug.DrawLine (this.transform.position, closestEnemy.transform.position);
		anim.SetBool ("isMoving", true);
		FindObjectOfType<AudioManager>().Play("Moving");
		agent.SetDestination (closestEnemy.transform.position);

			
	}
	private void OnTriggerEnter(Collider detectprey){
		if (detectprey.CompareTag ("prey")) {	
			FindObjectOfType<AudioManager>().Play("Attacking");
			anim.SetBool ("isAttacking", true);

			anim.SetBool ("isMoving", false);
			//FindObjectOfType<AudioManager>().Play("Moving");	
		}

	}

}
