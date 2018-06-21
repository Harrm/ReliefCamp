using UnityEngine;



public class RefugeeLabelManager: MonoBehaviour {

	public StatisticsHolder Statistics;

	public void SetStateLabelsActive(bool areActive) {
		foreach(var refugee in Statistics.Refugees) {
			refugee.transform.Find("Label/StateText").gameObject.SetActive(areActive);
		}
	}

	public void SetHappinessLabelsActive(bool areActive) {
		foreach(var refugee in Statistics.Refugees) {
			refugee.transform.Find("Label/HappinessText").gameObject.SetActive(areActive);
		}
	}

	public void SetParamLabelsActive(bool areActive) {
		foreach(var refugee in Statistics.Refugees) {
			refugee.transform.Find("Label/ExhaustionText").gameObject.SetActive(areActive);
			refugee.transform.Find("Label/ThirstText").gameObject.SetActive(areActive);
			refugee.transform.Find("Label/NeedText").gameObject.SetActive(areActive);
		}
	}

}