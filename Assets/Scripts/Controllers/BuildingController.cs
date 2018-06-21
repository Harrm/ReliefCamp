using System;
using UnityEngine;
using System.Collections;
using Refugee.Genetic;
using UnityEngine.UI;
using Refugee.Genetic.Data;
using System.Collections.Generic;
using System.IO;
using Refugee.Misc;
using Random = System.Random;

namespace Refugee.Controllers
{
    public class BuildingController: MonoBehaviour
    {
        // Buildings on the map
        public List<GameObject> tents = new List<GameObject>();
        public List<GameObject> watertanks = new List<GameObject>();
        public List<GameObject> washrooms = new List<GameObject>();

        public SimulationController Simulation;
        public UIController UI;
        public StatisticsController Stats;


        private void Start()
        {
            HideHappiness();
            // Get number of buildings from simulation
            numberOfTentsLeft = Simulation.NumberOfTents;
            numberOfWatertanksLeft = Simulation.NumberOfWatertanks;
            numberOfWashroomsLeft = Simulation.NumberOfWashrooms;
            
            // Update the UI
            UpdateButtonsState();
            // Start calculating happiness
            StartCoroutine(KeepHappinessUpToDate());

            GetComponent<StatisticsHolder>().Refugees = people;
            Stats.Statistics = GetComponent<StatisticsHolder>();
        }



        private void Update()
        {

            if(!spawned) {
                if(Input.GetKeyDown(KeyCode.Escape) && currentBuilding != null)
                    CancelCurrentBuilding();
                // Process building selection
                if(Input.GetMouseButtonDown(0) && currentBuilding == null) {
                    var dir = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit buildingHit;
                    bool collidedWithBuilding = Physics.Raycast(dir, out buildingHit, 1000, 1 << 10);
                    
                    if(collidedWithBuilding) {
                        currentBuilding = buildingHit.transform.gameObject;
                        currentBuilding.AddComponent<PlacementController>().Controller = this;

                    }
                }
            }
            finish.interactable = numberOfTentsLeft == 0 && numberOfWashroomsLeft == 0 && numberOfWatertanksLeft == 0;// && GetComponent<SimulationController>().Done; //Доступна ли кнопка финиш
            spawn.interactable = numberOfTentsLeft == 0 && numberOfWashroomsLeft == 0 && numberOfWatertanksLeft == 0;
        }



        private void FixedUpdate()
        {
            UpdateButtonsState();
        }



        private void HideHappiness() {
            /* col = happinesImage.color;
            col.a = 0f;
            happinesImage.color = col;
            */
            Color col = happinessResult.color;
            col.a = 0f;
            happinessResult.color = col;

            col = obstaclesResult.color;
            col.a = 0f;
            obstaclesResult.color = col;
        }



        private void ShowHappiness() {
            /*Color col = happinesImage.color;
            col.a = 100f;
            happinesImage.color = col;
            */
            Color col = happinessResult.color;
            col.a = 100f;
            happinessResult.color = col;

            col = obstaclesResult.color;
            col.a = 100f;
            obstaclesResult.color = col;
        }



        public void BuildCampOfDarkLord(Camp camp)
        {
            foreach(Tent tent in camp.GetTents())
            {
                Instantiate(tentPrefab, new Vector3(tent.X(), 0, tent.Y()), Quaternion.identity);
            }
            foreach (Water water in camp.GetWaters())
            {
                Instantiate(watertankPrefab, new Vector3(water.X(), 0, water.Y()), Quaternion.identity);
            }
            foreach (Toilet toilet in camp.GetToilets())
            {
                Instantiate(washroomPrefab, new Vector3(toilet.X(), 0, toilet.Y()), Quaternion.identity);
            }
        }

        public void OnClickBuildTent()
        {
            numberOfTentsLeft--;
            tents.Add(CreateBuilding(tentPrefab));
        }
        public void OnClickBuildWatertank()
        {
            numberOfWatertanksLeft--;
            watertanks.Add(CreateBuilding(watertankPrefab));
        }
        public void OnClickBuildWashroom()
        {
            numberOfWashroomsLeft--;
            washrooms.Add(CreateBuilding(washroomPrefab));
        }

