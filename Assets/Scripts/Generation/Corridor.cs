namespace PCG.Generation
{
    // uma faixa de circulacao, mesma ideia do room mas sem esse tipo
    public class Corridor
    {
        public Area Area;

        public Corridor(Area area)
        {
            Area = area;
        }

        public override string ToString()
        {
            return $"Corridor {Area}";
        }
    }
}