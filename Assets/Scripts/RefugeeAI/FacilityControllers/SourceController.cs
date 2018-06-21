using UnityEngine;
using UnityEngine.UI;



class SourceController: MonoBehaviour {

	public FacilityController Facility;

	void OnTriggerEnter(Collider other) {
		var obj = other.gameObject;
		var controller = obj.GetComponent<RefugeeController>();
		if(obj.CompareTag("Refugee") && controller.DoesNeed(Facility.SatisfiesNeed)) {
			controller.SetNearSource(Facility.SatisfiesNeed, true);
			Facility.ClientsNum++;
		}
	}

	void OnTriggerExit(Collider other) {
		if(other.gameObject.tag == "Refugee") {
			var controller = other.gameObject.GetComponent<RefugeeController>();
			Facility.LeadToExit(other.gameObject);
			// we cannot consider only real clients without ones passing by, so it may turn negative
			if(Facility.ClientsNum > 0) Facility.ClientsNum--;
		}
	}

}
