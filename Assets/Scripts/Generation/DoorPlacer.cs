using System.Collections.Generic;

namespace PCG.Generation
{
    // decide onde cada quarto abre para a circulacao.
    // regra: uma porta por quarto, no meio da fronteira com o corredor.
    public static class DoorPlacer
    {
        // chave de uma celula de parede: orientacao + coordenada da grade.
        // vertical = parede que corre no eixo Z, em x fixo.
        public struct WallKey
        {
            public bool Vertical;
            public int X;
            public int Y;

            public WallKey(bool vertical, int x, int y)
            {
                Vertical = vertical;
                X = x;
                Y = y;
            }
        }

        // devolve o conjunto de paredes que devem virar vao de porta
        public static HashSet<WallKey> Place(FloorPlan plan, CellMap map, int doorWidth)
        {
            HashSet<WallKey> doors = new HashSet<WallKey>();

            for (int i = 0; i < plan.Rooms.Count; i++)
            {
                List<WallKey> candidates = FindCorridorBorder(plan.Rooms[i].Area, map, i);

                if (candidates.Count == 0)
                    continue;

                // pega as celulas do meio da fronteira
                int start = (candidates.Count - doorWidth) / 2;

                if (start < 0)
                    start = 0;

                for (int k = 0; k < doorWidth && start + k < candidates.Count; k++)
                    doors.Add(candidates[start + k]);
            }

            return doors;
        }

        // lista as celulas de parede onde este quarto encosta no corredor
        private static List<WallKey> FindCorridorBorder(Area a, CellMap map, int roomId)
        {
            List<WallKey> found = new List<WallKey>();

            for (int x = a.X; x < a.Right; x++)
            {
                // borda de cima
                if (map.At(x, a.Y - 1) == CellMap.CorridorId)
                    found.Add(new WallKey(false, x, a.Y));

                // borda de baixo
                if (map.At(x, a.Bottom) == CellMap.CorridorId)
                    found.Add(new WallKey(false, x, a.Bottom));
            }

            for (int y = a.Y; y < a.Bottom; y++)
            {
                // borda da esquerda
                if (map.At(a.X - 1, y) == CellMap.CorridorId)
                    found.Add(new WallKey(true, a.X, y));

                // borda da direita
                if (map.At(a.Right, y) == CellMap.CorridorId)
                    found.Add(new WallKey(true, a.Right, y));
            }

            return found;
        }
    }
}