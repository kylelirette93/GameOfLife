using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Grid properties.
    bool[,] cells;
    bool[,] newGrid;
    Vector3 gridOffset;

    // Cell objects.
    public GameObject liveCell;
    public GameObject deadCell;
    List<GameObject> objects = new List<GameObject>();
    

    private void Start()
    {
        // Offset to center grid on the screen.
        gridOffset = new Vector3(-36, -24, 0);

        cells = InitializeGrid(50, 75);
        DrawGrid(cells);
        InvokeRepeating("Tick", 0.05f, 0.05f);
    }

    private bool[,] InitializeGrid(int height, int width)
    {
        // Initialize grid with random cells.
        cells = new bool[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[y, x] = Random.value > 0.5f;
            }
        }
        return cells;
    }

    private void Tick()
    {
        // Instantiate new grid to store next generation.
        newGrid = new bool[cells.GetLength(0), cells.GetLength(1)];

        // Check neighbours and apply rules to each cell.
        for (int y = 0; y < cells.GetLength(0); y++)
            {
            for (int x = 0; x < cells.GetLength(1); x++)
            {
                int neighbours = CheckNeighbours(x, y);
                ApplyRules(x, y, neighbours);
            }
        }

        // Update cells to next generation.
        cells = newGrid;
        ClearCells();
        DrawGrid(cells);
    }

    private void ClearCells()
    {
        // Clears cells before next iteration to avoid duplicates.
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
        objects.Clear();
    }
    private void DrawGrid(bool[,] grid)
    {
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                // Reset the cell before drawing.
                GameObject cell = null;

                // Check if cell is alive or dead based on grid state.
                if (grid[y, x] == true)
                {
                    cell = Instantiate(liveCell);
                }
                else
                {
                    cell = Instantiate(deadCell);
                }
                objects.Add(cell);
                cell.transform.position = new Vector3Int(x, y) + gridOffset;
            }
        }
    }

    private int CheckNeighbours(int gridX, int gridY)
    {
        int neighbourCount = 0;
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (y == 0 && x == 0) continue; // skip the cell itself.

                // Calculate neighbour positions based on grid.
                int neighbourX = x + gridX;
                int neighbourY = y + gridY;

                // Check if neihbour is within bounds.
                if (neighbourX < 0 || neighbourY < 0 || neighbourX >= cells.GetLength(1) || neighbourY >= cells.GetLength(0)) continue;

                bool isAlive = cells[neighbourY, neighbourX];

                if (isAlive)
                {
                    neighbourCount++;
                }
                else
                {
                    // Do nothing.
                }
            }
        }
        return neighbourCount;
    }

    private void ApplyRules(int x, int y, int neighbours)
    {
        // Check if cell is alive or dead before applying rules.
        if (cells[y, x] == true)
        {
            // If less than 2 or more than 3 neighbours, cell dies as if by under population.
            if (neighbours < 2 || neighbours > 3)
            {
                newGrid[y, x] = false;
            }
            // Any live cell with two or three live neighbours lives on to next generation.
            else if (neighbours == 2 || neighbours == 3)
            {
                newGrid[y, x] = true;
            }
        }
        else
        {
            // If exactly 3 neighbours, cell becomes alive as if by reproduction.
            if (neighbours == 3)
            {
                newGrid[y, x] = true;
            }
            //else newGrid[y, x] = false; 
        }
    }  
}
