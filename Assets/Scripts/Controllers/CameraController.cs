using UnityEngine;
using System.Collections;

namespace Refugee.Controllers
{
    public class CameraController : MonoBehaviour
    {
        float cameraSpeed = 15.0f;
        float scrollSpeed = 10.0f;
		float rotationSpeed = 5.0f;

        void Start()
        {
        }

        void Update()
        {
            // Camera movement
            float mousePosX = Input.mousePosition.x;
            float mousePosY = Input.mousePosition.y;

            Vector3 moveDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;
            if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;
            if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
            if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;

			/*if (Input.GetKey(KeyCode.Q))
				transform.Rotate(new Vector3(0,90,0));//transform.rotation = Quaternion.Euler(0,90,0);
			if (Input.GetKey(KeyCode.E))
				transform.Rotate(new Vector3(0,-90,0));//transform.rotation = Quaternion.Euler(0,-90,0);
			*/
			

			transform.Translate(moveDirection.normalized * cameraSpeed * Time.deltaTime);
            // Camera distance
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            transform.Translate(transform.GetChild(0).forward * scrollWheel * scrollSpeed);
        }
    }
}