using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasIntroduction : MonoBehaviour {
    public GenerateMap generateMap; // Reference to the GenerateMap script
    public MapVisualization mapVisualization;
    public float pressureValue = 100f; // Pressure value to set for the cell
    public Vector2Int position = new Vector2Int(0, 0); // Default position (0, 0)

    void Start() {
        if (generateMap == null) {
            Debug.LogError("GenerateMap reference is missing!");
            return;
        }

        IntroduceGas(position, pressureValue);
        mapVisualization.Update();
    }

    public void IntroduceGas(Vector2Int position, float pressure) {
        if (position.x < 0 || position.x >= generateMap.width || position.y < 0 || position.y >= generateMap.height) {
            Debug.LogError("Position is out of bounds!");
            return;
        }

        // Ensure the cell is an Air cell before introducing gas
        if (generateMap.map[position.x, position.y].type == GenerateMap.CellType.Air) {
            generateMap.map[position.x, position.y].pressure = pressure;
            Debug.Log($"Introduced gas at ({position.x}, {position.y}) with pressure {pressure}");
        } else {
            Debug.LogError("Cannot introduce gas to a non-Air cell!");
        }
    }
}