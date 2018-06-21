using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityStandardAssets.Water;
using Random = System.Random;


namespace Refugee.Misc {

	public class MapSetup: MonoBehaviour {

        public int SizeX {
			get {
				return sizeX;
			}
		}
		public int SizeZ {
			get {
				return sizeZ;
			}
		}

		private void Awake() {
			sizeX = Settings.MapSizeX;
			sizeZ = Settings.MapSizeY;
		}
		
		private void Start() {
            
            //Settings.Save("TestSave.xml");

            SetUpCamera();
			SetUpTerrain();
			SetUpSources();
            SetUpWater();
            //DebugGraph();
		}

        private void SetUpCamera() {
            Camera.main.transform.position = Settings.CameraPosition;
        }

		private void SetUpTerrain() {
			//sizeZ = 150;
			//sizeX = 150;
			// Mesh for the map
			var mesh = new Mesh();
			// Setting vertices
			var vertices = new List<Vector3>(Settings.MapVertices);
			
			// Generating indices
			var indices = new List<int>();
			for (int z = 0;z < sizeZ;z++) {
				for (int x = 0;x < sizeX;x++) {
					// Fetching indices for the current square
					int v1 = x + (z * (sizeX + 1));
					int v2 = x + 1 + (z * (sizeX + 1));
					int v3 = x + ((z + 1) * (sizeX + 1));
					int v4 = x + 1 + ((z + 1) * (sizeX + 1));
					// Saving them in the right order
					indices.AddRange(new[] { v3,v2,v1,v3,v4,v2 });
				}
			}

			// Finilizing mesh
			mesh.SetVertices(vertices);   //Назначаем вершины
			mesh.SetTriangles(indices,0); //Назначаем треугольники - индексы вершин
			mesh.uv = Settings.MapUVs;

			mesh.RecalculateBounds();  //Постройка меша
			mesh.RecalculateNormals(); //Постройка меша
									   // Setting mesh to mesh renderers
			GetComponent<MeshFilter>().mesh = mesh;
			// Creating collider
			gameObject.AddComponent<MeshCollider>();
			// Scaling the basement
			//basement.localScale = new Vector3(sizeX + 1,1,sizeZ + 1);
			//basement.position = new Vector3(sizeX / 2f,-0.52f,sizeZ / 2f);

			//Setting rocks
			foreach (var coord in Settings.RocksCoords) {
				var rock = Instantiate(rockPrefab);
				rock.transform.position = coord;
			}
		}

		private void SetUpSources() {
			var buildingController = GetComponent<Refugee.Controllers.BuildingController>();
			var sourceManager = GetComponent<SourceManager>();
			sourceManager.Washrooms = buildingController.washrooms;
			sourceManager.Tents = buildingController.tents;
			sourceManager.WaterSources = buildingController.watertanks;
		}

	    private void SetUpWater() {
	        //sizeZ = 150;
	        //sizeX = 150;
	        // Mesh for the map
	        var mesh = new Mesh();
	        // Setting vertices
	        var vertices = new List<Vector3>(Settings.WaterVertices);

	        // Generating indices
	        var indices = new List<int>();
	        for(int z = 0;z < sizeZ;z++) {
	            for(int x = 0;x < sizeX;x++) {
	                // Fetching indices for the current square
	                int v1 = x + (z * (sizeX + 1));
	                int v2 = x + 1 + (z * (sizeX + 1));
	                int v3 = x + ((z + 1) * (sizeX + 1));
	                int v4 = x + 1 + ((z + 1) * (sizeX + 1));
	                // Saving them in the right order
	                indices.AddRange(new[] { v3,v2,v1,v3,v4,v2 });
	            }
	        }

	        // Finilizing mesh
	        mesh.SetVertices(vertices);   //Назначаем вершины
	        mesh.SetTriangles(indices,0); //Назначаем треугольники - индексы вершин
	        mesh.uv = Settings.WaterUVs;

	        mesh.RecalculateBounds();  //Постройка меша
	        mesh.RecalculateNormals(); //Постройка меша
            
	        // Setting mesh to mesh renderers
	        WaterObject.GetComponent<MeshFilter>().mesh = mesh;
            // Creating collider
	        WaterObject.gameObject.AddComponent<MeshCollider>();
            WaterObject.AddComponent<WaterBasic>();
            WaterObject.transform.Translate(Vector3.up * Settings.WaterHeight);

        }

        private void DebugGraph() {
            var addVector = new Vector3(0,0,0);
            var graph = Settings.MapGraph;
            foreach (var vertex in graph.Vertices) {
                foreach (var adjacentVertex in vertex.AdjacentVertices()) {
                    Debug.DrawLine(vertex.Coord+addVector, adjacentVertex.Coord + addVector, Color.yellow, 600, true);
                }
            }
        }

		[SerializeField]
		private Transform basement;
		[SerializeField]
		private GameObject rockPrefab;
		[SerializeField]
		private int sizeX;
		[SerializeField]
		private int sizeZ;
        //[SerializeField]
        //private GameObject peoplePrefab;
        [SerializeField]
        private GameObject vertexPrefab;
        [SerializeField]
        private GameObject WaterObject;
	}
}
