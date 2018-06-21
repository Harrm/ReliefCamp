using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Refugee.Misc {
    class PersonsLabel:MonoBehaviour {
        
        public GameObject person;
 
        void Update() {
            //var wantedPos = Camera.main.WorldToViewportPoint(person.transform.position);
            transform.position = person.transform.position+new Vector3(0,1.5f,0);
            RotatePls();
        }

        void RotatePls() {
            var destination = Camera.main.transform.position;
            float angle = Vector3.Angle(transform.forward,destination) * Time.deltaTime;
            Quaternion rot = Quaternion.LookRotation(destination - transform.position);
            transform.rotation = rot;//Quaternion.Slerp(transform.rotation, rot, rotSpeed*Time.deltaTime / angle); //rot * Time.deltaTime;
            transform.Rotate(transform.up,angle);
        }
    }
}
