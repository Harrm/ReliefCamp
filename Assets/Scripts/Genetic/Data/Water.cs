using System;

namespace Refugee.Genetic.Data
{
    public class Water : ILocationable
    {
        private Location location;
        public Water(int x, int y)
        {
            location = new Location(x, y);
        }
        public double GetDistance(ILocationable structure)
        {
            return Math.Sqrt(Math.Pow((X() - structure.X()), 2) + Math.Pow((Y() - structure.Y()), 2));
        }

        public void Move(int x, int y)
        {
            location.Move(x, y);
        }

        public int X() { return (int)location.GetLocation().x; }
        public int Y() { return (int)location.GetLocation().y; }
    }
}