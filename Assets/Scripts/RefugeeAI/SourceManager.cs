using System.Collections;
using System.Collections.Generic;
using Random = System.Random;
using UnityEngine;

public class SourceManager: MonoBehaviour {

	public GameObject GetNearestSource(Need need, Vector3 start) {
        GameObject nearest = null;
        
        foreach (var source in sources[need]) {
     		if(source.GetComponent<FacilityController>().IsBusy) {
				continue;
     		}
     		if(nearest == null) {
     			nearest = source;
     			
     		} else {
     			var sourceToStart = Vector3.Distance(source.transform.position, start);
        		var nearestToStart = Vector3.Distance(nearest.transform.position, start);
        		if(sourceToStart < nearestToStart) {
        			nearest = source;
        		}
        	}
        }
        return nearest;
	}



	public Vector3 GetDirectionToNearestSource(Need need, Vector3 from) {
		var s = GetNearestSource(need, from);
		if (s != null) {
			var d = transform.position - from;
			return d / d.magnitude;
		}
		return Vector3.zero;
	}

	public List<GameObject> Washrooms;

	public List<GameObject> Tents;

	public List<GameObject> WaterSources;

	void Start () {
		sources[Need.Rest] = Tents;
		sources[Need.WC] = Washrooms;
		sources[Need.Water] = WaterSources;
	}



	private Dictionary<Need, List<GameObject>> sources = new Dictionary<Need, List<GameObject>>();
}
