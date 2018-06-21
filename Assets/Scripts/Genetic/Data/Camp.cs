using System;
using System.Collections.Generic;
using UnityEngine;

namespace Refugee.Genetic.Data
{
    public class Camp : IComparable
    {
        private List<Tent> tents = new List<Tent>();
        private List<Water> waters = new List<Water>();
        private List<Toilet> toilets = new List<Toilet>();

        private int people;
        private int allWater;
        private Happiness happiness = new Happiness();
        private Vector2 size;

        private int waterPerPersonForDrink;
        //private int waterPerPersonForFood;
        //private int waterPerPersonForHyhiene;

        public Camp(int water, int waterPerPersonForDrink,/* int waterPerPersonForFood, int waterPerPersonForHyhiene,*/ int people, Vector2 size)
        {
            this.size = size;
            this.waterPerPersonForDrink = waterPerPersonForDrink;
            //this.waterPerPersonForFood = waterPerPersonForFood;
            //this.waterPerPersonForHyhiene = waterPerPersonForHyhiene;
            allWater = water;
            this.people = people;
        }
        public Camp(Camp p1, Camp p2)
        {
            System.Random gen = new System.Random();
            Camp[] ps = new Camp[2];
            ps[0] = p1;
            ps[1] = p2;
            people = p1.people;
            happiness = p1.happiness;
            size = p1.GetSize();
            allWater = p1.GetAllWater();
            tents = ps[gen.Next(2)].tents;
            waters = ps[gen.Next(2)].waters;
            toilets = ps[gen.Next(2)].toilets;
            waterPerPersonForDrink = ps[gen.Next(2)].GetWaterPerPersonForDrink();
            //waterPerPersonForFood = ps[gen.Next(2)].GetWaterPerPersonForFood();
            //waterPerPersonForHyhiene = ps[gen.Next(2)].GetWaterPerPersonForHyhiene();
        }

        private int CalculateHappiness()
        {
            happiness.SetParameters(Settings.TentsThreshold, Settings.WaterThreshold, Settings.ToiletToWaterThreshold, Settings.ToiletThreshold, Settings.TentsPrior, Settings.WaterPrior, Settings.ToiletPrior, Settings.ToiletToWaterPrior);

            happiness.IncreaseHappy(tents, waters, toilets);
            happiness.IncreaseHappy(tents, people);
            int tmp = allWater;
            happiness.IncreaseHappy(waterPerPersonForDrink,/* waterPerPersonForFood, waterPerPersonForHyhiene,*/ people, allWater);
            allWater = tmp;
            return happiness.GetHappy();
        }

        public double GetAvrDist()
        {
            return happiness.GetAvrDistance();
        }
        public string GetAvrDistBetEach()
        {
            return happiness.GetDistance();
        }

        public List<Tent> GetTents()
        {
            return tents;
        }
        public void SetTents(List<Tent> tents)
        {
            this.tents = tents;
        }
        public List<Water> GetWaters()
        {
            return waters;
        }
        public void SetWaters(List<Water> waters)
        {
            this.waters = waters;
        }
        public List<Toilet> GetToilets()
        {
            return toilets;
        }
        public void SetToilets(List<Toilet> toilets)
        {
            this.toilets = toilets;
        }

        public int GetAllWater()
        {
            return allWater;
        }
        public int GetHappiness()
        {
            return CalculateHappiness();
        }
        public Vector2 GetSize()
        {
            return size;
        }

        public int GetWaterPerPersonForDrink()
        {
            return waterPerPersonForDrink;
        }
        public void SetWaterPerPersonForDrink(int waterPerPersonForDrink)
        {
            if (waterPerPersonForDrink == 0) waterPerPersonForDrink = 1;
            this.waterPerPersonForDrink = waterPerPersonForDrink;
        }

        public double AvrCountOfWaterPerPerson()
        {
            return happiness.GetAvrCountOfWaterThatTakeEachPerson();
        }

        /*
        public int GetWaterPerPersonForFood()
        {
            return waterPerPersonForFood;
        }
        public void SetWaterPerPersonForFood(int waterPerPersonForFood)
        {
            this.waterPerPersonForFood = waterPerPersonForFood;
        }
        public int GetWaterPerPersonForHyhiene()
        {
            return waterPerPersonForHyhiene;
        }
        public void SetWaterPerPersonForHyhiene(int waterPerPersonForHyhiene)
        {
            this.waterPerPersonForHyhiene = waterPerPersonForHyhiene;
        }
        */

        public int CompareTo(object obj)
        {
            if (obj is Camp)
            {
                var camp = (Camp)obj;
                return GetHappiness() - camp.GetHappiness();
            }
            else return -1;
        }
    }
}