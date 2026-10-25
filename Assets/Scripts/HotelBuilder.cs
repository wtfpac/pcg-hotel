using UnityEngine;
using System.Collections.Generic;
using PCG.Generation;

// le uma FloorPlan e constroi a geometria na cena.
// esta e a camada que conversa com a unity, por isso fica fora
// da pasta Generation, que e c# puro.
public class HotelBuilder : MonoBehaviour
{
    [Header("Parametros de geracao")]
    public int seed = 42;
    public int planWidth = 40;
    public int planHeight = 22;
    public int corridorWidth = 3;
    public int targetRoomWidth = 6;
    public int doorWidth = 2;

    [Header("Escala")]
    public float unitSize = 1f;        // quantos metros vale uma unidade de malha
    public float wallHeight = 3f;
    public float wallThickness = 0.15f;

    // paredes que viram vao de porta, consultadas na hora de construir
    private HashSet<DoorPlacer.WallKey> doors;

    void Start()
    {
        HotelGenerator generator = new HotelGenerator(corridorWidth, targetRoomWidth, seed);
        FloorPlan plan = generator.Generate(new Area(0, 0, planWidth, planHeight), seed);

        CellMap map = new CellMap(plan);
        doors = DoorPlacer.Place(plan, map, doorWidth);

        Debug.Log($"gerado: {plan.RoomCount} quartos, {doors.Count} celulas de porta");

        foreach (Corridor c in plan.Corridors)
            CreateFloor(c.Area, "Corridor", new Color(0.78f, 0.72f, 0.60f));

        foreach (Room r in plan.Rooms)
            CreateFloor(r.Area, "Room_" + r.Type, new Color(0.86f, 0.84f, 0.78f));

        BuildWalls(plan, map);
    }

    // cria uma laje fina cobrindo a area informada
    void CreateFloor(Area area, string name, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.parent = transform;

        float cx = (area.X + area.Width / 2f) * unitSize;
        float cz = (area.Y + area.Height / 2f) * unitSize;

        go.transform.position = new Vector3(cx, -0.05f, cz);
        go.transform.localScale = new Vector3(area.Width * unitSize,
                                              0.1f,
                                              area.Height * unitSize);

        go.GetComponent<Renderer>().material.color = color;
    }

    // percorre a planta celula por celula e levanta parede onde
    // duas celulas vizinhas pertencem a regioes diferentes.
    void BuildWalls(FloorPlan plan, CellMap map)
    {
        Area b = plan.Bounds;

        for (int x = b.X; x < b.Right; x++)
        {
            for (int y = b.Y; y < b.Bottom; y++)
            {
                int here = map.At(x, y);

                if (here == CellMap.Outside)
                    continue;

                if (map.At(x + 1, y) != here)
                    TryWall(x + 1, y, true, map.At(x + 1, y) == CellMap.Outside);

                if (map.At(x, y + 1) != here)
                    TryWall(x, y + 1, false, map.At(x, y + 1) == CellMap.Outside);

                // bordas de cima e da esquerda so aparecem na primeira
                // linha e coluna, porque la nao existe celula anterior
                if (x == b.X && map.At(x - 1, y) != here)
                    TryWall(x, y, true, true);

                if (y == b.Y && map.At(x, y - 1) != here)
                    TryWall(x, y, false, true);
            }
        }
    }

    // levanta a parede, a menos que aquela celula tenha virado porta
    void TryWall(int x, int y, bool vertical, bool exterior)
    {
        if (doors.Contains(new DoorPlacer.WallKey(vertical, x, y)))
            return;

        CreateWall(x, y, vertical, exterior);
    }

    // cria um pedaco de parede de 1 unidade de comprimento.
    // vertical = true significa que ela corre no eixo Z.
    void CreateWall(int x, int y, bool vertical, bool exterior)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = exterior ? "WallExt" : "WallInt";
        go.transform.parent = transform;

        float px = x * unitSize;
        float pz = y * unitSize;

        if (vertical)
        {
            go.transform.position = new Vector3(px, wallHeight / 2f, pz + unitSize / 2f);
            go.transform.localScale = new Vector3(wallThickness, wallHeight, unitSize);
        }
        else
        {
            go.transform.position = new Vector3(px + unitSize / 2f, wallHeight / 2f, pz);
            go.transform.localScale = new Vector3(unitSize, wallHeight, wallThickness);
        }

        Color color = exterior
            ? new Color(0.55f, 0.52f, 0.45f)
            : new Color(0.80f, 0.78f, 0.72f);

        go.GetComponent<Renderer>().material.color = color;
    }
}