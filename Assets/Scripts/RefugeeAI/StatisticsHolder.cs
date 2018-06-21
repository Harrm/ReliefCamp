using System.Collections;
using System.Collections.Generic;
using System;
using Random = System.Random;
using UnityEngine;


public class StatisticsHolder: MonoBehaviour {

	public List<RefugeeController> Refugees = new List<RefugeeController>();
	public const float DeltaTime = 0.5f; // In seconds
	public float TimePassed {
		get { return averageHappiness.Count * StatisticsHolder.DeltaTime; }
	}



	public float GetAverageHappiness(float timestamp) {
		int index = (int)Math.Floor(timestamp / StatisticsHolder.DeltaTime);
		return averageHappiness[index-1];
	}



	public float GetAverageNeed(Need need, float timestamp) {
		int index = (int)Math.Floor(timestamp / StatisticsHolder.DeltaTime);
		return averageNeed[need][index-1];
	}



	void Start() {
		averageNeed.Add(Need.WC, new List<float>());
		averageNeed.Add(Need.Water, new List<float>());
		averageNeed.Add(Need.Rest, new List<float>());
		StartCoroutine(updateValues());
	}



	private IEnumerator updateValues() {
		while(true) {
			updateAverageHappiness();
			updateAverageNeed(Need.WC);
			updateAverageNeed(Need.Water);
			updateAverageNeed(Need.Rest);
        	yield return new WaitForSeconds(StatisticsHolder.DeltaTime);
		}
	}



	private void updateAverageHappiness() {
		if(Refugees.Count == 0) {
			averageHappiness.Add(0);
			return;
		}
		float average = 0.0f;
		foreach(RefugeeController r in Refugees) {
			average += r.GetHappiness();
		}
		average /= Refugees.Count;
		averageHappiness.Add(average);
	}



	private void updateAverageNeed(Need need) {
		if(Refugees.Count == 0) {
			averageNeed[need].Add(0);
			return;
		}
		float average = 0.0f;
		foreach(RefugeeController r in Refugees) {
			average += r.GetNeed(need);
		}
		average /= Refugees.Count;
		averageNeed[need].Add(average);
	}



	private List<float> averageHappiness = new List<float>();
	private Dictionary<Need, List<float>> averageNeed = new Dictionary<Need, List<float>>();
}