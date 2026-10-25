using System;

namespace PCG.Generation
{
    // gera um pavimento simples: um corredor central e duas fileiras
    // de quartos, uma de cada lado. todo quarto encosta no corredor.
    public class HotelGenerator
    {
        private readonly int corridorWidth;
        private readonly int targetRoomWidth;
        private readonly Random random;

        // lista de tipos montada uma vez so, na forma que a unity aceita
        private static readonly RoomType[] AllTypes =
            (RoomType[])Enum.GetValues(typeof(RoomType));

        public HotelGenerator(int corridorWidth, int targetRoomWidth, int seed)
        {
            this.corridorWidth = corridorWidth;
            this.targetRoomWidth = targetRoomWidth;
            this.random = new Random(seed);
        }

        public FloorPlan Generate(Area bounds, int seed)
        {
            FloorPlan plan = new FloorPlan(bounds, seed);

            // corredor corre ao longo do lado mais comprido
            bool horizontal = bounds.Width >= bounds.Height;

            if (horizontal)
            {
                // faixa horizontal no meio da altura
                int corridorY = bounds.Y + (bounds.Height - corridorWidth) / 2;

                plan.Corridors.Add(new Corridor(
                    new Area(bounds.X, corridorY, bounds.Width, corridorWidth)));

                // fileira de cima e fileira de baixo
                FillRow(plan, new Area(bounds.X, bounds.Y,
                                       bounds.Width, corridorY - bounds.Y), true);

                int belowY = corridorY + corridorWidth;
                FillRow(plan, new Area(bounds.X, belowY,
                                       bounds.Width, bounds.Bottom - belowY), true);
            }
            else
            {
                // faixa vertical no meio da largura
                int corridorX = bounds.X + (bounds.Width - corridorWidth) / 2;

                plan.Corridors.Add(new Corridor(
                    new Area(corridorX, bounds.Y, corridorWidth, bounds.Height)));

                FillRow(plan, new Area(bounds.X, bounds.Y,
                                       corridorX - bounds.X, bounds.Height), false);

                int rightX = corridorX + corridorWidth;
                FillRow(plan, new Area(rightX, bounds.Y,
                                       bounds.Right - rightX, bounds.Height), false);
            }

            return plan;
        }

        // corta uma faixa em quartos. cutAlongX diz se os cortes
        // sao verticais (fileira deitada) ou horizontais (fileira em pe).
        private void FillRow(FloorPlan plan, Area band, bool cutAlongX)
        {
            if (band.Width <= 0 || band.Height <= 0)
                return;

            int length = cutAlongX ? band.Width : band.Height;

            // quantos quartos cabem, arredondando pro mais proximo
            int count = Math.Max(1, (int)Math.Round((double)length / targetRoomWidth));

            // divide o comprimento o mais igual possivel
            int baseSize = length / count;
            int leftover = length - baseSize * count;

            int offset = 0;

            for (int i = 0; i < count; i++)
            {
                // os primeiros quartos ficam 1 unidade maiores
                // ate o resto da divisao acabar
                int size = baseSize + (i < leftover ? 1 : 0);

                Area area = cutAlongX
                    ? new Area(band.X + offset, band.Y, size, band.Height)
                    : new Area(band.X, band.Y + offset, band.Width, size);

                plan.Rooms.Add(new Room(area, PickType()));

                offset += size;
            }
        }

        // por enquanto sorteia entre os tipos existentes
        private RoomType PickType()
        {
            return AllTypes[random.Next(AllTypes.Length)];
        }
    }
}