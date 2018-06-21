using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Refugee.Controllers {
	public class RocksPlacementController:MonoBehaviour {

		public TerrainEditingController Controller;

		// Use this for initialization
		void Start() {
			GameObject.Find("TipDis").GetComponent<Text>().text = "Press ESC to cancel";
		}

		// Update is called once per frame
		void Update() {
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,1000,1 << 8)) //игнорит всё, кроме карты. 8 - индекс Map layout
			{
				// Get building position
				Vector3 position = hit.point;
				// Snap it to grid
				position.x = (int)(position.x); //Ставит объекты "по сетке"
				//position.y = (int)position.y; // 0;
				position.z = (int)(position.z);
				transform.position = position;
			}
			// Check for left mouse click
			if (Input.GetMouseButtonDown(0)) //Кликнул - установил
			{
				Controller.PlacementCallback();
				Destroy(this);
			}
		}

		private void OnDestroy() {
			GameObject.Find("TipDis").GetComponent<Text>().text = "";
		}
	}
}
