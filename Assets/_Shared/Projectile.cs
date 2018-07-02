using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

    [SerializeField] float speed;
    [SerializeField] float timeToLive;
    [SerializeField] float damage;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, timeToLive);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        var destrutable = other.transform.GetComponent<Destructable>();
        if (destrutable == null)
            return;

        destrutable.TakeDamage(damage);
    }
}
