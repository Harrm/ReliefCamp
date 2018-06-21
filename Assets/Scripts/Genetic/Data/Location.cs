using UnityEngine;

namespace Refugee.Genetic.Data
{
    public class Location
    {
        private int x, y;

        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Location(Vector2 p)
        {
            x = (int)p.x;
            y = (int)p.y;
        }

        public void Move(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 GetLocation()
        {
            return new Vector2(x, y);
        }
    }
}