namespace PCG.Generation
{
    // um quarto do hotel: onde fica e de que tipo é
    public class Room
    {
        public Area Area;
        public RoomType Type;

        public Room(Area area, RoomType type)
        {
            Area = area;
            Type = type;
        }

        public override string ToString()
        {
            return $"Room[{Type}] {Area}";
        }
    }
}
