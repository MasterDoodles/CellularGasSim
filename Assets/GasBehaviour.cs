using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GasBehaviour : MonoBehaviour
{
    public GenerateMap generateMap;  // Reference to the GenerateMap script
    public MapVisualization mapVisualization;  // Reference to the MapVisualization script

    public int iterations = 10;
    public float delayBetweenIterations = 1.0f; // Time in seconds

    private int counter = 0;

    void Start()
    {
        if (generateMap == null)
        {
            Debug.LogError("GenerateMap reference is missing!");
            return;
        }
        if (mapVisualization == null)
        {
            Debug.LogError("MapVisualization reference is missing!");
            return;
        }
        StartCoroutine(DelayedPropagation());



    }
    private IEnumerator DelayedPropagation()
    {
        yield return new WaitForSeconds(delayBetweenIterations);
        for (int x = 0; x < iterations; x++)
        {
            counter += 1;
            PerformGasPropagation();
            Debug.LogError("Itteration " + x);
            yield return new WaitForSeconds(delayBetweenIterations);
        }
    }

    void PerformGasPropagation()
    {
        // Create a deep copy of the current map
        GenerateMap.Cell[,] oldMap = generateMap.map;
        GenerateMap.Cell[,] newMap = new GenerateMap.Cell[generateMap.width, generateMap.height];

        // Copy oldMap to newMap, setting pressure of air cells to 0
        for (int x = 0; x < generateMap.width; x++)
        {
            for (int y = 0; y < generateMap.height; y++)
            {
                // Copy the cell type and other properties
                newMap[x, y] = new GenerateMap.Cell
                {
                    type = oldMap[x, y].type,
                    pressure = (oldMap[x, y].type == GenerateMap.CellType.Air) ? 0 : oldMap[x, y].pressure
                };
            }
        }

        // Debug.LogError("Old Map middle pressure "+ oldMap[5, 5].pressure);
        for (int x = 0; x < generateMap.width; x++)
        {
            for (int y = 0; y < generateMap.height; y++)
            {
                if (newMap[x, y].type == GenerateMap.CellType.Air)
                { //if its air
                    // Sum pressures from neighboring cells
                    float centre = oldMap[x, y].pressure;

                    // Initialize the list to hold valid pressure data
                    List<PressureData> validPressureData = new List<PressureData>();



                    // Check neighboring cells and add valid pressures to the list
                    AddValidPressureData(oldMap, x, y - 1, centre, validPressureData); // Top
                    AddValidPressureData(oldMap, x - 1, y, centre, validPressureData); // Left
                    AddValidPressureData(oldMap, x + 1, y, centre, validPressureData); // Right
                    AddValidPressureData(oldMap, x, y + 1, centre, validPressureData); // Bottom

                    AddValidPressureData(oldMap, x - 1, y - 1, centre, validPressureData); // Top-left
                    AddValidPressureData(oldMap, x + 1, y - 1, centre, validPressureData); // Top-right
                    AddValidPressureData(oldMap, x - 1, y + 1, centre, validPressureData); // Bottom-left
                    AddValidPressureData(oldMap, x + 1, y + 1, centre, validPressureData); // Bottom-right

                    // Calculate the new pressure by summing valid pressures
                    float totalPressure = 0f;
                    foreach (PressureData data in validPressureData)
                    {

                        totalPressure += data.Pressure;
                        // if( counter ==3 && x==3 && y ==7){
                        //     Debug.LogError("Valid data count: "+ validPressureData.Count);
                        //     Debug.LogError("Total Pressure: "+ totalPressure);
                        // }
                        // Optional: Use data.Position for further calculations
                    }
                    //Equilibrium calculation of trying to reach a state of rest 
                    // sums all the surrounding pressures that are lower and gives a average rounded down for it
                    totalPressure += centre;
                    // if( counter ==3 && x==3 && y ==7){
                    //         Debug.LogError("Total Pressure after centre: "+ totalPressure);
                    //     }
                    int count = validPressureData.Count + 1;

                    float newPressure = totalPressure / count;
                    // if(counter ==3  && x==3 && y ==7){
                    //         Debug.LogError("New Pressure before round=  "+ newPressure);


                    //     }
                    // newPressure = (int)Math.Floor(newPressure);
                    newMap[x, y].pressure += newPressure;
                    if (counter == 1 && x == 5 && y == 5)
                    {
                        Debug.LogError("New Pressure =  " + newPressure);


                    }
                    //then it adds its pressure to the new map  

                    // if(newPressure>0){Debug.LogError(newPressure);}



                    //newPressure is gonna be the equilibrium of all the cells around it 
                    // plus

                    // so far check that if its bigger than the centre pressure it gets assigned -1
                    // then i want to add them to a list
                    // then loop through add together all values that are not -1
                    //that becomes my centre pressure

                    //done all above

                    float total_weight = 0;
                    foreach (PressureData data in validPressureData)//calculate total weight for pressures
                    {
                        total_weight += data.Weight;//found an error where i was inversing the inverse kill meeeeeee

                        // Debug.LogError(total_weight);
                        // if(counter ==3  && x==5 && y ==5){
                        //     Debug.LogError("data.Weight"+ data.Weight);


                        // }

                    }
                    if (counter == 3 && x == 5 && y == 5)
                    {
                        Debug.LogError("Total_weight" + total_weight);


                    }

                    float pressure_change = Math.Abs(centre - newPressure);
                    if (counter == 3 && x == 5 && y == 5)
                    {
                        Debug.LogError("Preasure Change then " + pressure_change);


                    }
                    float total_pressure_moved = 0;
                    foreach (PressureData data in validPressureData)
                    {
                        // Adjust the pressure in the new map
                        int nx = data.Position.x;
                        int ny = data.Position.y;

                        float inverse_pressure = data.Weight;

                        float weight = inverse_pressure / total_weight;
                        //We do all this to have more pressure go to lower pressure areas


                        // Distribute the pressure change equally
                        float pressure_to_add = pressure_change * weight; //if all things go well this should be 
                        // pressure_to_add = (int)Math.Floor(pressure_to_add); // I do this so that its nice round numbers we are working with

                        newMap[nx, ny].pressure += pressure_to_add;
                        total_pressure_moved += pressure_to_add;
                    }



                    float remaining_pressure = Math.Abs(total_pressure_moved - pressure_change);
                    //any remaining pressure we are just going to say is added back to the original centre
                    newMap[x, y].pressure += remaining_pressure;
                    // if(pressure_change>0)Debug.LogError("Pressure change: "+pressure_change);
                    // if (remaining_pressure > 0) Debug.LogError("Left over pressure: "+remaining_pressure);


                    //after that we need to work out how much pressure is lost from centre to new centre so that should be a calulation
                    //after calculating that we need to find out how many pressure units are allocated to each other cell
                    //Bigger values get less smaller values get more
                    // Debug.Log($"Cell ({x}, {y}) - Old Pressure: {centre}, New Pressure: {newPressure}, Total Pressure Moved: {total_pressure_moved}, Remaining Pressure: {remaining_pressure}");
                }
            }
        }

        // Replace the old grid with the new one
        generateMap.map = newMap;

        if (counter == 250)
        {
            Debug.Log(newMap[1, 1].pressure);

        }

        // Update the visualization
        mapVisualization.Update();
    }
    void AddValidPressureData(GenerateMap.Cell[,] map, int x, int y, float centre, List<PressureData> pressureDataList)
    {
        // Check boundaries
        if (x >= 0 && x < generateMap.width && y >= 0 && y < generateMap.height)
        {
            float pressure = map[x, y].pressure;
            if (pressure != -1 && pressure < centre)
            {
                float weight = 1;
                if (pressure > 0) weight = 1 / pressure;
                pressureDataList.Add(new PressureData(pressure, new Vector2Int(x, y), weight));
            }
        }
    }
    public class PressureData
    {
        public float Pressure { get; set; }
        public Vector2Int Position { get; set; }

        public float Weight { get; set; }

        public PressureData(float pressure, Vector2Int position, float weight)
        {
            Pressure = pressure;
            Position = position;
            Weight = weight;
        }
    }
}