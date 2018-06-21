using UnityEngine;
using System.Collections.Generic;


abstract class FacilityController: MonoBehaviour {
	
	public Need SatisfiesNeed = Need.Rest;
	public BuildingType Type = BuildingType.Tent;
	public int ClientsNum = 0;
	public abstract bool IsBusy { get; }

	void Start() {
		foreach (Transform child in transform) {
			if (child.gameObject.CompareTag("Source")) {
				sources.Add(child.gameObject);
				child.gameObject.GetComponent<SourceController>().Facility = this;
			}
		}
	}

	public void LeadToExit(GameObject refugee) {
		var controller = refugee.GetComponent<RefugeeController>();
		// TODO
	}

	public void OnTriggerEnter(Collider other) {
		var obj = other.gameObject;
		if(obj.CompareTag("Refugee") && obj.GetComponent<RefugeeController>().DoesNeed(SatisfiesNeed)
			&& (!IsBusy)) {
			leadToSource(obj);
		}
	}

	public void OnTriggerStay(Collider other) {
		OnTriggerEnter(other);	
	}

	public void OnTriggerExit(Collider other) {
		var controller = other.gameObject.GetComponent<RefugeeController>();
		if(other.gameObject.CompareTag("Refugee")) {
			controller.LeaveBuilding(Type, Vector3.zero);
		}
	}

	protected void leadToSource(GameObject refugee) {
		var controller = refugee.GetComponent<RefugeeController>();
		Vector3 sourcePosition;
		sourcePosition = sources[0].transform.position;
		controller.StartEntering(Type, sourcePosition);
	}

	protected List<GameObject> sources = new List<GameObject>();

}
