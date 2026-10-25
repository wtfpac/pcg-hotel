namespace PCG.Generation
{
    //retangulo em coordenadas interas. x,y = canto superior esquerdo
    public struct Area
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Area(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        // bordas calculadas, nao guardadas evita valor inconsistente
        public int Right => X + Width;
        public int Bottom => Y + Height;

        // facilita debug consolewritelinearea ja sai lgivel
        public override string ToString() => $"Area({X}, {Y}, {Width}x{Height})";
    }
}