        private GameObject CreateBuilding(GameObject prefab)
        {
            currentBuilding = Instantiate(prefab);
            currentBuilding.AddComponent<PlacementController>().Controller = this; // BuildingController у PlacementController'a . Тот вызывает BuildingController по щелчку ЛКМ. 
            return currentBuilding;
        }
        private void CancelCurrentBuilding()
        {
            switch(currentBuilding.tag)
            {
                case "Tent":
                    numberOfTentsLeft++;
                    tents.Remove(currentBuilding);
                    break;
                case "Watertank":
                    numberOfWatertanksLeft++;
                    watertanks.Remove(currentBuilding);
                    break;
                case "Washroom":
                    numberOfWashroomsLeft++;
                    washrooms.Remove(currentBuilding);
                    break;
            }
            Destroy(currentBuilding);
        }

        private int CalculateHappiness()
        {
            Camp currentCamp = Simulation.GetEmptyCamp();
            int tentCapacity = Simulation.TentCapacity;
            // Set tents
            List<Tent> tentsForLord = new List<Tent>();
            foreach(GameObject tent in tents)
            {
                Vector2 position = new Vector3(tent.transform.position.x, tent.transform.position.z);
                tentsForLord.Add(new Tent(position, tentCapacity));
            }
            currentCamp.SetTents(tentsForLord);
            // Set watertanks
            List<Water> watertanksForLord = new List<Water>();
            foreach(GameObject watertank in watertanks)
            {
                watertanksForLord.Add(new Water((int)watertank.transform.position.x, (int)watertank.transform.position.z));
            }
            currentCamp.SetWaters(watertanksForLord);
            // Set washrooms
            List<Toilet> washroomsForLord = new List<Toilet>();
            foreach(GameObject washroom in washrooms)
            {
                Vector2 position = new Vector3(washroom.transform.position.x, washroom.transform.position.z);
                washroomsForLord.Add(new Toilet(position));
            }
            currentCamp.SetToilets(washroomsForLord);

            // Set water per person
            currentCamp.SetWaterPerPersonForDrink((int)UI.WaterPerPerson);
            return currentCamp.GetHappiness();
        }

        public void BuildingCallback()
        {
            currentBuilding = null;
        }

        private void UpdateButtonsState()
        {
            buildTentButton.GetComponentInChildren<Text>().text = numberOfTentsLeft.ToString();
            buildWatertankButton.GetComponentInChildren<Text>().text = numberOfWatertanksLeft.ToString();
            buildWashroomButton.GetComponentInChildren<Text>().text = numberOfWashroomsLeft.ToString();

            buildTentButton.interactable = !(numberOfTentsLeft == 0 || currentBuilding != null);
            buildWatertankButton.interactable = !(numberOfWatertanksLeft == 0 || currentBuilding != null);
            buildWashroomButton.interactable = !(numberOfWashroomsLeft == 0 || currentBuilding != null);
        }

        private IEnumerator KeepHappinessUpToDate()
        {
            while(true)
            {
                happinessText.text = CalculateHappiness().ToString() + "%";
                yield return new WaitForEndOfFrame();
            }
        }

        public void OnClickFinish() {
            int happiness = NewCalculateHappiness(); //CalculateHappiness();
            var obs = CalculateObstacles();
            happiness -=  obs; 

            happiness = happiness < 0 ? 0 : happiness;
            obs = obs > 100 ? 100 : obs;
            
            happinessResult.text = "Happiness: " + happiness.ToString() + "%";
            obstaclesResult.text = "Decreased by obstacles: " + obs + "%";

            ShowHappiness();
        }

        public void OnClickDump()
        {
            string path = Application.dataPath + "/../SavedMap_" + System.DateTime.Now.ToBinary().ToString() + ".rc";
            BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create));
            Camp bestCamp = GetComponent<SimulationController>().BestCamp;
            writer.Write(CalculateHappiness());
            // Player's camp
            writer.Write(tents.Count);
            foreach(GameObject tent in tents)
            {
                writer.Write((int)tent.transform.position.x);
                writer.Write((int)tent.transform.position.z);
            }

            writer.Write(watertanks.Count);
            foreach (GameObject watertank in watertanks)
            {
                writer.Write((int)watertank.transform.position.x);
                writer.Write((int)watertank.transform.position.z);
            }

