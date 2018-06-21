using System;
using System.Collections.Generic;

namespace Refugee.Genetic.Data
{
    public class Happiness
    {
        private int happy = 60;
        private int thresholdTent = 3;
        private int thresholdWater = 50;
        private int thresholdToiletToWater = 25;
        private int thresholdToiletToTent = 25;
        private double priorityTent = 0.2;
        private double priorityWater = 0.2;
        private double priorityToilet = 0.1; 
        private double priorityBeds = 0.2;

        private double avrCountOfWaterThatTakeEachPerson = 0;
        public double GetAvrCountOfWaterThatTakeEachPerson()
        {
            return avrCountOfWaterThatTakeEachPerson / people;
        }

        public double GetAvrDistance()
        {
            return avrDistance;
        }

        public string GetDistance()
        {
            return "Between tents: " + avrDistanceBetTents + "\nBetween tents and toilets: " + avrDistanceBetTentsAndToilets
                    + "\nBetween Tents and water " + avrDistanceBetTentsAndWater + "\nBetween toilet and water " +
                    avrDistanceBetToiletAndWater;
        }

        private double avrDistance = 0;
        private double avrDistanceBetTents = 0;
        private double avrDistanceBetTentsAndToilets = 0;
        private double avrDistanceBetTentsAndWater = 0;
        private double avrDistanceBetToiletAndWater = 0;


        public void SetHappyLevel(int happyLevel)
        {
            happy = (happyLevel > 100) ? 100 : happyLevel < 0 ? 0 : happyLevel;
        }

        public void IncreaseHappy(List<Tent> tents, List<Water> waters, List<Toilet> toilets)
        {
            double distBetweenStruc = 0;
            for (int i = 0; i < tents.Count; i++)
            {
                for (int j = i + 1; j < tents.Count; j++)
                {
                    distBetweenStruc += (tents[i].GetDistance(tents[j]));
                    if (tents[i].GetDistance(tents[j]) == 0)
                    {
                        SetHappyLevel(0);
                        return;
                    }
                }
            }
            if (tents.Count != 0 && distBetweenStruc != 0)
            {
                distBetweenStruc = distBetweenStruc / (tents.Count * (tents.Count - 1) / 2);
                if (distBetweenStruc >= thresholdTent) SetHappyLevel((int)(happy + 100 * priorityTent));
                else SetHappyLevel((int)(happy - (thresholdTent / distBetweenStruc) * 100 * priorityTent));
            }
            else SetHappyLevel(happy - (int)(100 * priorityTent));

            avrDistanceBetTents = distBetweenStruc;
            avrDistance += distBetweenStruc;
            distBetweenStruc = 0;
            for (int i = 0; i < tents.Count; i++)
            {
                for (int j = 0; j < waters.Count; j++)
                {
                    distBetweenStruc += tents[i].GetDistance(waters[j]);
                    if (tents[i].GetDistance(waters[j]) == 0)
                    {
                        SetHappyLevel(0);
                        return;
                    }
                }
            }
            distBetweenStruc = distBetweenStruc / (tents.Count * waters.Count);
            if (distBetweenStruc <= thresholdWater) SetHappyLevel((int)(happy + 100 * priorityWater));
            else SetHappyLevel((int)(happy - (distBetweenStruc / thresholdWater) * 100 * priorityWater));

            avrDistanceBetTentsAndWater = distBetweenStruc;
            avrDistance += distBetweenStruc;
            distBetweenStruc = 0;
            for (int i = 0; i < toilets.Count; i++)
            {
                for (int j = 0; j < tents.Count; j++)
                {
                    distBetweenStruc += tents[j].GetDistance((toilets[i]));
                    if (tents[j].GetDistance(toilets[i]) == 0)
                    {
                        SetHappyLevel(0);
                        return;
                    }
                }
            }
            distBetweenStruc = distBetweenStruc / (tents.Count * toilets.Count);
            if (toilets.Count != 0 && distBetweenStruc <= thresholdToiletToTent)
                SetHappyLevel((int)(happy + 100 * priorityToilet));
            else if (toilets.Count == 0) SetHappyLevel(happy - (int)(100 * priorityToilet));
            else SetHappyLevel((int)(happy - (distBetweenStruc / thresholdToiletToTent) * 100 * priorityToilet));

            avrDistanceBetTentsAndToilets = distBetweenStruc;
            avrDistance += distBetweenStruc;
            distBetweenStruc = 0;
            for (int i = 0; i < toilets.Count; i++)
            {
                for (int j = 0; j < waters.Count; j++)
                {
                    distBetweenStruc += waters[j].GetDistance((toilets[i]));
                    if (toilets[i].GetDistance(waters[j]) == 0)
                    {
                        SetHappyLevel(0);
                        return;
                    }
                }
            }
            distBetweenStruc = distBetweenStruc / (toilets.Count * waters.Count);
            if (distBetweenStruc >= thresholdToiletToWater && distBetweenStruc != 0)
                SetHappyLevel((int)(happy + 100 * priorityToilet));
            else if (distBetweenStruc == 0) SetHappyLevel(happy - (int)(100 * priorityToilet));
            else SetHappyLevel((int)(happy - (thresholdToiletToWater / distBetweenStruc) * 100 * priorityToilet));

            avrDistanceBetToiletAndWater = distBetweenStruc;
            avrDistance += distBetweenStruc;
            avrDistance /= 4;
        }

