using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NullableNeed = System.Nullable<Need>;
using Math = System.Math;
using InvalidOperationException = System.InvalidOperationException;


public class RefugeeController: MonoBehaviour {

    public SourceManager Sources;

    public static Dictionary<string, float> paramIncreasePerSec = 
    new Dictionary<string, float>() {
        {"Thirst", 10.0f}, {"Exhaustion", 10.0f}, {"Need", 10.0f}
    };

    public static Dictionary<string, float> paramDecreasePerSec = 
    new Dictionary<string, float>() {
        {"Thirst", 50.0f}, {"Exhaustion", 50.0f}, {"Need", 50.0f}
    };



    public float GetHappiness() {
        var h = 100.0f;
        h -= Math.Max(0, animator.GetFloat("Exhaustion") - 100) * Settings.TentsPrior;
        h -= Math.Max(0, animator.GetFloat("Need") - 100) * Settings.WaterPrior;
        h -= Math.Max(0, animator.GetFloat("Thirst") - 100) * Settings.ToiletPrior;
        return h;
    }



    public float GetNeed(Need need) {
        switch(need) {
            case Need.Rest:
                return animator.GetFloat("Exhaustion");
            case Need.WC:
                return animator.GetFloat("Need");
            case Need.Water:
                return animator.GetFloat("Thirst");
            default:
                throw new System.ArgumentException();
        }
    }



    public void SetNearSource(Need need, bool isNear) {
        animator.SetBool(needTrigger[need], isNear);
        if(isNear) {
            isSourceNear.Add(need);
        } else {
            isSourceNear.Remove(need);
        }
    }



    public bool DoesNeed(Need need) {
        var currentNeed = getCurrentNeed();
        return currentNeed != null && currentNeed == need;
    }



    public void StartEntering(BuildingType buildingType, Vector3 destination) {
        animator.SetBool(nearBuildingTrigger[buildingType], true);
        movement.SetStraightDestination(destination);
    }



    public void LeaveBuilding(BuildingType buildingType, Vector3 destination) {
        animator.SetBool(nearBuildingTrigger[buildingType], false);
    }



	void Start () {
        label = transform.Find("Label/StateText").gameObject.GetComponent<Text>();
        
        animator = gameObject.GetComponent<Animator>();

        movement = GetComponent<RefugeeMovement>();
        movement.NavigationGraph = Settings.MapGraph;

        needTrigger[Need.Water] = Animator.StringToHash("NearWaterSource");
        needTrigger[Need.WC] = Animator.StringToHash("InsideWC");
        needTrigger[Need.Rest] = Animator.StringToHash("InsideTent");
        
        nearBuildingTrigger[BuildingType.WaterTank] = Animator.StringToHash("NearWaterTank");
        nearBuildingTrigger[BuildingType.WC] = Animator.StringToHash("NearWCEntrance");
        nearBuildingTrigger[BuildingType.Tent] = Animator.StringToHash("NearTentEntrance");
    }



	void Update () {
        updateParameters();

        Debug.DrawLine(transform.position, 
            transform.position + movement.GetFlatDirection(), Color.red);

        updateState();
	}



    private void updateState() {
        var currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (currentStateInfo.IsName("Idle")) {
            if (goal != Target.Random || movement.HasArrived()) {
                goal = Target.Random;
                var verticesNum = Settings.MapGraph.Vertices.Count;
                var n = Random.Range(0, verticesNum);
                var dest = Settings.MapGraph.Vertices[n].Coord;
                movement.SetDestination(dest);
            }
            label.text = "Idle";
        
        } else if (currentStateInfo.IsName("SearchForWater")) {
            setNewRouteToSource(Need.Water, Target.WaterSource);
            label.text = "Needs water";
       
        } else if (currentStateInfo.IsName("SearchForWC")) {
            setNewRouteToSource(Need.WC, Target.WC);
            label.text = "Needs WC";
        
        } else if (currentStateInfo.IsName("SearchForTent")) {
            setNewRouteToSource(Need.Rest, Target.Tent);
            label.text = "Needs rest";

        } else {
            label.text = "Enters";
        }
    }



    private void setNewRouteToSource(Need need, Target newTarget) {
        goal = newTarget;
        var source = Sources.GetNearestSource(need, transform.position);
        if(source != null) {
            var entrance = source.transform.Find("Entrance");
            var dest = entrance.position;
            if(!dest.Equals(movement.GetDestination())) {
                movement.SetDestination(dest);
            }
        } else {
            movement.SetStraightDestination(transform.position);
        }
    }



    private void updateParameters() {
        updateParameter("Void", "Need");
        updateParameter("Rest", "Exhaustion");
        updateParameter("Drink", "Thirst");
    }



    private void updateParameter(string stateIncreaseName, string needName) {
        var currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        var need = animator.GetFloat(needName);
        if (currentStateInfo.IsName(stateIncreaseName)) {
            var decrease = paramDecreasePerSec[needName] * Time.deltaTime;
            var newNeed = Math.Max(need - decrease, 0);
            animator.SetFloat(needName, newNeed);

        } else {
            var increase = paramIncreasePerSec[needName] * Time.deltaTime;
            var newNeed = Math.Min(need + increase, 200);
            animator.SetFloat(needName, newNeed);
        }
    }



    private bool isNearSource(Need need) {
        return isSourceNear.Contains(need);
    }



    private NullableNeed getCurrentNeed() {
        switch(goal) {
            case Target.WC:
                return Need.WC;
            case Target.Tent:
                return Need.Rest;
            case Target.WaterSource:
                return Need.Water;
            default:
                return null;
        }
    }



    private HashSet<Need> isSourceNear = new HashSet<Need>();
    private Dictionary<Need, int> needTrigger = new Dictionary<Need, int>();
    private Dictionary<BuildingType, int> nearBuildingTrigger = new Dictionary<BuildingType, int>();
    private enum Target {WC, Tent, WaterSource, Random}
    [SerializeField]
    private Target goal;
    private Animator animator;
    private RefugeeMovement movement;
    private Text label;
}
