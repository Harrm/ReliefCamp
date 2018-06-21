using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEventThrower : MonoBehaviour {

	FacilityController c;

	void Start () {
		c = transform.parent.GetComponent<FacilityController>();
	}
	
	void OnTriggerEnter(Collider other) {
		c.OnTriggerEnter(other);
	}

}
