using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapVisualization : MonoBehaviour
{
    public GenerateMap generateMap;
    public GameObject solidPrefab;
    public GameObject airPrefab;
    public bool displayNumbers = false;

    private GameObject[,] cellObjects; // Store references to the instantiated cell objects

    private void Start()
    {
        if (generateMap == null)
        {
            Debug.LogError("GenerateMap reference is missing!");
            return;
        }
        if (solidPrefab == null)
        {
            Debug.LogError("SolidPrefab reference is missing!");
            return;
        }
        if (airPrefab == null)
        {
            Debug.LogError("AirPrefab reference is missing!");
            return;
        }

        InitializeMap();
    }

    private void InitializeMap()
    {
        cellObjects = new GameObject[generateMap.width, generateMap.height];

        for (int x = 0; x < generateMap.width; x++)
        {
            for (int y = 0; y < generateMap.height; y++)
            {
                Vector3 position = new Vector3(x, y, 0);
                GameObject prefab = generateMap.map[x, y].type == GenerateMap.CellType.Solid ? solidPrefab : airPrefab;
                cellObjects[x, y] = Instantiate(prefab, position, Quaternion.identity);
                cellObjects[x, y].transform.parent = transform;
            }
        }

        VisualizeMap(); // Initial visualization
    }

    public void Update()
    {
        VisualizeMap(); // Call this to update visuals each frame
    }

    void VisualizeMap()
    {
        for (int x = 0; x < generateMap.width; x++)
        {
            for (int y = 0; y < generateMap.height; y++)
            {
                if (generateMap.map[x, y].type == GenerateMap.CellType.Air)
                {
                    float pressure = generateMap.map[x, y].pressure;
                    Color baseColor = pressure > 0 ? new Color(1f, 0.5f, 0.5f) : Color.white; // Light red if gas is present
                    float normalizedPressure = Mathf.Clamp01(pressure / 100f); // Normalize pressure
                    Color cellColor = Color.Lerp(baseColor, Color.red, normalizedPressure);

                    Renderer renderer = cellObjects[x, y].GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = cellColor;
                    }

                    if (displayNumbers)
                    {
                        // Display numbers if enabled
                        TextMesh textMesh = cellObjects[x, y].GetComponentInChildren<TextMesh>();
                        if (textMesh == null)
                        {
                            GameObject textObject = new GameObject("PressureText");
                            textObject.transform.position = cellObjects[x, y].transform.position;
                            textMesh = textObject.AddComponent<TextMesh>();
                            textMesh.fontSize = 10;
                            textMesh.color = Color.black;
                            textMesh.alignment = TextAlignment.Center;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textObject.transform.parent = cellObjects[x, y].transform;
                        }
                        textMesh.text = Mathf.RoundToInt(pressure).ToString();
                    }
                }
            }
        }
    }
}
