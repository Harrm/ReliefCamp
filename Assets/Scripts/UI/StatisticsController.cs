using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;



public class StatisticsController: MonoBehaviour {

    public float MaxPlotHeight = 1.0f;
    public float PlotStepPerTimeUnit = 2.5f;

	public LineRenderer HappinessPlot;	
	public LineRenderer ExhaustionPlot;
	public LineRenderer ThirstPlot;
	public LineRenderer WCNeedPlot;

    public GameObject Background;

	public StatisticsHolder Statistics;


	void Start() {
		StartCoroutine(updatePlots());
	}



    void OnEnable() {
    	if(Statistics != null)
    		buildPlots();
			StartCoroutine(updatePlots());
    }



    private void buildPlots() {
        var positionCount = (int)(Statistics.TimePassed / StatisticsHolder.DeltaTime);
    	var positions = new Vector3[positionCount];
    	for(int i = 0; i < positionCount; i++) {
            var v = Statistics.GetAverageHappiness((i + 1) * StatisticsHolder.DeltaTime);
    		var clamped_v = MaxPlotHeight * v / 100.0f;
    		positions[i] = new Vector3(PlotStepPerTimeUnit * i, clamped_v, 0.0f);
    	}
    	HappinessPlot.positionCount = positionCount;
    	HappinessPlot.SetPositions(positions);
    	buildNeedPlot(ExhaustionPlot, Need.Rest);
    	buildNeedPlot(ThirstPlot, Need.Water);
    	buildNeedPlot(WCNeedPlot, Need.WC);
    }



    private void buildNeedPlot(LineRenderer plot, Need need) {
		var positionCount = (int)(Statistics.TimePassed / StatisticsHolder.DeltaTime);
        var positions = new Vector3[positionCount];
        for(int i = 0; i < positionCount; i++) {
    		var v = Statistics.GetAverageNeed(need, (i + 1) * StatisticsHolder.DeltaTime);
    		var clamped_v = MaxPlotHeight * v / 200.0f;
            positions[i] = new Vector3(PlotStepPerTimeUnit * i, clamped_v, 0.0f);
    	}
    	plot.positionCount = positionCount;
    	plot.SetPositions(positions);
    }



	private IEnumerator updatePlots() {
        while(true) {
        	var t = Statistics.TimePassed;
        	updatePlot(HappinessPlot, MaxPlotHeight * Statistics.GetAverageHappiness(t) / 100.0f);
        	updatePlot(ExhaustionPlot, MaxPlotHeight * Statistics.GetAverageNeed(Need.Rest, t) / 200.0f);
        	updatePlot(ThirstPlot, MaxPlotHeight * Statistics.GetAverageNeed(Need.Water, t) / 200.0f);
        	updatePlot(WCNeedPlot, MaxPlotHeight * Statistics.GetAverageNeed(Need.WC, t) / 200.0f);
            adjustBackgroundSize();

        	yield return new WaitForSeconds(0.5f);
        }
	}



    private IEnumerator updatePlotsRandomly() {
        while(true) {
            var rand = new Random();
            updatePlot(HappinessPlot, (float)rand.NextDouble() * MaxPlotHeight);
            updatePlot(ExhaustionPlot, (float)rand.NextDouble() * MaxPlotHeight);
            updatePlot(ThirstPlot, (float)rand.NextDouble() * MaxPlotHeight);
            updatePlot(WCNeedPlot, (float)rand.NextDouble() * MaxPlotHeight);
            adjustBackgroundSize();

            yield return new WaitForSeconds(0.5f);
        }
    }



    private void updatePlot(LineRenderer plot, float newValue) {
		plot.positionCount += 1;
        var len = plot.positionCount - 1;
        plot.SetPosition(len, new Vector3(PlotStepPerTimeUnit * len, newValue, 0.0f));
	}


    private void adjustBackgroundSize() {
        float plotWidth = HappinessPlot.positionCount * PlotStepPerTimeUnit * HappinessPlot.transform.localScale.x;
        var threshold = Background.transform.localScale.x - 2.0f;
        if(plotWidth > threshold) {
            var oldScale = Background.transform.localScale;
            var newScaleX = plotWidth + 2.0f;
            Background.transform.localScale = new Vector3(newScaleX, oldScale.y, oldScale.z);
            Background.transform.Translate(Vector3.right * (newScaleX - oldScale.x) / 2.0f);
        }
    }

}
