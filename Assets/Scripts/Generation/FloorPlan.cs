using System.Collections.Generic;

namespace PCG.Generation
{
    // resultado de uma geracao, tudo descreve um pavimento
    // eh isso q o gerador devolve e que o resto do sistema consome
    public class FloorPlan
    {
        public Area Bounds;     // retangulo total do pavimento
        public int Seed;        // semente que produziu essa planta
        public List<Room> Rooms;
        public List<Corridor> Corridors;

        public FloorPlan(Area bounds, int seed)
        {
            Bounds = bounds;
            Seed = seed;
            Rooms = new List<Room>();
            Corridors = new List<Corridor>();
        }

        // atalhos de leitura, usados na metrica dps
        public int RoomCount => Rooms.Count;
        public int CorridorCount => Corridors.Count;

        // quantos quartos de um tipo especifico existem nessa planta
        public int CountOfType(RoomType type)
        {
            int total = 0;

            foreach (Room r in Rooms)
            {
                if (r.Type == type)
                    total++;
            }

            return total;
        }
    }
}