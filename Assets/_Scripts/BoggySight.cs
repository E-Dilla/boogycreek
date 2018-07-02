using System.Collections;
using UnityEngine;
using UnityEngine.AI;


namespace UnityStandardAssets.Characters.ThirdPerson
{

    public class BoggySight : MonoBehaviour
    {

        //for audio
        public AudioClip chaseSound;
        AudioSource audioPoint;
        public bool alreadyPlayed = false;

        public NavMeshAgent agent;
        public ThirdPersonCharacter character;

        public enum State
        {
            PATROL,
            NIGHTPATROL,
            CHASE,
            INVESTIGATE,
        }

        public State state;
        private bool alive;

        // Variables for Patrolling
        public GameObject[] waypoints;
        private int waypointInd;
        public float patrolSpeed = 0.5f;

        // Variables for Night Patrolling
        public GameObject[] nightWaypoints;

        // Variables for Chasing 
        public float chaseSpeed = 1f;
        public GameObject target;
        //public GameObject sightSpear;

        // Variables for Investigate 
        private Vector3 investiageSpot;
        private float timer = 0;
        public float investigateWait = 10;
        public bool investigation = false;

        // Variables for Sight
        public float heightMultiplier;
        public float sightDist;

        // Use this for initialization
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();
            audioPoint = GetComponent<AudioSource>();

            agent.updatePosition = true;
            agent.updateRotation = false;

            nightWaypoints = GameObject.FindGameObjectsWithTag("nightwaypoint");
            waypoints = GameObject.FindGameObjectsWithTag("waypoint");
#pragma warning disable CS0618 // Type or member is obsolete
            waypointInd = Random.RandomRange(0, waypoints.Length);
#pragma warning restore CS0618 // Type or member is obsolete

            heightMultiplier = 1.36f;

            // must state with an state, or it will not start
            state = BoggySight.State.PATROL;

            alive = true;

            // start FSM
            StartCoroutine(FSM());
        }

        void Update()
        {
            /*
            StartCoroutine(FSM());

            if (investigation)
            {
                agent.speed = 0;
                timer += Time.deltaTime;
                transform.LookAt(target.transform.position);
                agent.SetDestination(this.transform.position);
                character.Move(Vector3.zero, false, false);
                if (timer >= investigateWait)
                {
                    state = BoggySight.State.PATROL;
                    investigation = false;
                    timer = 0;
                }
            }
            */
        }

        IEnumerator FSM()
        {
            while (alive)
            {
                switch (state)
                {
                    case State.PATROL:
                        Patrol();
                        break;
                    case State.NIGHTPATROL:
                        NightPatrol();
                        break;
                    case State.CHASE:
                        Chase();
                        break;
                    case State.INVESTIGATE:
                        Investigate();
                        break;
                }
                yield return null;
            }

        }


        void Patrol()
        {
            agent.speed = patrolSpeed;
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

        void NightPatrol()
        {
            agent.speed = patrolSpeed;
            if (Vector3.Distance(this.transform.position, nightWaypoints[waypointInd].transform.position) >= 2)
            {
                agent.SetDestination(nightWaypoints[waypointInd].transform.position);
                character.Move(agent.desiredVelocity, false, false);
            }
            else if (Vector3.Distance(this.transform.position, nightWaypoints[waypointInd].transform.position) <= 2)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                waypointInd = Random.RandomRange(0, nightWaypoints.Length);
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
            agent.SetDestination(this.transform.position);
            character.Move(Vector3.zero, false, false);
            transform.LookAt(investiageSpot);
            //transform.LookAt(target.transform.position);
            
            if (timer >= investigateWait)
            {
                state = BoggySight.State.PATROL;
                timer = 0;
            }
            
        }



        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Player")
            {
                if (!alreadyPlayed)
                {
                    audioPoint.PlayOneShot(chaseSound);
                    alreadyPlayed = true;
                }
                //investigation = true;
                state = BoggySight.State.INVESTIGATE;
                target = col.gameObject;
            }
        }



        void FixedUpdate()
        {
            StartCoroutine(FSM());
            RaycastHit hit;
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, transform.forward * sightDist, Color.green);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized * sightDist, Color.red);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized * sightDist, Color.red);
            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    state = BoggySight.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }
            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    state = BoggySight.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }
            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    state = BoggySight.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }



            /*
            if (investigation)
            {
                agent.speed = 0;
                timer += Time.deltaTime;
                transform.LookAt(target.transform.position);
                agent.SetDestination(this.transform.position);
                character.Move(Vector3.zero, false, false);
                if (timer >= investigateWait)
                {
                    state = BoggySight.State.PATROL;
                    investigation = false;
                    //sightSpear.SetActive(false);
                    timer = 0;
                }
            }
            */
        }
    }
}


