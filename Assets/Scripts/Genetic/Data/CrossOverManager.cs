using System.Collections.Generic;
using UnityEngine;

namespace Refugee.Genetic.Data
{
    public class CrossOverManager
    {
        private List<Camp> experimentalSelection;
        int amount;
        int amountOfWater;
        int numberOfWashrooms;
        int numberOfWaterResources;
        int numberOfTents;
        int numberOfPeople;
        int tentCapacity;
        int xBound;
        int yBound;
        int waterPerPersonForDrink;

        public CrossOverManager(int amount, int amountOfWater, int numberOfWashrooms, int numberOfWaterResources, int numberOfTents, int numberOfPeople, int tentCapacity, int xBound, int yBound, int waterPerPersonForDrink)
        {
            this.amount = amount;
            this.amountOfWater = amountOfWater;
            this.numberOfWashrooms = numberOfWashrooms;
            this.numberOfWaterResources = numberOfWaterResources;
            this.numberOfTents = numberOfTents;
            this.numberOfPeople = numberOfPeople;
            this.tentCapacity = tentCapacity;
            this.xBound = xBound;
            this.yBound = yBound;
            this.waterPerPersonForDrink = waterPerPersonForDrink;
        }

        public Camp Run(int epochs)
        {
            experimentalSelection = (GeneratePopulation());
            for (; epochs >= 0; epochs--)
            {
                experimentalSelection = CrossOver(experimentalSelection);
            }
            return Max(experimentalSelection);

        }

        private Camp Max(List<Camp> experimentalSelection)
        {
            if (experimentalSelection != null && experimentalSelection.Count > 0)
            {
                Camp result = experimentalSelection[0];

                foreach (Camp camp in experimentalSelection)
                {
                    if (camp.GetHappiness() > result.GetHappiness()) result = camp;
                }
                return result;
            }
            return null;
        }

        public List<Camp> GeneratePopulation()
        {

            List<Camp> population = new List<Camp>();
            System.Random gen = new System.Random();
            //Randomly generate camps
            for (int i = 0; i < amount; i++)
            {
                Vector2 sizeOfCamp = new Vector2(xBound, yBound);
                int tmpRand = gen.Next(3);
                tmpRand += (tmpRand == 0) ? 1 : 0;
                //Picking random number for water/person
                Camp camp = new Camp(
                        amountOfWater,
                        waterPerPersonForDrink,
                        //gen.Next(amountOfWater) % 9 + 1,
                        //gen.Next(amountOfWater) % 9 + 1,
                        numberOfPeople,
                        sizeOfCamp);
                //Placing tents
                for (int j = 0; j < numberOfTents; j++)
                {
                    Vector2 p = new Vector2(gen.Next(xBound), gen.Next(yBound));
                    camp.GetTents().Add(new Tent(p, tentCapacity));
                }
                //Placing water
                for (int k = 0; k < numberOfWaterResources; k++)
                {
                    camp.GetWaters().Add(new Water(gen.Next(xBound), gen.Next(yBound)));
                }
                //Placing washrooms
                for (int z = 0; z < numberOfWashrooms; z++)
                {
                    camp.GetToilets().Add(new Toilet(gen.Next(xBound), gen.Next(yBound)));
                }

                //camp.waterPerPerson = gen.nextInt(amountOfWater);
                population.Add(camp);
            }
            return population;
        }

        public List<Camp> CrossOver(List<Camp> population)
        {
            System.Random gen = new System.Random();
            //Dividing into 2 groups
            List<Camp> group1 = new List<Camp>();
            List<Camp> group2 = new List<Camp>();
            group1.Add(population[0]);
            group1.Add(population[1]);
            group2.Add(population[2]);
            group2.Add(population[3]);
            population.RemoveAt(0);
            population.RemoveAt(0);
            population.RemoveAt(0);
            population.RemoveAt(0);

            foreach (Camp c in population)
            {
                if (gen.Next(2) == 0)
                {
                    group1.Add(c);
                }
                else
                {
                    group2.Add(c);
                }
            }
            //Sort groups by happiness
            group1.Sort((Camp c1, Camp c2) => c1.CompareTo(c2));
            group2.Sort((Camp c1, Camp c2) => c1.CompareTo(c2));
            //Clearing initial population
            population.Clear();
            //Generating children and replacing the least happy camps
            Camp child1 = GenerateChild(group1[group1.Count - 1], group1[group1.Count - 2]);
            Camp child2 = GenerateChild(group1[group1.Count - 1], group1[group1.Count - 2]);
            group1.RemoveAt(0);
            group1.RemoveAt(0);
            group1.Add(child1);
            group1.Add(child2);
            //        System.out.println(group1.size() + " " + group2.size());
            Camp child3 = GenerateChild(group2[group2.Count - 1], group2[group2.Count - 2]);
            Camp child4 = GenerateChild(group2[group2.Count - 1], group2[group2.Count - 2]);
            group2.RemoveAt(0);
            group2.RemoveAt(0);
            group2.Add(child3);
            group2.Add(child4);
            //Adding all together
            population.AddRange(group1);
            population.AddRange(group2);

            return population;
        }

        public Camp GenerateChild(Camp p1, Camp p2)
        {
            System.Random gen = new System.Random();
            Camp child = new Camp(p1, p2);

            //TODO
            //MUTATIONS!!!!!!

            switch (gen.Next(4))
            {
                case 0:
                    MoveTents(child); break;
                case 1:
                    MoveWater(child); break;
                case 2:
                    MoveWashroom(child); break;
                case 3:
                    ChangeWaterPerPerson(child); break;
            }

            return child;
        }
        public void MoveTents(Camp camp)
        {
            System.Random gen = new System.Random();
            foreach (Tent t in camp.GetTents())
            {
                t.Move(gen.Next((int)camp.GetSize().x), gen.Next((int)camp.GetSize().y));
            }
        }

        public void MoveWater(Camp camp)
        {
            System.Random gen = new System.Random();
            foreach (Water w in camp.GetWaters())
            {
                w.Move(gen.Next((int)camp.GetSize().x), gen.Next((int)camp.GetSize().y));
            }
        }

        public void MoveWashroom(Camp camp)
        {
            System.Random gen = new System.Random();
            foreach (Toilet t in camp.GetToilets())
            {
                t.Move(gen.Next((int)camp.GetSize().x), gen.Next((int)camp.GetSize().y));
            }
        }

        public void ChangeWaterPerPerson(Camp camp)
        {
            System.Random gen = new System.Random();
            camp.SetWaterPerPersonForDrink(waterPerPersonForDrink);
            //camp.SetWaterPerPersonForFood(gen.Next(camp.GetAllWater()));
            //camp.SetWaterPerPersonForHyhiene(gen.Next(camp.GetAllWater()));
        }
    }
}