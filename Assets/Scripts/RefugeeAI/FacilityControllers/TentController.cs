using UnityEngine;



class TentController: FacilityController {

	public override bool IsBusy {
		get {
			return ClientsNum >= 4;
		}
	}

	protected void OnTriggerEnter(Collider other) {
		if (!IsBusy) {
			base.OnTriggerEnter(other);
		}
	}

	protected void OnTriggerStay(Collider other) {
		if(!IsBusy) {
			base.OnTriggerStay(other);
		}
	}


}
