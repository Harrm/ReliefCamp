using UnityEngine;

class WashroomController: FacilityController {
	
	public override bool IsBusy {
		get {
			return ClientsNum >= 1;
		}
	}

	protected void OnTriggerEnter(Collider other) {
		if(!IsBusy) {
			base.OnTriggerEnter(other);
		}
	}

	protected void OnTriggerStay(Collider other) {
		if(!IsBusy) {
			base.OnTriggerStay(other);
		}
	}

}
