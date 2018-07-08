using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Boggy_Monster_AISM : MonoBehaviour {
	public GameObject _monster;
	public Transform prey;
	public GameObject _Tom;
	public Transform _player;
	public Transform _beast;
	public float _runRadius = 10f;       //radius that player will trigger monster to wnter run state
	public float _idleRadius = 40f;       // outside the range of monster and monster is idle
	public float _investigateRadius = 60f;  // look out in the distance for player by using a wide radius
	public float _attackRadius = 1.5f;   // Attack radius
	public float _stalkingRadius = 150f;
	public float _huntingRadius = 30f;
	public float _intownRadius = 280f;
	public float _enemyRunSpeed = 4.5f;  // monster run speed
	public float _enemyTurnSpeed = 10f;  // how fast monster turrns
	public float roamRadius = 70f;    // Radius used to trigger monster roaming
	private HunterSM_test _huntAI;
	List<Transform> points = new List <Transform> ();
	public GameObject Waypoint;
	GameObject WP;
	float min_distance =-50.0f;
	float max_distance = 50.0f;
	private int destPoint = 0;
	private NavMeshAgent agent;

	public Waypoint_System path;
	public float remainingDistance = 0.3f;

	public bool wayP = false;
	Animator anim;
	private BoggyAI_Night_State _enemyState;
	private enum BoggyAI_Night_State{                  //States enemy lives in
		Initialize =  0,
		Idle = 1,
		Run = 2,
		Investigate = 3,
		Strike = 4,
		Impact = 5,
		Attack = 6,
		Roam = 7,
		Hunt = 8,
		Stalk = 9,
		Alert = 10,
		Dead = 11


	}
	// Use this for initialization
	void Start () {
		GameObject _playerGameObject = GameObject.FindGameObjectWithTag ("Player");
		_player = _playerGameObject.transform;   // caches player position
		prey = GetComponent <Transform>();                      // store the position of the player
		_beast = GetComponent<Transform>();                     // store the position of the monster

		anim = GetComponent<Animator>();
		StartCoroutine (BOGGYAI_FSM ());    // Behaviuor in ths coroutine starts immediately
		points = path.waypoints;
		agent = GetComponent<NavMeshAgent>();
		_huntAI = GetComponent<HunterSM_test> ();      // initialize hunting 

		_huntAI.GetComponent<HunterSM_test>().enabled = false;   // start with hunting off

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private IEnumerator BOGGYAI_FSM(){

		while (true) {                     //while FSM is active
			switch (_enemyState) {         //switch the enemy state
			case BoggyAI_Night_State.Initialize:
				Initialize ();
				break;
			case BoggyAI_Night_State.Idle:
				Idle ();
				break;
			case BoggyAI_Night_State.Run:
				Run ();
				break;
			case BoggyAI_Night_State.Investigate:
				Investigate ();
				break;
			case BoggyAI_Night_State.Roam:
				Roam();
				break;
			case BoggyAI_Night_State.Hunt:
				Hunt();
				break;
			case BoggyAI_Night_State.Stalk:
				Stalk();
				break;
			case BoggyAI_Night_State.Strike:
				Strike ();
				break;
			case BoggyAI_Night_State.Impact:
				Impact ();
				break;
			case BoggyAI_Night_State.Attack:
				Attack();
				break;
			case BoggyAI_Night_State.Alert:
				Alert();
				break;
			case BoggyAI_Night_State.Dead:
				Dead();
				break;
						}
			yield return null;
		}

	}
	private void Initialize(){
		Debug.Log("Initialize");
		//_huntAI.GetComponent<HunterSM_test>().enabled = false;
		anim.SetBool ("isAlert", true);
		anim.SetBool ("isMoving", false);
		anim.SetBool ("isIdle", false);
		StopCoroutine (ChooseAction ());
		float _playdist = Vector3.Distance (_player.position, _beast.transform.position); 

		anim.SetBool ("isMoving", false);
		anim.SetBool ("isAttacking", false);
		anim.SetBool ("isAlert", false);
		anim.SetBool ("isIdle",true);
			
		if (_playdist <= _idleRadius) {
			_enemyState = BoggyAI_Night_State.Idle;
		} else if (_playdist < _runRadius) {                                                    // if player distance is than run radius
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isIdle", false);
			_enemyState = BoggyAI_Night_State.Run;
		} else if (_playdist > _stalkingRadius && (_playdist < _intownRadius)) {                                                    // if player distance is than run radius
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isIdle", false);
			StartCoroutine (ChooseAction ());
		}

	}
	private void Idle(){
		Debug.Log ("Idle");
		StopCoroutine (ChooseAction ());
		anim.SetBool ("isIdle", true);
		anim.SetBool ("isMoving", false);
	
	
		float _playdist = Vector3.Distance (_player.position, _beast.transform.position);      // Find players position

		if (_playdist < _runRadius) {                                                    // if player distance is than run radius
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isIdle", false);
			_enemyState = BoggyAI_Night_State.Run;
		}  else if (_playdist >= _runRadius && (_playdist < roamRadius)) {
			


			_enemyState = BoggyAI_Night_State.Roam;
		}if (_playdist >roamRadius &&(_playdist < _stalkingRadius) ){                                                    // if player distance is than run radius
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isIdle", false);
			_enemyState = BoggyAI_Night_State.Stalk;
		} if (_playdist < _attackRadius) {

			_enemyState = BoggyAI_Night_State.Attack;
		}else if (_playdist >= _runRadius && (_playdist > _huntingRadius) &&(_playdist >= _investigateRadius && ( _playdist < _intownRadius )))  {


			StartCoroutine (ChooseAction ());

		}
	

	}
	private void Run(){

		Debug.Log("Run");
		StopCoroutine (ChooseAction ());
		//StartCoroutine (BOGGYAI_FSM());
		transform.position = Vector3.MoveTowards (_beast.transform.position, _player.transform.position, _enemyRunSpeed * Time.deltaTime);  //move in direction of player
		var rotation = Quaternion.LookRotation (_player.transform.position - transform.position);            // ster turns in direction of player
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, _enemyTurnSpeed * Time.deltaTime);       // Calculate monster turn speed and turn radius
		StopCoroutine (ChooseAction ());
		float _playdist = Vector3.Distance (_player.position, _beast.transform.position);      // Find players position

		if (_playdist >= _runRadius &&(_playdist <= _investigateRadius)) {                                                    // if player distance is than run radius
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isAttacking", false);
			anim.SetBool ("isAlert", false);
			anim.SetBool ("isIdle", false);
			_enemyState = BoggyAI_Night_State.Investigate;
		}
		if (_playdist < _attackRadius) {

			_enemyState = BoggyAI_Night_State.Attack;
		}
		float _preydist = Vector3.Distance (prey.position, _beast.transform.position);
		 if(_preydist < _huntingRadius && (_preydist > _runRadius && _preydist >_investigateRadius)){
			_enemyState = BoggyAI_Night_State.Hunt;
		}else if (_playdist >= _runRadius && (_playdist > _huntingRadius) &&(_playdist >= _investigateRadius && ( _playdist < _intownRadius )))  {


			StartCoroutine (ChooseAction ());

		}
	}

	private void Investigate(){
		Debug.Log("Investigate");

		StopCoroutine (ChooseAction ());
		anim.SetBool ("isMoving", true);
		anim.SetBool ("isAttacking", false);
		anim.SetBool ("isAlert", false);
		anim.SetBool ("isIdle", false);
		SearchForPlayer ();
		transform.position = Vector3.MoveTowards (_beast.transform.position, _player.transform.position, _enemyRunSpeed * Time.deltaTime);  //move in direction of player
		var rotation = Quaternion.LookRotation (_player.transform.position - transform.position);            // ster turns in direction of player
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, _enemyTurnSpeed * Time.deltaTime);       // Calculate monster turn speed and turn radius
		StopCoroutine (ChooseAction ());
		float _preydist = Vector3.Distance (prey.position, _beast.transform.position);
		float _playdist = Vector3.Distance (_player.position, _beast.transform.position);      // Find players position
		if (_playdist < _runRadius) {  

			_enemyState = BoggyAI_Night_State.Run;
		
		}

		if (_playdist < _runRadius) {                                                    // if player distance is than run radius
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isIdle", false);
			_enemyState = BoggyAI_Night_State.Run;
		}
		if (_preydist < _huntingRadius && _playdist > _investigateRadius) {

			_enemyState = BoggyAI_Night_State.Hunt;
		}
		if (_playdist < _attackRadius) {

			_enemyState = BoggyAI_Night_State.Attack;
		}
		else if (_playdist >= _runRadius && (_playdist > _huntingRadius) &&(_playdist >= _investigateRadius && ( _playdist < _intownRadius )))  {
			

			StartCoroutine (ChooseAction ());

			}

	}
	private void Roam(){
		Debug.Log("Roam");

		anim.SetBool ("isMoving", true);
		anim.SetBool ("isAttacking", false);
		anim.SetBool ("isAlert", false);
		anim.SetBool ("isIdle", false);
		roamArea ();
		move ();
		float _playdist = Vector3.Distance (_player.position, _beast.transform.position); 
		if (_playdist >= _runRadius &&(_playdist <= _investigateRadius)) {                                                    // if player distance is than run radius
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isAttacking", false);
			anim.SetBool ("isAlert", false);
			anim.SetBool ("isIdle", false);
			_enemyState = BoggyAI_Night_State.Investigate;
		}
		if (_playdist < _attackRadius) {

			_enemyState = BoggyAI_Night_State.Attack;
		}

	}
	private void Stalk ()
	{
		StopCoroutine (ChooseAction ());
		float _preydist = Vector3.Distance (prey.position, _beast.transform.position);
		float _playdist = Vector3.Distance (_player.position, _beast.transform.position);
		//if (_preydist > _huntingRadius && (_playdist > roamRadius  && (_playdist > _runRadius  && (_playdist > _investigateRadius  &&  ( _playdist <= _stalkingRadius ))))) {
		if (_playdist > roamRadius  &&  ( _playdist <= _stalkingRadius )) {

		//if (_enemyState != BoggyAI_Night_State.Roam || (_enemyState != BoggyAI_Night_State.Investigate) || (_enemyState != BoggyAI_Night_State.Run)) {
			Debug.Log ("Stalk");
			if (agent.remainingDistance < remainingDistance)
				GotoNextPoint();
			
		} 
		if (_playdist >= _runRadius && (_playdist < roamRadius) && (_playdist >= _investigateRadius)) {
			

			StartCoroutine (ChooseAction ());

		}  if (_playdist >= _runRadius && (_playdist <= _investigateRadius)) {                                                    // if player distance is than run radius
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isAttacking", false);
			anim.SetBool ("isAlert", false);
			anim.SetBool ("isIdle", false);
			_enemyState = BoggyAI_Night_State.Investigate;
		} 
		else if (_preydist < _huntingRadius) {

			_enemyState = BoggyAI_Night_State.Hunt;
		}
		
	}

	private void Hunt(){                             // Boggy Monster Hunting behaviour
		Debug.Log("Hunt");
		StopCoroutine (ChooseAction ());
		float _preydist = Vector3.Distance (prey.position, _beast.transform.position);
		float _playdist = Vector3.Distance (_player.position, _beast.transform.position);
		if (_preydist < _huntingRadius && _playdist > _stalkingRadius ) {

			_enemyState = BoggyAI_Night_State.Hunt;
		}

		_huntAI.GetComponent<HunterSM_test>().enabled = true;

		//float _playdist = Vector3.Distance (_player.position, _beast.transform.position);
		if (_playdist >= _runRadius && (_playdist >roamRadius) && (_playdist < _intownRadius)) {
		//if (_enemyState != BoggyAI_Night_State.Roam || (_enemyState != BoggyAI_Night_State.Investigate) || (_enemyState != BoggyAI_Night_State.Run)) {
			Debug.Log ("Stalk");
			if (agent.remainingDistance < remainingDistance)
				GotoNextPoint();
			
		} 
		if (_playdist >= _runRadius && (_playdist < roamRadius) && (_playdist >= _investigateRadius)) {
			

			StartCoroutine (ChooseAction ());

		}  if (_playdist >= _runRadius && (_playdist <= _investigateRadius)) {                                                    // if player distance is than run radius
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isAttacking", false);
			anim.SetBool ("isAlert", false);
			anim.SetBool ("isIdle", false);
			_enemyState = BoggyAI_Night_State.Investigate;
		}else if (_playdist < _attackRadius) {

			_enemyState = BoggyAI_Night_State.Attack;
		}
	}
		

		
		

	
	
	private void Strike(){
		Debug.Log("Strike");

	}

	private void Impact(){
		Debug.Log("Impact");

	}
	private void Attack(){       // Boggy Monster attack behaviour
		StopCoroutine (ChooseAction ());
		Debug.Log("Attack");
		anim.SetBool ("isAttacking", true);
		anim.SetBool ("isMoving", false);
		anim.SetBool ("isAlert", false);
		anim.SetBool ("isIdle", false);
		float _playdist = Vector3.Distance (_player.position, _beast.transform.position);
		if (_playdist > _attackRadius && (_playdist <= _runRadius)) {
			_enemyState = BoggyAI_Night_State.Run;
		} if (_playdist >= _runRadius &&(_playdist <= _investigateRadius)) {                                                    // if player distance is than run radius
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isAttacking", false);
			anim.SetBool ("isAlert", false);
			anim.SetBool ("isIdle", false);
			_enemyState = BoggyAI_Night_State.Investigate;
		}
		else if (_playdist >= _runRadius && (_playdist < roamRadius) &&(_playdist >= _investigateRadius))  {
			//i

			StartCoroutine (ChooseAction ());

		}

	}
	private void Alert(){
		Debug.Log("Alert");
		anim.SetBool ("isAlert", true);
		anim.SetBool ("isIdle", false);
		anim.SetBool ("isMoving", false);
	}
	private void Dead(){
		Debug.Log("Dead");

	}
	void SearchForPlayer(){

		Vector3 center = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z);
		Collider[] hitColliders = Physics.OverlapSphere (center, _investigateRadius);
		int i = 0;
		while (i < hitColliders.Length) {
			if (hitColliders [i].transform.tag == "Player") {
				_Tom = hitColliders [i].transform.gameObject;
			}
			i++;
		}
	}
		void roamArea(){   // used to create the random spots in a square that the moster will roam
		if (!wayP) {
			float distance_x = transform.position.x + Random.Range (min_distance, max_distance);
			float distance_z = transform.position.z + Random.Range (min_distance, max_distance);
			WP = Instantiate (Waypoint, new Vector3 (distance_x, 11, distance_z), Quaternion.identity)as GameObject;
			wayP = true;
		}





		}











	void move(){   // moving monster to waypoints so he patrols randomly

		transform.position = Vector3.MoveTowards (_beast.transform.position, WP.transform.position, _enemyRunSpeed * Time.deltaTime);
		var rotation = Quaternion.LookRotation (WP.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation,  _enemyTurnSpeed * Time.deltaTime);
	}

	void GotoNextPoint() {
		// Returns if no points have been set up
		if (points.Count == 0)
			return;

		// Set the agent to go to the currently selected destination.
		agent.destination = points[destPoint].position;

		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPoint = (destPoint + 1) % points.Count;
	}

		

	private IEnumerator ChooseAction()        // initiate auto behaviour
	{
	while (true) {
				yield return new WaitForSeconds (3.0f);
	if (!wayP) {
				int num = Random.Range (0,2);
				if (num == 0) {
					_enemyState = BoggyAI_Night_State.Roam;
				} else if (num == 1) {
					_enemyState = BoggyAI_Night_State.Alert;

				} //else if (num == 2) {
					//_enemyState = BoggyAI_Night_State.Hunt;
				//}

			}
		}
	}


		
						
			
		
	


	private void OnTriggerEnter(Collider detectplayer){
		if (detectplayer.CompareTag ("Player")) {						//Detects if Player entered Collider
			_enemyState = BoggyAI_Night_State.Initialize;				//set monster state to initialize

			Debug.Log ("FSM-Start");
			float _playdist = Vector3.Distance (_player.position, _beast.transform.position); 
			if (_playdist < _runRadius) {                                                    // if player distance is than run radius
				anim.SetBool ("isMoving", true);
				anim.SetBool ("isIdle", false);
				_enemyState = BoggyAI_Night_State.Run;
			} else if (_playdist >= _runRadius && (_playdist < roamRadius)) {
				//if (_playdist > _idleRadius) {


				_enemyState = BoggyAI_Night_State.Roam;
			}

				
			//Debug.Log ("FSM-Start");

		} else if (Waypoint.CompareTag ("waypoint")) {      // check if collider is  a waypoint
			Destroy (WP);
			wayP = false;
			_enemyState = BoggyAI_Night_State.Idle;
		} else if (Waypoint.CompareTag ("prey")) {           // check if collider is a prey
			_enemyState = BoggyAI_Night_State.Hunt;

		}

		StopCoroutine (ChooseAction ());  
		StartCoroutine (BOGGYAI_FSM ());
	
	}

	private void OnTriggerExit(Collider detectplayer){
		if (detectplayer.CompareTag ("Player")) {						//Detects if Player exits Collider
			float _playdist = Vector3.Distance (_player.position, _beast.transform.position); 
			if (_playdist < _runRadius) {                                                    // if player distance is than run radius
				anim.SetBool ("isMoving", true);
				anim.SetBool ("isIdle", false);
				_enemyState = BoggyAI_Night_State.Run;
				Debug.Log ("FSM-Stop");															
				//StopCoroutine (BOGGYAI_FSM ());								//stop FSM
				StopCoroutine (ChooseAction ());
				
				
			}	

		} else if (Waypoint.CompareTag ("waypoint")) {
			if (_enemyState != BoggyAI_Night_State.Roam || (_enemyState != BoggyAI_Night_State.Investigate)||(_enemyState != BoggyAI_Night_State.Run)||(_enemyState != BoggyAI_Night_State.Stalk)) {
				//StartCoroutine (ChooseAction ());                             // if waypoint found , when it continue roaming 
			
			}
		}

			
	}
	// used to check if radius is working for each State
	private void OnDrawGizmos(){
		Gizmos.color = Color.yellow;                                   // Draw yellow line
		Gizmos.DrawWireSphere (transform.position, _runRadius);         // yellow sphere for run radius

		Gizmos.color = Color.red;                                   // Draw red 
		Gizmos.DrawWireSphere (transform.position, _attackRadius);         // red sphere for run radius

		Gizmos.color = Color.blue;                                   // Draw blue
		Gizmos.DrawWireSphere (transform.position, _stalkingRadius); 
		Gizmos.color = Color.black;                                   // Draw red 
		Gizmos.DrawWireSphere (transform.position, roamRadius);
		Gizmos.color = Color.green;                                   // Draw green 
		Gizmos.DrawWireSphere (transform.position, _investigateRadius);
		Gizmos.color = Color.gray;                                   // Draw grey 
		Gizmos.DrawWireSphere (transform.position, _huntingRadius);
		Gizmos.color = Color.cyan;                                   // Draw grey 
		Gizmos.DrawWireSphere (transform.position, _intownRadius);

	}
}
