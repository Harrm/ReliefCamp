using UnityEngine;

class WaterTankController: FacilityController {
	
	public override bool IsBusy {
		get {
			return ClientsNum > 4;
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

	protected void leadToSource(GameObject refugee) {
		var controller = refugee.GetComponent<RefugeeController>();
		Vector3 sourcePosition;
		var source = sources[lastGivenSource];
		lastGivenSource++;
		lastGivenSource %= sources.Count;
		sourcePosition = source.transform.position;
		controller.StartEntering(Type, sourcePosition);
	}

	private int lastGivenSource = 0;
}
