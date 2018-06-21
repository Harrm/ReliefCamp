using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Refugee.Genetic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Refugee.Misc;
using UnityStandardAssets.Water;
using Object = UnityEngine.Object;


namespace Refugee.Controllers {

	public class TerrainEditingController:MonoBehaviour {

		[SerializeField]
		private GameObject Map;
	    [SerializeField]
	    private GameObject WaterObject;
        [SerializeField]
		private GameObject rockPrefab;
		[SerializeField]
		private Button createRockButton;
		[SerializeField]
		private Text speedText;
        //[SerializeField]					//diff colors
        //private Material flatMaterial;
        //[SerializeField]
        //private Material mountainMaterial;

        [SerializeField]
        private Text WaterHeightText;
        [SerializeField]
        private Text WaterRadiusText;

        private int WaterRadius = 0;

		private GameObject currentObject;
		private List<GameObject> rocks;


	    private bool raiseTerrain = false;
	    private bool lowerTerrain = false;
	    private bool raiseWater = false;
	    //private bool lowerWater = false;
        private float changeSpeed = 5.0f;
        private Tools selectedTool = Tools.Surface;

        private enum Tools {Surface, Water}
		// Use this for initialization
		void Start() {

            if(Settings.Load(Settings.LoadPath)) {
                LoadTerrain();
                LoadWater();
            }

            SetUpCamera();
			rocks = new List<GameObject>();
            //Load();

			//** different heights - different colours **\\
			//Mesh map = Map.GetComponent<MeshFilter>().mesh;
			//map.subMeshCount = 1;
			//Map.GetComponent<Renderer>().materials = new[] { mountainMaterial,mountainMaterial };
			//Map.GetComponent<Renderer>().material.mainTexture = Resources.Load("mount") as Texture2D;
		}

        void SetUpCamera() {
            Camera.main.transform.position += new Vector3(Settings.MapSizeX/2 ,0 , 0);
        }

