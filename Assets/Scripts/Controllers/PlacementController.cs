using Refugee.Misc;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Refugee.Controllers
{
    public class PlacementController: MonoBehaviour
    {
        public BuildingController Controller { get; set; }

        private List<GameObject> lines = new List<GameObject>();
        private List<GameObject> distances = new List<GameObject>();

        private bool showThem;

        public void SpawnDistances()
        {
            // SUPERMASIIVEBADCODE
            GameObject linePrefab = Resources.Load<GameObject>("Line Prefab");
            GameObject distancePrefab = Resources.Load<GameObject>("Distance Prefab");
            BuildingController controller = GameObject.Find("Map").GetComponent<BuildingController>();
            foreach(GameObject tent in controller.Tents)
            {
                if (tent == gameObject) continue;
                GameObject newLine = Instantiate(linePrefab);
                newLine.GetComponent<LineRenderer>().SetPosition(0, transform.position + new Vector3(0.5f, 0.5f, 0.5f));
                newLine.GetComponent<LineRenderer>().SetPosition(1, tent.transform.position + new Vector3(0.5f, 0.5f, 0.5f));
                newLine.GetComponent<LineRenderer>().SetColors(new Color(0.8f, 0.8f, 0.8f), new Color(0.8f, 0.8f, 0.8f));
                lines.Add(newLine);
                GameObject newDistance = (GameObject)Instantiate(distancePrefab, GameObject.Find("UI").transform);
                newDistance.AddComponent<DistanceStorage>().EndPoint = tent.transform.position;
                newDistance.GetComponent<Text>().color = new Color(0.8f, 0.8f, 0.8f);
                distances.Add(newDistance);
            }
            foreach (GameObject watertank in controller.Watertanks)
            {
                if (watertank == gameObject) continue;
                GameObject newLine = Instantiate(linePrefab);
                newLine.GetComponent<LineRenderer>().SetPosition(0, transform.position + new Vector3(0.5f, 0.5f, 0.5f));
                newLine.GetComponent<LineRenderer>().SetPosition(1, watertank.transform.position + new Vector3(0.5f, 0.5f, 0.5f));
                newLine.GetComponent<LineRenderer>().SetColors(new Color(0, 0, 0.8f), new Color(0, 0, 0.8f));
                lines.Add(newLine);
                GameObject newDistance = (GameObject)Instantiate(distancePrefab,GameObject.Find("UI").transform);
                newDistance.AddComponent<DistanceStorage>().EndPoint = watertank.transform.position;
                newDistance.GetComponent<Text>().color = new Color(0, 0, 0.8f);
                distances.Add(newDistance);
            }
            foreach (GameObject washroom in controller.Washrooms)
            {
                if (washroom == gameObject) continue;
                GameObject newLine = Instantiate(linePrefab);
                newLine.GetComponent<LineRenderer>().SetPosition(0, transform.position + new Vector3(0.5f, 0.5f, 0.5f));
                newLine.GetComponent<LineRenderer>().SetPosition(1, washroom.transform.position + new Vector3(0.5f, 0.5f, 0.5f));
                newLine.GetComponent<LineRenderer>().SetColors(new Color(0, 0.8f, 0), new Color(0, 0.8f, 0));
                lines.Add(newLine);
                GameObject newDistance = (GameObject)Instantiate(distancePrefab, GameObject.Find("UI").transform);
                newDistance.AddComponent<DistanceStorage>().EndPoint = washroom.transform.position;
                newDistance.GetComponent<Text>().color = new Color(0, 0.8f, 0);
                distances.Add(newDistance);
            }
            SetTheirState(false);
        }

        private void UpdateDistances()
        {
            foreach(GameObject line in lines)
            {
                line.GetComponent<LineRenderer>().SetPosition(0, transform.position + new Vector3(0.5f, 0.5f, 0.5f));
            }
            foreach(GameObject distance in distances)
            {
                int distanceValue = (int)Vector3.Distance(transform.position, distance.GetComponent<DistanceStorage>().EndPoint);
                distance.GetComponent<Text>().text = distanceValue.ToString() + "m";
                distance.transform.position = Camera.main.WorldToScreenPoint(Vector3.Lerp(transform.position + new Vector3(0.5f, 0.5f, 0.5f), distance.GetComponent<DistanceStorage>().EndPoint + new Vector3(0.5f, 0.5f, 0.5f), 0.5f));
            }
        }

        private void Start()
        {
            SpawnDistances();
            GameObject.Find("TipDis").GetComponent<Text>().text = "Hold Right Mouse Button to view more information";
        }
        private void Update()
        {
            // Input
            if(Input.GetMouseButtonDown(1))
            {
                showThem = true;
                SetTheirState(true);
            }
            if(Input.GetMouseButtonUp(1))
            {
                showThem = false;
                SetTheirState(false);
            }
            // LINES
            if (showThem)
                UpdateDistances();
            // Update building position
            
            var dir = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit groundHit;
            bool collidedWithGround = Physics.Raycast(dir, out groundHit, 1000, 1 << 9);
            RaycastHit waterHit;
            bool collidedWithWater = Physics.Raycast(dir, out waterHit, 1000, 1 << 4);

            bool feasiblePlace = false;

            if(collidedWithGround && (!collidedWithWater || groundHit.point.y > waterHit.point.y)) {
    			feasiblePlace = true;
                transform.position = groundHit.point;
            }
            // Check for left mouse click
            if(Input.GetMouseButtonDown(0) && feasiblePlace) //Кликнул - установил
            {
                Controller.BuildingCallback();
                Destroy(this);
            }
        }

        private void SetTheirState(bool state)
        {
            for(int i = 0; i < lines.Count; i++)
            {
                lines[i].SetActive(state);
                distances[i].SetActive(state);
            }
        }

        private void OnDestroy()
        {
            for(int i = 0; i < lines.Count; i++)
            {
                Destroy(lines[i]);
                Destroy(distances[i]);
            }
            lines.Clear();
            distances.Clear();
            GameObject.Find("TipDis").GetComponent<Text>().text = "";
        }
    }
}