        public void IncreaseHappy(List<Tent> tents, int people)
        {
            int capasity = 0;
            foreach (Tent tent in tents)
            {
                capasity += tent.GetCapacity();
            }
            SetHappyLevel(happy + (int)(((capasity - people) % 100) * priorityBeds));
        }

        int people;
        int allWater;

        public void IncreaseHappy(int overalWaterForDrink,/* int overalWaterForFood, int overalWaterForHyhiene,*/ int people, int allWater)
        {
            double priorityWaterForDrink = 0.3;
            //double priorityWaterForFood = 0.1;
            //double priorityWaterForHyhiene = 0.1;
            this.people = people;
            this.allWater = allWater;
            for (int i = 0; i < people; i++)
            {
                int stohastic = new Random().Next(overalWaterForDrink);
                stohastic = (stohastic == 0) ? 1 : stohastic;
                avrCountOfWaterThatTakeEachPerson += stohastic;
                /*if (CheckWater(overalWaterForDrink, priorityWaterForDrink))
                {
                    if (CheckWater(overalWaterForFood, priorityWaterForFood))
                    {
                        CheckWater(overalWaterForHyhiene, priorityWaterForHyhiene);
                    }
                }*/
            }
        }

        private bool CheckWater(int overalWater, double prWater)
        {
            if (allWater - overalWater >= 0)
            {
                allWater -= overalWater;
                SetHappyLevel((int)(happy + (100 * prWater) / people));
                return true;
            }
            else if (allWater - overalWater > -overalWater)
            {
                SetHappyLevel((int)(happy + ((Math.Abs(allWater - overalWater) / overalWater) * 100 / people) * prWater));
                allWater = 0;
                return false;
            }
            else
            {
                SetHappyLevel((int)(happy - (overalWater * 100 / people) * prWater));
                return false;
            }
        }

        public int GetHappy()
        {
            return happy;
        }

        
        //private int thresholdTent = 3;
        //private int thresholdWater = 50;
        //private int thresholdToiletToWater = 25;
        //private int thresholdToiletToTent = 25;
        //private double priorityTent = 0.2;
        //private double priorityWater = 0.2;
        //private double priorityToilet = 0.1;
        //private double priorityBeds = 0.2;

        public void SetParameters(int thrTent, int thrWater, int thrToiletToWater, int thrToiletToTent, double prTent, double prWater, double prToilet, double prBeds) {
            thresholdTent = thrTent;
            thresholdWater = thrWater;
            thresholdToiletToWater = thrToiletToWater;
            thresholdToiletToTent = thrToiletToTent;
            priorityTent = prTent;
            priorityWater = prWater;
            priorityToilet = prToilet;
            priorityBeds = prBeds;
        }

    }
}