            writer.Write(washrooms.Count);
            foreach (GameObject washroom in washrooms)
            {
                writer.Write((int)washroom.transform.position.x);
                writer.Write((int)washroom.transform.position.z);
            }
            // AI's camp
            writer.Write(bestCamp.GetHappiness());
            foreach(Tent tent in bestCamp.GetTents())
            {
                writer.Write(tent.X());
                writer.Write(tent.Y());
            }

            foreach (Water water in bestCamp.GetWaters())
            {
                writer.Write(water.X());
                writer.Write(water.Y());
            }

            foreach (Toilet toilet in bestCamp.GetToilets())
            {
                writer.Write(toilet.X());
                writer.Write(toilet.Y());
            }
            writer.Write(Settings.WaterHeight);
            writer.Close();
            Application.Quit();
        }

        public List<GameObject> Tents { get { return tents; } }
        public List<GameObject> Watertanks { get { return watertanks; } }
        public List<GameObject> Washrooms { get { return washrooms; } }

        private int CellDist(Vector3 o,Vector3 d) {
            return (Math.Abs((int)d.x - (int)o.x) + Math.Abs((int)d.z - (int)o.z)) / 2 + 1;
        }

        private float FlatDistance(Vector3 v1,Vector3 v2) {
            return (float)Math.Sqrt(Math.Pow(Math.Abs(v2.x - v1.x), 2) + Math.Pow(Math.Abs(v2.z- v1.z), 2));
        }

        private float FullDistance(List<Vector3> list) {
            float distance = 0.0f;
            for(int i = 0;i<list.Count-1;++i)
                distance += FlatDistance(list[i],list[i+1]);
            return distance;
        }

        private int CalculateObstacles() {
            float obstacles = 0.0f;
            foreach (var tent in Tents) {
                foreach (var washroom in Washrooms) {
                    obstacles += ObstaclesBetween(tent.transform.position,washroom.transform.position);
                }
                foreach (var watertank in Watertanks) {
                    obstacles += ObstaclesBetween(tent.transform.position,watertank.transform.position);
                }
            }
            return (int)obstacles;
        }

        //private int CalcWCObstacles() {
        //    int obstacle = 0;
        //    var graph = Settings.MapGraph;
        //    foreach (var tent in Tents) {
        //        foreach (var washroom in Washrooms) {
        //            var tmp1 = FlatDistance(tent.transform.position,washroom.transform.position);//graph.FindRoute(tent.transform.position,washroom.transform.position).Count;
        //            var tmp2 = FullDistance(graph.FindRoute(tent.transform.position,washroom.transform.position));//CellDist(tent.transform.position,washroom.transform.position);
        //            if(Math.Abs(tmp1 - tmp2) > 2.0f)
        //                obstacle++;
        //        }
        //    }

        //    return 50 * (obstacle/(washrooms.Count*tents.Count));
        //}

        //private int CalcWaterObstacles() {
        //    int obstacle = 0;
        //    var graph = Settings.MapGraph;
        //    foreach(var tent in Tents) {
        //        foreach(var watertank in Watertanks) {
        //            var tmp1 = FlatDistance(tent.transform.position,watertank.transform.position);
        //            var tmp2 = FullDistance(graph.FindRoute(tent.transform.position,watertank.transform.position));
        //            if(Math.Abs(tmp1 - tmp2) > 2.0f)
        //                obstacle++;
        //        }
        //    }

        //    return 50 * (obstacle/(Watertanks.Count*tents.Count));
        //}

        private float ObstaclesBetween(Vector3 a,Vector3 b) {
            var graph = Settings.MapGraph;

            float slopes = 0.0f;
            int rocks = 0;

            var route = graph.FindFlatRoute(a,b);

            for(int i = 0;i<route.Count-1;++i) {
                slopes += Mathf.Abs(route[i].y - route[i+1].y);
                if(Settings.isRock(route[i]))
                    rocks++;
            }
            if(Settings.isRock(route[route.Count-1]))
                rocks++;

            var dslope = Settings.SlopeDecrease * (slopes/Settings.SlopeHeight);
            var drocks = Settings.RockDecrease * rocks;

            var result = dslope + drocks;

            return result;
        }



        public void OnClickSpawnPeople() {
            if(spawned) {
                DeletePeople();
                spawned = false;
            } else {
                SpawnPeople();
                spawned = true;
            }
        }

        private void SpawnPeople() {
            Random rnd = new Random();
            var SizeX = Settings.MapSizeX;
            var SizeZ = Settings.MapSizeY;

            for(int i = 0; i < Settings.AmountOfPeople; ++i) {
                var tent = tents[i % tents.Count];
                Transform t = tent.transform;
                var human = Instantiate(peoplePrefab);
                human.transform.position = t.position + t.right + t.forward;

                var rc = human.GetComponent<RefugeeController>();
                rc.Sources = gameObject.GetComponent<SourceManager>();
                people.Add(rc);
            }

        }

        private void DeletePeople() {           //Soooo cruel
            foreach (var person in people) {
                Destroy(person);                
            }
            people.Clear();
        }

        private int NewCalculateHappiness() {
            float tentDistance      = 0.0f;  
            float watertankDistance = 0.0f;
            float WCDistance        = 0.0f;
            float WTtoWCDistance    = 0.0f;

            for(int i = 0; i < tents.Count - 1; ++i) {
                watertankDistance += ClosestWT(tents[i].transform.position);
                for(int j = i + 1; j < tents.Count; ++j)
                    tentDistance += FlatDistance(tents[i].transform.position,tents[j].transform.position);
            }
            watertankDistance += ClosestWT(tents[tents.Count-1].transform.position);

            watertankDistance /= tents.Count;
            tentDistance /= tents.Count;

            for(int i = 0;i<washrooms.Count;++i) {
                for(int k = 0;k<watertanks.Count;++k)
                    WTtoWCDistance += FlatDistance(watertanks[k].transform.position,washrooms[i].transform.position);

                foreach (var tent in tents)
                    WCDistance += FlatDistance(washrooms[i].transform.position,tent.transform.position);
            }
            WCDistance /= tents.Count*washrooms.Count;
            WTtoWCDistance /= watertanks.Count*washrooms.Count;

            int tentHappiness   = (int)(tentDistance / Settings.TentsThreshold * 100.0f);
            int wtHappiness     = (int)(Settings.WaterThreshold / watertankDistance * 100.0f);
            int wcHappiness     = (int)(WCDistance / Settings.ToiletThreshold * 100.0f);
            int wtTowcHappiness = (int)(WTtoWCDistance / Settings.ToiletToWaterThreshold * 100.0f);

            if(tentHappiness > 100)
                tentHappiness = 100;
            if(wtHappiness > 100)
                wtHappiness = 100;
            if(wcHappiness > 100)
                wcHappiness = 100;
            if(wtTowcHappiness > 100)
                wtTowcHappiness = 100;

            int result = (int)(tentHappiness * Settings.TentsPrior + wtHappiness * Settings.WaterPrior + wcHappiness * Settings.ToiletPrior + wtTowcHappiness * Settings.ToiletToWaterPrior);
            if(result > 100)
                result = 100;

            return result;
        }

        private float ClosestWT(Vector3 tentPosition) {
            MinHeap<float> heap = new MinHeap<float>();
            foreach (var wt in watertanks) {
                heap.Add(FlatDistance(tentPosition, wt.transform.position));
            }
            return heap.Peek();
        }



        private List<RefugeeController> people = new List<RefugeeController>();
        
        // Prefabs
        [SerializeField]
        private GameObject tentPrefab;
        [SerializeField]
        private GameObject watertankPrefab;
        [SerializeField]
        private GameObject washroomPrefab;
        // UI fields
        [SerializeField]
        private Button buildTentButton;
        [SerializeField]
        private Button buildWatertankButton;
        [SerializeField]
        private Button buildWashroomButton;
        [SerializeField]
        private Text happinessText;
        // Number of buildings left to build
        private int numberOfTentsLeft;
        private int numberOfWatertanksLeft;
        private int numberOfWashroomsLeft;
        // Currently selected building
        private GameObject currentBuilding;

        [SerializeField]
        private Button finish;
        [SerializeField]
        private Button spawn;

        //additional
        //[SerializeField]
        //private Image happinesImage;
        [SerializeField]
        private Text happinessResult;
        [SerializeField]
        private Text obstaclesResult;

        [SerializeField]
        private GameObject peoplePrefab;

        private bool spawned = false;
    }
}