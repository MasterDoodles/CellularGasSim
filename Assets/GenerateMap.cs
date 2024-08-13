using UnityEngine;

public class GenerateMap : MonoBehaviour {
    public int width = 10;  // Set default values for testing
    public int height = 10; // Set default values for testing
    public string seed;
    public bool useRandomSeed;

    public Cell[,] map;

    void Start() {
        Generate();
    }

    void Generate() {
        map = new Cell[width, height];
        InitializeMap();
        Debug.Log("Map generated with size: " + width + "x" + height);
    }

    void InitializeMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                map[x, y] = new Cell {
                    type = (x == 0 || x == width - 1 || y == 0 || y == height - 1) ? CellType.Solid : CellType.Air,
                    pressure = (x == 0 || x == width - 1 || y == 0 || y == height - 1) ? -1 : 0
                };
                // Debug.Log($"Cell ({x}, {y}) initialized as {map[x, y].type}");
            }
        }
    }

    public enum CellType {
        Solid,
        Air
    }

    public class Cell {
        public CellType type;
        public float pressure;
    }
}