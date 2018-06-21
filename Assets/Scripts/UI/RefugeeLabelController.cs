using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;



public class RefugeeLabelController: MonoBehaviour {

	public RefugeeController Controller;
	public Text HappinessText;
	public Text ExhaustionText;
	public Text ThirstText;
	public Text NeedText;



	void Start () {
		Controller = transform.parent.GetComponent<RefugeeController>();
		HappinessText = transform.Find("HappinessText").GetComponent<Text>();
		ExhaustionText = transform.Find("ExhaustionText").GetComponent<Text>();
		ThirstText = transform.Find("ThirstText").GetComponent<Text>();
		NeedText = transform.Find("NeedText").GetComponent<Text>();

		var constraint = new ConstraintSource();
		constraint.sourceTransform = Camera.main.transform;
		constraint.weight = 1.0f;
		GetComponent<AimConstraint>().AddSource(constraint);
	}
	


	void Update () {
		HappinessText.text = "Happiness: " + (int)Controller.GetHappiness();
		ExhaustionText.text = "E: " + (int)Controller.GetNeed(Need.Rest);
		ThirstText.text = "T: " + (int)Controller.GetNeed(Need.Water);
		NeedText.text = "N: " + (int)Controller.GetNeed(Need.WC);
		transform.LookAt(Camera.main.transform.position);
		transform.forward *= -1;
	}

}
