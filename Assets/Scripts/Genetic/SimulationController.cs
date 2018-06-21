using Refugee.Genetic.Data;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Refugee.Genetic
{
    public class SimulationController : MonoBehaviour
    {
        // Input fields
        [SerializeField]
        private int amount;
        [SerializeField]
        private int amountOfWater;
        [SerializeField]
        private int numberOfWashrooms;
        [SerializeField]
        private int numberOfWatertanks;
        [SerializeField]
        private int numberOfTents;
        [SerializeField]
        private int numberOfPeople;
        [SerializeField]
        private int tentCapacity;
        [SerializeField]
        private int waterPerPersonToDrink;
        [SerializeField]
        private int mapSizeX;
        [SerializeField]
        private int mapSizeY;
        // Simulation thread
        private Thread simulationThread;
        // Output data
        Camp minCamp;
        Camp maxCamp;

        bool done;

        private void Awake()
        {

            //amount = Settings.AiRuns; // = 10
            amountOfWater = Settings.AmountOfWater;
            numberOfWashrooms = Settings.AmountOfWashrooms;
            numberOfWatertanks = Settings.AmountOfWatertanks;
            numberOfTents = Settings.AmountOfTents;
            numberOfPeople = Settings.AmountOfPeople;
            mapSizeX = Settings.MapSizeX;
            mapSizeY = Settings.MapSizeY;
        }
        private void Start()
        {
            // Start the simulation in background
            simulationThread = new Thread(Simulate);
            simulationThread.Priority = System.Threading.ThreadPriority.BelowNormal;
            simulationThread.Start();
        }
        private void Update()
        {
            if(done)
            {
                //GameObject.Find("Happiness Panel AI").GetComponentInChildren<Text>().text = maxCamp.GetHappiness().ToString() + "%";
            }
        }

        private void Simulate()
        {
            CrossOverManager crossOver = new CrossOverManager(amount, amountOfWater, numberOfWashrooms, numberOfWatertanks, numberOfTents, numberOfPeople, tentCapacity, mapSizeX, mapSizeY, waterPerPersonToDrink);
            int max = 0;
            int min = 100;
            minCamp = null;
            maxCamp = null;
            for (int i = 0; i < 50; i++)
            {
                Camp tmp = crossOver.Run(50);
                int happy = tmp.GetHappiness();
                if (max < happy)
                {
                    max = happy;
                    maxCamp = tmp;
                }
                if (min > happy && happy != 0)
                {
                    min = happy;
                    minCamp = tmp;
                }
            }
            Debug.Log("Done! Best result is " + maxCamp.GetHappiness());
            done = true;
        }

        public Camp GetEmptyCamp()
        {
            return new Camp(amountOfWater, waterPerPersonToDrink, numberOfPeople, new Vector2(mapSizeX, mapSizeY));
        }

        public int AmountOfWater { get { return amountOfWater; } set { amountOfWater = value; } }
        public int NumberOfWashrooms { get { return numberOfWashrooms; } set { NumberOfWashrooms = value; } }
        public int NumberOfWatertanks { get { return numberOfWatertanks; } set { numberOfWatertanks = value; } }
        public int NumberOfTents { get { return numberOfTents; } set { numberOfTents = value; } }
        public int NumberOfPeople { get { return numberOfPeople; } set { numberOfPeople = value; } }
        public int TentCapacity { get { return tentCapacity; } set { tentCapacity = value; } }
        public int MapSizeX { get { return mapSizeX; } set { mapSizeX = value; } }
        public int MapSizeY { get { return mapSizeY; } set { mapSizeY = value; } }

        public bool Done { get { return done; } }
        public Camp BestCamp { get { return maxCamp; } }
    }
}