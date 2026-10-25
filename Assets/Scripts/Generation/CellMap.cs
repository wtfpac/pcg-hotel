namespace PCG.Generation
{
    // grade auxiliar que diz, pra cada celula, a que regiao ela pertence.
    // usamos isso pra descobrir onde vai parede: entre regioes diferentes.
    public class CellMap
    {
        public const int Outside = -2;
        public const int CorridorId = -1;

        private readonly int[,] cells;
        private readonly Area bounds;

        public CellMap(FloorPlan plan)
        {
            bounds = plan.Bounds;
            cells = new int[bounds.Width, bounds.Height];

            // tudo comeca como lado de fora
            for (int x = 0; x < bounds.Width; x++)
                for (int y = 0; y < bounds.Height; y++)
                    cells[x, y] = Outside;

            // corredores primeiro
            foreach (Corridor c in plan.Corridors)
                Paint(c.Area, CorridorId);

            // depois os quartos, cada um com seu indice na lista
            for (int i = 0; i < plan.Rooms.Count; i++)
                Paint(plan.Rooms[i].Area, i);
        }

        private void Paint(Area a, int id)
        {
            for (int x = a.X; x < a.Right; x++)
                for (int y = a.Y; y < a.Bottom; y++)
                    if (Contains(x, y))
                        cells[x - bounds.X, y - bounds.Y] = id;
        }

        public bool Contains(int x, int y)
        {
            return x >= bounds.X && x < bounds.Right &&
                   y >= bounds.Y && y < bounds.Bottom;
        }

        // devolve a regiao da celula, ou Outside se estiver fora da planta
        public int At(int x, int y)
        {
            if (!Contains(x, y))
                return Outside;

            return cells[x - bounds.X, y - bounds.Y];
        }
    }
}