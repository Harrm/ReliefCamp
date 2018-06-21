using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Xml.Serialization;
using Refugee.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Settings: MonoBehaviour
{
    private static Settings instance;

    [SerializeField]
    private InputField mapSizeX;
    [SerializeField]
    private InputField mapSizeY;
    //[SerializeField]
    //private InputField aiRuns;
    [SerializeField]
    private InputField amountOfWater;
    [SerializeField]
    private InputField amountOfWashrooms;
    [SerializeField]
    private InputField amountOfWatertanks;
    [SerializeField]
    private InputField amountOfTents;
    [SerializeField]
    private InputField amountOfPeople;

    //formula
    [SerializeField]
    private InputField tentsDist;
    [SerializeField]
    private InputField waterDist;
    [SerializeField]
    private InputField toiletDist;
    [SerializeField]
    private InputField toiletToWaterDist;
    [SerializeField]
    private InputField tentsPrior;
    [SerializeField]
    private InputField waterPrior;
    [SerializeField]
    private InputField toiletPrior;
    [SerializeField]
    private InputField toiletToWaterPrior;

    //water
    [SerializeField]
    private InputField minWaterDelay;
    [SerializeField]
    private InputField drinkTime;

    //obstacles
    [SerializeField]
    private InputField rockDecrease;
    [SerializeField]
    private InputField slopeDecrease;
    [SerializeField]
    private InputField slopeHeight;

    [SerializeField]
    private Text warning;

    private void Start()
    {

    }
    private void Update()
    {
        
    }

    private void SetParameters() {
        MapSizeX = int.Parse(mapSizeX.text);
        MapSizeY = int.Parse(mapSizeY.text);
        //AiRuns = int.Parse(aiRuns.text);
        AmountOfWater = int.Parse(amountOfWater.text);
        AmountOfWashrooms = int.Parse(amountOfWashrooms.text);
        AmountOfWatertanks = int.Parse(amountOfWatertanks.text);
        AmountOfTents = int.Parse(amountOfTents.text);
        AmountOfPeople = int.Parse(amountOfPeople.text);

        TentsThreshold = int.Parse(tentsDist.text);
        WaterThreshold = int.Parse(waterDist.text);
        ToiletThreshold = int.Parse(toiletDist.text);
        ToiletToWaterThreshold = int.Parse(toiletToWaterDist.text);
        TentsPrior = float.Parse(tentsPrior.text);
        WaterPrior = float.Parse(waterPrior.text);
        ToiletPrior = float.Parse(toiletPrior.text);
        ToiletToWaterPrior = float.Parse(toiletToWaterPrior.text);

        SlopeDecrease = int.Parse(slopeDecrease.text);
        RockDecrease = int.Parse(rockDecrease.text);
        SlopeHeight = int.Parse(slopeHeight.text);

        MaxHeightDifference = 1; //For graph building

        WaterDelay = int.Parse(minWaterDelay.text);
        DrinkTime = float.Parse(drinkTime.text);
    }

    public static int MapSizeX { get; set; }
    public static int MapSizeY { get; set; }
    //public static int AiRuns { get; set; }
    public static int AmountOfWater { get; set; }
    public static int AmountOfWashrooms { get; set; }
    public static int AmountOfWatertanks { get; set; }
    public static int AmountOfTents { get; set; }
    public static int AmountOfPeople { get; set; }
    //formula
    public static int TentsThreshold {get; set;}
    public static int WaterThreshold {get; set;}
    public static int ToiletThreshold {get; set;}
    public static int ToiletToWaterThreshold {get; set;}
    public static float TentsPrior {get; set;}
    public static float WaterPrior {get; set;}
    public static float ToiletPrior {get; set;}
    public static float ToiletToWaterPrior {get; set;}
    //obstacles
    public static int SlopeDecrease {get;set;}
    public static float SlopeHeight {get; set;}
    public static int RockDecrease {get; set;}
    //camera
    public static Vector3 CameraPosition {get; set;}
    //water
    public static int WaterDelay;
    public static float DrinkTime {get;set;}
    //terrain
    public static float MaxHeightDifference {get; set;}
	public static List<Vector3> MapVertices { get; set; }   // просто записывать меш?
	public static Vector2[] MapUVs { get; set; }
	//public static List<int> MapIndexes { get; set; }
	public static List<Vector3> RocksCoords { get; set; }
    //public static Material MapMaterial { get; set; }

    //Water
    public static float WaterHeight = -0.5f;
    public static List<Vector3> WaterVertices {get; set;}
    public static Vector2[] WaterUVs {get; set;}
    //Water end
    public static string LoadPath = "";
    public static bool IsLoad = false;

    public static Graph MapGraph {get; set;}

    public static bool isRock(Vector3 vect) {
        foreach(var v3 in RocksCoords) {
            if((int)v3.x == (int)vect.x && (int)v3.z == (int)vect.z)
                return true;
        }
        return false;
    }

    public bool OnClickStart() {
        bool res = true;
        try {
            SetParameters();
        } catch(FormatException ex) {
            res = false;
            Color col = warning.color;
            col.a = 255f;
            warning.color = col;
        }
        return res;
    }



    public static void Save(string path) {
        var serSet = new SerializebleSettings();
        serSet.Rocks = RocksCoords;
        serSet.Verts = MapVertices;
        serSet.MapUVs = MapUVs;
        serSet.Water = WaterVertices;
        serSet.WaterUVs = WaterUVs;
        serSet.WaterHeight = WaterHeight;

        XmlSerializer serializer = new XmlSerializer(typeof(SerializebleSettings));
        FileStream fs = new FileStream(path,FileMode.Create);

        serializer.Serialize(fs, serSet);
        fs.Close();
    }

    public static bool Load(string path) {
        if(path=="")
            return false;

        try {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializebleSettings));
            FileStream fs = new FileStream(path,FileMode.Open);
            SerializebleSettings loaded = (SerializebleSettings)serializer.Deserialize(fs);
            fs.Close();

            MapVertices = loaded.Verts;
            RocksCoords = loaded.Rocks;
            MapUVs = loaded.MapUVs;
            WaterVertices = loaded.Water;
            WaterUVs = loaded.WaterUVs;
            WaterHeight = loaded.WaterHeight;
        } catch(FileNotFoundException ex) {
            return false;
        }

        return true;
    }
}

public class SerializebleSettings {
    public List<Vector3> Verts {get; set;}
    public List<Vector3> Rocks {get; set;}
    public Vector2[] MapUVs { get; set; }
    public List<Vector3> Water {get; set;}
    public Vector2[] WaterUVs { get; set; }
    public float WaterHeight {get; set; }
}
