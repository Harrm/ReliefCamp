using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;



public class RefugeeMovement: MonoBehaviour {

	public Graph NavigationGraph;



	public bool HasArrived() {
		return route.Count  == 0;
	}



	// direction.y is always zero
	public Vector3 GetFlatDirection() {
		Vector3 dest;
        if(route.Count > 0) {
            dest = route[0];
            var v = dest - transform.position;
            v = new Vector3(v.x, 0, v.z);
            var rand = new Random(); 
            var r = (float)(rand.NextDouble() - rand.NextDouble()) / 10.0f;
            return v.normalized + new Vector3(r, 0.0f, r);
        }
        return Vector3.zero;
	}



	public Vector3 GetDestination() {
		return (route.Count > 0) ? route[route.Count - 1] : transform.position;
	}



	public void SetStraightDestination(Vector3 dest) {
		route.Clear();
		route.Add(dest);
	}



	public void SetDestination(Vector3 dest) {
		route = NavigationGraph.FindRoute(transform.position, dest);
		route.Add(dest); // because it may not be an exact graph vertex
	}



	public void AddStraightDestination(Vector3 dest) {
		route.Add(dest);
	}



	public void AddDestination(Vector3 dest) {
		var newRoute = NavigationGraph.FindRoute(transform.position, dest);
		route.AddRange(newRoute);
		route.Add(dest); // because it may not be an exact graph vertex

	}



	void Start () {
		route = new List<Vector3>();
		controller = GetComponent<CharacterController>();
	}


	
	void Update () {
		var dir = GetFlatDirection();
		if(!dir.Equals(Vector3.zero)) {
			controller.SimpleMove(dir);
			transform.Find("Character").forward = Vector3.Slerp(transform.Find("Character").forward, dir, 0.1f);
		}
		if(route.Count > 0 && hasArrivedAt(route[0])) {
            route.RemoveAt(0);
        }
	}



    // needed 'cause the actual position is higher than the map 
    private bool hasArrivedAt(Vector3 mapPosition, float precision = 0.05f) {
        var pos = transform.position;
        var dest = mapPosition;
        var flatPos = new Vector3(pos.x, 0, pos.z);
        var flatDest = new Vector3(dest.x, 0, dest.z);
        return (flatDest - flatPos).magnitude < precision;
    }


	private CharacterController controller;
	private List<Vector3> route;
}
