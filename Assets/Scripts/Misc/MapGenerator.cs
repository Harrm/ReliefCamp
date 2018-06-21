using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityStandardAssets.Water;

namespace Refugee.Misc
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        private Transform basement;
        [SerializeField]
        private int sizeX;
        [SerializeField]
        private int sizeZ;
        [SerializeField]
        private GameObject water;

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

		private void Awake()
        {
            sizeX = Settings.MapSizeX;
            sizeZ = Settings.MapSizeY;
        }
        private void Start() {
            if(!Settings.Load(Settings.LoadPath)) {
                GenerateTerrain();
                GenerateWater();
            }
            
                

			/*terrain.terrainData.size = new Vector3(sizeX,0,sizeZ);

			float[,] heights = new float[sizeX,sizeZ];
			for (int i = 0;i<sizeX;++i)
				for (int j = 0;j<sizeZ;++j)
					heights[i,j] = 0;//i*j*0.001f;
			
			terrain.terrainData.SetHeights(0,0, heights);*/
        }

		private void GenerateTerrain() {
			//sizeZ = 150;
			//sizeX = 150;
			// Mesh for the map
			var mesh = new Mesh();
			// Generating vertices
			var vertices = new List<Vector3>();
			var UV = new List<Vector2>();

			//float textSize = 5.0f;
			for (int z = 0;z <sizeZ + 1;z++) { //
				for (int x = 0;x <sizeX + 1;x++) { //
					
					vertices.Add(new Vector3(x,0,z)); //0 (float)rnd.NextDouble()%2
					float u = z / (float)(sizeZ+1);
					float v = x / (float)(sizeX+1);
					//float v = (x % textSize + 1.0f) / textSize;
					//float u = (z % textSize + 1.0f) / textSize;
					/*if (x%2==0)
						u=1;
					if (z%2==0)
						v=1;
					//*/
					UV.Add(new Vector2(u,v));
				}
			}
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
					indices.AddRange(new int[] { v3,v2,v1,v3,v4,v2 });
				}
			}

			Vector2[] arrUV = new Vector2[UV.Count];
			int i = 0;
			foreach (var v in UV) {
				arrUV[i++]=v;
			}

			// Finilizing mesh
			mesh.SetVertices(vertices);   //Назначаем вершины
			mesh.SetTriangles(indices,0); //Назначаем треугольники - индексы вершин

			mesh.uv = arrUV;

			mesh.RecalculateBounds();  //Постройка меша
			mesh.RecalculateNormals(); //Постройка меша
			// Setting mesh to mesh renderers
			GetComponent<MeshFilter>().mesh = mesh;
			// Creating collider
			gameObject.AddComponent<MeshCollider>();
			// Scaling the basement
			//basement.localScale = new Vector3(sizeX + 1,1,sizeZ + 1);
			//basement.position = new Vector3(sizeX / 2f,-0.52f,sizeZ / 2f);
		}


        private void GenerateWater() {
            var mesh = new Mesh();
            // Generating vertices
            var vertices = new List<Vector3>();
            var UV = new List<Vector2>();

            //float textSize = 5.0f;
            for(int z = 0;z <sizeZ + 1;z++) { //
                for(int x = 0;x <sizeX + 1;x++) { //

                    vertices.Add(new Vector3(x, 0, z)); //0 (float)rnd.NextDouble()%2
                    float u = z / (float)(sizeZ+1);
                    float v = x / (float)(sizeX+1);
                    UV.Add(new Vector2(u,v));
                }
            }
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
                    indices.AddRange(new int[] { v3,v2,v1,v3,v4,v2 });
                }
            }

            Vector2[] arrUV = new Vector2[UV.Count];
            int i = 0;
            foreach(var v in UV) {
                arrUV[i++]=v;
            }

            // Finilizing mesh
            mesh.SetVertices(vertices);   //Назначаем вершины
            mesh.SetTriangles(indices,0); //Назначаем треугольники - индексы вершин

            mesh.uv = arrUV;

            mesh.RecalculateBounds();  //Постройка меша
            mesh.RecalculateNormals(); //Постройка меша

            // Setting mesh to mesh renderers
            water.GetComponent<MeshFilter>().mesh = mesh;
            water.gameObject.AddComponent<MeshCollider>();
            water.AddComponent<WaterBasic>();
            water.transform.Translate(Settings.WaterHeight*Vector3.up);
        }


    }
}