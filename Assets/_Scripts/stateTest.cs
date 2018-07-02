using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{

    public class stateTest : MonoBehaviour
    {

        public ThirdPersonCharacter character;
        public NavMeshAgent agent;

        public enum State
        {
            PATROLS,
            CHASE,
            INVESTIGATE
        }

        public State state;
        private bool alive;

        // Variables for PATROLSling
        public GameObject[] waypoints;
        private int waypointInd;
        public float PATROLSpeed = 0.5f;

        // Variables for Chasing 
        public float chaseSpeed = 1f;
        public GameObject target;

        // Variables for Investigate 
        private Vector3 investiageSpot;
        private float timer = 0;
        public float investigateWait = 10;

        // Variables for Sight
        public float heightMultiplier;
        public float sightDist;

        // Use this for initialization
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();
            //audioPoint = GetComponent<AudioSource>();

            agent.updatePosition = true;
            agent.updateRotation = false;

            waypoints = GameObject.FindGameObjectsWithTag("waypoint");
#pragma warning disable CS0618 // Type or member is obsolete
            waypointInd = Random.RandomRange(0, waypoints.Length);
#pragma warning restore CS0618 // Type or member is obsolete

            heightMultiplier = 1.36f;

            // must state with an state, or it will not start
            state = stateTest.State.PATROLS;

            alive = true;

            // start FSM
            //StartCoroutine(FSM());

        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator FSM()
        {
            while (alive)
            {
                switch (state)
                {
                    case State.PATROLS:
                        PATROLS();
                        break;
                    case State.CHASE:
                        Chase();
                        break;
                    case State.INVESTIGATE:
                        break;
                }
                yield return null;
            }
        }

        void PATROLS()
        {
            agent.speed = PATROLSpeed;
            if (Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) >= 2)
            {
                agent.SetDestination(waypoints[waypointInd].transform.position);
                character.Move(agent.desiredVelocity, false, false);
            }
            else if (Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) <= 2)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                waypointInd = Random.RandomRange(0, waypoints.Length);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }

        void Chase()
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(target.transform.position);
            character.Move(agent.desiredVelocity, false, false);

        }

        void Investigate()
        {
            timer += Time.deltaTime;
            //agent.SetDestination(this.transform.position);
            character.Move(Vector3.zero, false, false);
            //transform.LookAt(investiageSpot);
            transform.LookAt(target.transform.position);
            if (timer >= investigateWait)
            {
                state = stateTest.State.PATROLS;
                timer = 0;
            }
        }


        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Player")
            {

                target = col.gameObject;
                //agent.SetDestination(this.transform.position);
                //character.Move(Vector3.zero, false, false);
                //transform.LookAt(investiageSpot);
                transform.LookAt(target.transform.position);
                state = stateTest.State.INVESTIGATE;

            }
        }
    }
}