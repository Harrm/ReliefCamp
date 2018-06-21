using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsCameraMover: MonoBehaviour {

	public GameObject StatisticsPlot;
	public float CameraSpeed = 1.0f;
	
    void Update() {
        float xShift = 0;

        if (Input.GetKey(KeyCode.A)) xShift -= CameraSpeed;
        if (Input.GetKey(KeyCode.D)) xShift += CameraSpeed;
		
		var x = transform.position.x;
        var leftmost_x = StatisticsPlot.transform.position.x - StatisticsPlot.transform.lossyScale.x / 2;
        if(x + xShift < leftmost_x) {
        	xShift = leftmost_x - x;
        }
        var rightmost_x = StatisticsPlot.transform.position.x + StatisticsPlot.transform.lossyScale.x / 2;
        if(x + xShift > rightmost_x) {
        	xShift = rightmost_x - x;
        }

		transform.Translate(xShift, 0, 0);
    }

}
