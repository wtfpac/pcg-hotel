using System;
using System.Text;

namespace PCG.Generation
{
    // desenha uma FloorPlan em texto, so pra conferir no terminal.
    public static class PlanRenderer
    {
        public static string Render(FloorPlan plan)
        {
            int w = plan.Bounds.Width;
            int h = plan.Bounds.Height;

            // grade de caracteres, comeca toda vazia
            char[,] grid = new char[h, w];

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    grid[y, x] = ' ';

            // corredores primeiro, quartos por cima
            foreach (Corridor c in plan.Corridors)
                Paint(grid, c.Area, ':');

            foreach (Room r in plan.Rooms)
                Paint(grid, r.Area, Symbol(r.Type));

            // monta o texto final, dobrando cada coluna
            // porque caractere de terminal e mais alto que largo
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    sb.Append(grid[y, x]);
                    sb.Append(grid[y, x]);
                }

                sb.Append('\n');
            }

            return sb.ToString();
        }

        // pinta uma area na grade, com borda '#' e miolo com o simbolo
        private static void Paint(char[,] grid, Area a, char fill)
        {
            for (int y = a.Y; y < a.Bottom; y++)
            {
                for (int x = a.X; x < a.Right; x++)
                {
                    bool border = (x == a.X || x == a.Right - 1 ||
                                   y == a.Y || y == a.Bottom - 1);

                    grid[y, x] = border ? '#' : fill;
                }
            }
        }

        private static char Symbol(RoomType type)
        {
            switch (type)
            {
                case RoomType.Single: return '1';
                case RoomType.Double: return '2';
                case RoomType.Twin:   return 'T';
                case RoomType.Suite:  return 'S';
                default:              return '?';
            }
        }
    }
}