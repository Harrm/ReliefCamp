namespace Refugee.Genetic.Data
{
    public interface ILocationable
    {
        int X();
        int Y();
        double GetDistance(ILocationable structure);
        void Move(int x, int y);
    }
}