		// Update is called once per frame
		void Update() {
			UpdateUI();

		    if(Input.GetKeyDown(KeyCode.Escape) && currentObject != null) {
		        CancelPlacement();
		    }

            if(selectedTool == Tools.Surface) {
                if(Input.GetMouseButtonDown(0) && currentObject == null) {
                    RaycastHit hit;
                    if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,1000,1 << 10)) { // 1 - индекс layer'а Obstacle - луч игнорит все, кроме препятсвий.
                        currentObject = hit.transform.gameObject;                                               // Move that obstacle
                        currentObject.AddComponent<RocksPlacementController>().Controller = this;
                    } else if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,1000,1 << 8)) {
                        raiseTerrain = true;
                    }
                }

                if(Input.GetMouseButtonUp(0)  && currentObject == null)
                    raiseTerrain = false;

                if(Input.GetMouseButtonDown(1) && currentObject == null)
                    lowerTerrain = true;
                if(Input.GetMouseButtonUp(1)  && currentObject == null)
                    lowerTerrain = false;

                if(raiseTerrain)
                    ChangeTerrain(changeSpeed * Time.deltaTime);
                else if(lowerTerrain)
                    ChangeTerrain(-changeSpeed * Time.deltaTime);

            }
		}


		private void ChangeTerrain(float delta) {
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,1000,1 << 8)) {   //Опускаем луч на землю - находим точку клика
				Vector3 pos = hit.point;                                //Сама точка

				Mesh map = Map.GetComponent<MeshFilter>().mesh;         //Достаем меш земли


				int x = (int)pos.x;                             //Получаем координаты квадрата в меше
				int z = (int)pos.z;                             //на который кликнули

				int sizeX = Map.GetComponent<MapGenerator>().SizeX;  //mb I can get it from Settings. ?
				int sizeZ = Map.GetComponent<MapGenerator>().SizeZ;

				var nverts = (map.vertices);                    //Получаем массив вершин	1---2
																//Изменяем высоту вершин	| / |
				nverts[x + ((z + 1) * (sizeX + 1))].y += delta; //Вершина 3					3---4
				nverts[x + 1 + (z * (sizeX + 1))].y += delta;   //Вершина 2

				//if (pos.x - (int)pos.x > 0)                     //Находим нужный треугольник в квадрате
					nverts[x + (z * (sizeX + 1))].y += delta;   //Индекс вершины 1 
				//else
					nverts[x + 1 + ((z + 1) * (sizeX + 1))].y += delta; //Вершина 4


				map.SetVertices(new List<Vector3>(nverts));     //Устанавливаем измененные вершины и пересобираем меш


				var verts = new List<int>();
				//var indx1 = new List<int>();
				//var indx2 = new List<int>();

				//indx2.AddRange(new[] { 0,0,0 });  //need tests and fixes

				for (int i = 0;i < sizeZ;i++) {
					for (int j = 0;j < sizeX;j++) {
						int v1 = j + (i * (sizeX + 1));
						int v2 = j + 1 + (i * (sizeX + 1));
						int v3 = j + ((i + 1) * (sizeX + 1));
						int v4 = j + 1 + ((i + 1) * (sizeX + 1));

						verts.AddRange(new[] { v3,v2,v1,v3,v4,v2 });

						/*
						if (nverts[v2].y > 1 || nverts[v3].y > 1) {
							indx2.AddRange(new[] { v3,v2,v1,v3,v4,v2 });
						} else {
							if (nverts[v1].y > 1) 
								indx2.AddRange(new[] { v3,v2,v1 });
							else
								indx1.AddRange(new[] { v3,v2,v1 });

							if (nverts[v4].y > 1) 
								indx2.AddRange(new[] { v3,v4,v2 }); //carefully
							else 
								indx1.AddRange(new[] { v3,v4,v2 });
						
						}
						*/
						//}
						//}

						/*
						if (indx2.Count>0)
							map.subMeshCount=2;
						else
							map.subMeshCount=1;

						map.SetTriangles(indx1, 0);
						map.SetTriangles(indx2, 1);
						//*/

					}
				}
				map.SetTriangles(verts,0);
				map.RecalculateBounds();
				map.RecalculateNormals();

				Object.Destroy(Map.GetComponent<MeshCollider>()); //Пересобираем коллайдер
				Map.AddComponent<MeshCollider>();
			}
		}


	    private void RaiseWater(float height) {
			WaterObject.transform.position += Vector3.up * height;
			Settings.WaterHeight += height;
	    }



        public void OnClickCreateRock() {
			currentObject = Instantiate(rockPrefab);
			currentObject.AddComponent<RocksPlacementController>().Controller = this;
			rocks.Add(currentObject);
			raiseTerrain = false;
		}

		public void PlacementCallback() {
			currentObject = null;
		}

		private void CancelPlacement() {
			rocks.Remove(currentObject);
			Destroy(currentObject);
		}

		private void UpdateUI() {
			createRockButton.GetComponentInChildren<Text>().text = rocks.Count.ToString();
			speedText.text = changeSpeed.ToString("0.0");
            WaterHeightText.text = Settings.WaterHeight.ToString("0.0");
		}

        private void SetSettings() {
            Settings.MapVertices = new List<Vector3>(Map.GetComponent<MeshFilter>().mesh.vertices);

            Settings.MapUVs = (Vector2[])Map.GetComponent<MeshFilter>().mesh.uv.Clone();

            var rocksCoordinates = new List<Vector3>();
            foreach(var rock in rocks) {
                rocksCoordinates.Add(rock.transform.position);
            }

            Settings.RocksCoords = rocksCoordinates;

            CreateGraph();

            Settings.CameraPosition = Camera.main.transform.position;

            Settings.WaterVertices = new List<Vector3>(WaterObject.GetComponent<MeshFilter>().mesh.vertices);
            Settings.WaterUVs = (Vector2[])WaterObject.GetComponent<MeshFilter>().mesh.uv.Clone();
        }

		public void OnClickNextButton() {
			SetSettings();

            SceneManager.LoadScene("Main");
		}

		public void OnClickPlusButton() {
			changeSpeed += 0.5f;
			raiseTerrain = false;
		}
		public void OnClickMinusButton() {
			changeSpeed -= 0.5f;
			/*if (changeSpeed < 0)
				changeSpeed = 0;*/
			raiseTerrain = false;
		}

        private void CreateGraph() {
            int sizeX = Settings.MapSizeX + 1;
            int sizeZ = Settings.MapSizeY + 1;

            Settings.MapGraph = new Graph();
            var verts = Map.GetComponent<MeshFilter>().mesh.vertices;

            int idx = 0;
            var arr = new Vector3[sizeX, sizeZ];
            bool[,] isInGraph = new bool[sizeX, sizeZ];
            for(int i = 0; i < sizeX; ++i) {
                for(int j = 0; j < sizeZ; ++j) {
                	var v = verts[idx];
            		var notRock = !Settings.isRock(arr[i,j]);
                    var notWater = v.y > Settings.WaterHeight;
                    if(notRock && notWater) {
	                    arr[i, j] = v;
	                    Settings.MapGraph.AddVertex(v);
	                    isInGraph[i, j] = true;
                    } else {
                    	isInGraph[i, j] = false;
                    }
                    idx++;
                }
            }

            // edge ones are removed because people fall off 
            for(int i = 1; i < sizeX - 1; ++i) {
                for(int j = 1; j < sizeZ - 1; ++j) {
                    for(int ki = -1; ki < 2; ++ki)        //TODO: не просматривать вершины левее
                        for(int kj = -1; kj < 2; ++kj) {
                            int li = i+ki;
                            int lj = j+kj;
                            if (li < 0 || lj < 0 || li >= sizeX || lj >= sizeZ ||
                            	!isInGraph[i, j] || !isInGraph[li, lj])
                            	continue;
                            var properHeightDiff = Mathf.Abs(arr[i,j].y - arr[li,lj].y) < Settings.MaxHeightDifference;
                            if ((ki != 0 || kj != 0) && properHeightDiff) {
                                Settings.MapGraph.AddEdge(arr[i,j], arr[li,lj]);
                            }
                        }
                }
            }
        }

	    private void LoadTerrain() {
	        //foreach (var rock in rocks) {
	        //    Object.Destroy(rock);
	        //}

	        int sizeX = Settings.MapSizeX;
	        int sizeZ = Settings.MapSizeY;

            //sizeZ = 150;
            //sizeX = 150;
            // Mesh for the map
            var mesh = new Mesh();
	        // Setting vertices
	        var vertices = new List<Vector3>(Settings.MapVertices);

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
	        mesh.uv = Settings.MapUVs;

	        mesh.RecalculateBounds();  //Постройка меша
	        mesh.RecalculateNormals(); //Постройка меша
	        // Setting mesh to mesh renderers
	        Map.GetComponent<MeshFilter>().mesh = mesh;

            // Creating collider
	        Object.Destroy(Map.GetComponent<MeshCollider>()); //Пересобираем коллайдер
	        Map.AddComponent<MeshCollider>();
            // Scaling the basement
            //basement.localScale = new Vector3(sizeX + 1,1,sizeZ + 1);
            //basement.position = new Vector3(sizeX / 2f,-0.52f,sizeZ / 2f);

            //Setting rocks
            foreach(var coord in Settings.RocksCoords) {
	            var rock = Instantiate(rockPrefab);
	            rock.transform.position = coord;
	        }

	    }

	    private void LoadWater() {

	        int sizeX = Settings.MapSizeX;
	        int sizeZ = Settings.MapSizeY;

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
	        Object.Destroy(WaterObject.GetComponent<MeshCollider>()); //Пересобираем коллайдер
	        WaterObject.AddComponent<MeshCollider>();
            WaterObject.AddComponent<WaterBasic>();

            WaterObject.transform.Translate(Settings.WaterHeight*Vector3.up);
	    }

        public void Load() {
            //Settings.Load("TestSave.xml");
            LoadTerrain();
            LoadWater();
        }

        public List<Vector2> Area(int x, int y, int r) {
            List<Vector2> points = new List<Vector2>();
            for(int i = -r;i <= r;++i) {
                points.Add(new Vector2( x+i , y ));
                for(int j = 1; j <= r - Math.Abs(i); ++j) {
                    points.Add(new Vector2( x+i , y+j ));
                    points.Add(new Vector2( x+i , y-j ));
                }
            }

            return points;
        }


        public void OnSurfaceToolClick() {
            selectedTool = Tools.Surface;
        }
        public void OnWaterToolClick() {
            selectedTool = Tools.Water;
        }

        public void OnClickWaterHeightPlus() {
            RaiseWater(0.5f);
        }
        public void OnClickWaterHeightMinus() {
            RaiseWater(-0.5f);
        }

        public void OnClickWaterRadiusPlus() {
            WaterRadius += 1;
        }
        public void OnClickWaterRadiusMinus() {
            WaterRadius -= 1;
        }

        public void OnClickSaveButton() {
            SetSettings();
            Settings.Save("save.savf");
        }
    }
}