using UnityEngine;
using System.Collections.Generic;
using PCG.Generation;

// le uma FloorPlan e constroi a geometria na cena.
// R regera com semente nova, T mostra ou esconde o teto.
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
    public float unitSize = 1f;
    public float wallHeight = 3f;
    public float wallThickness = 0.15f;

    [Header("Teclas")]
    public KeyCode regenerateKey = KeyCode.R;
    public KeyCode toggleCeilingKey = KeyCode.T;

    private HashSet<DoorPlacer.WallKey> doors;

    // guardamos tudo que criamos pra conseguir apagar na hora de regerar
    private readonly List<GameObject> spawned = new List<GameObject>();
    private readonly List<GameObject> ceilings = new List<GameObject>();

    private bool ceilingVisible = true;

    void Start()
    {
        Build();
    }

    void Update()
    {
        if (Input.GetKeyDown(regenerateKey))
        {
            // semente nova a cada vez
            seed = Random.Range(0, 999999);
            Build();
        }

        if (Input.GetKeyDown(toggleCeilingKey))
        {
            ceilingVisible = !ceilingVisible;

            foreach (GameObject c in ceilings)
                c.SetActive(ceilingVisible);
        }
    }

    // apaga o hotel anterior e constroi um novo
    void Build()
    {
        Clear();

        HotelGenerator generator = new HotelGenerator(corridorWidth, targetRoomWidth, seed);
        FloorPlan plan = generator.Generate(new Area(0, 0, planWidth, planHeight), seed);

        CellMap map = new CellMap(plan);
        doors = DoorPlacer.Place(plan, map, doorWidth);

        Debug.Log($"semente {seed} | {plan.RoomCount} quartos");

        foreach (Corridor c in plan.Corridors)
        {
            CreateSlab(c.Area, "Corridor", new Color(0.78f, 0.72f, 0.60f), -0.05f);
            CreateCeiling(c.Area);
        }

        foreach (Room r in plan.Rooms)
        {
            CreateSlab(r.Area, "Room_" + r.Type, new Color(0.86f, 0.84f, 0.78f), -0.05f);
            CreateCeiling(r.Area);
        }

        BuildWalls(plan, map);
    }

    // destroi tudo que foi criado na geracao anterior
    void Clear()
    {
        foreach (GameObject go in spawned)
            Destroy(go);

        spawned.Clear();
        ceilings.Clear();
    }

    // laje horizontal usada tanto pra chao quanto pra teto
    GameObject CreateSlab(Area area, string name, Color color, float y)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.parent = transform;

        float cx = (area.X + area.Width / 2f) * unitSize;
        float cz = (area.Y + area.Height / 2f) * unitSize;

        go.transform.position = new Vector3(cx, y, cz);
        go.transform.localScale = new Vector3(area.Width * unitSize,
                                              0.1f,
                                              area.Height * unitSize);

        go.GetComponent<Renderer>().material.color = color;

        spawned.Add(go);
        return go;
    }

    // teto vai na mesma posicao do chao, so que la em cima.
    // guardado numa lista separada pra poder ligar e desligar.
    void CreateCeiling(Area area)
    {
        GameObject go = CreateSlab(area, "Ceiling", new Color(0.92f, 0.90f, 0.86f), wallHeight);
        go.SetActive(ceilingVisible);
        ceilings.Add(go);
    }

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

                if (x == b.X && map.At(x - 1, y) != here)
                    TryWall(x, y, true, true);

                if (y == b.Y && map.At(x, y - 1) != here)
                    TryWall(x, y, false, true);
            }
        }
    }

    void TryWall(int x, int y, bool vertical, bool exterior)
    {
        if (doors.Contains(new DoorPlacer.WallKey(vertical, x, y)))
            return;

        CreateWall(x, y, vertical, exterior);
    }

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

        spawned.Add(go);
    }
}