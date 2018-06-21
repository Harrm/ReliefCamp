using System;
using UnityEngine;

namespace Refugee.Genetic.Data
{
    public class Tent : ILocationable
    {
        private Location location;
        private int capacity;

        public Tent(Vector2 p, int capacity)
        {
            location = new Location(p);
            this.capacity = capacity;
        }

        public double GetDistance(ILocationable structure)
        {
            return Math.Sqrt((this.X() - structure.X()) * (this.X() - structure.X()) + ((this.Y() - structure.Y()) * (this.Y() - structure.Y())));
        }
        public void Move(int x, int y)
        {
            location.Move(x, y);
        }

        public int X() { return (int)location.GetLocation().x; }
        public int Y() { return (int)location.GetLocation().y; }

        public int GetCapacity()
        {
            return capacity;
        }
    }
}