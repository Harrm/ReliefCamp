using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Refugee.Genetic;

namespace Refugee.Controllers
{
    public class UIController: MonoBehaviour
    {
        [SerializeField]
        private Text populationText;
        [SerializeField]
        private Text waterTotal;
        [SerializeField]
        private Text waterPerPersonText;

        public float WaterPerPerson { get; set; }

        private void Start()
        {
            var simulation = GameObject.Find("Simulation").GetComponent<SimulationController>();
            WaterPerPerson = 3.0f;
            populationText.text = simulation.NumberOfPeople.ToString();
            waterPerPersonText.text = string.Format("{0:0.0}", WaterPerPerson);
            waterTotal.text = "Total - " + simulation.AmountOfWater.ToString();
        }

        public void OnClickPlus()
        {
            WaterPerPerson += 0.100000f;
            waterPerPersonText.text = string.Format("{0:0.0}", WaterPerPerson);
        }
        public void OnClickMinus()
        {
            WaterPerPerson -= 0.100000f;
            WaterPerPerson = Mathf.Clamp(WaterPerPerson, 0, float.MaxValue);
            waterPerPersonText.text = string.Format("{0:0.0}", WaterPerPerson);
        }
    }
}