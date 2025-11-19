using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Grid properties.
    bool[,] cells;
    bool[,] newGrid;
    bool[,] zombies;
    Vector3 gridOffset;

    // Cell objects.
    [SerializeField] GameObject liveCell;
    [SerializeField] GameObject deadCell;
    [SerializeField] GameObject zombieCell;
    List<GameObject> objects = new List<GameObject>();
    int timeElapsed = 0;
    bool canCount = true;
    [SerializeField] int maxTicks = 1000;
    [SerializeField] int gridHeight = 50;
    [SerializeField] int gridWidth = 75;
    [SerializeField] int gridOffsetX = -36;
    [SerializeField] int gridOffsetY = -24;
    [SerializeField] int zombieCount = 5;


    private void Start()
    {
        // Offset to center grid on the screen.
        gridOffset = new Vector3(gridOffsetX, gridOffsetY, 0);
        cells = InitializeGrid(gridHeight, gridWidth);
        zombies = new bool[gridHeight, gridWidth];

        // Spawn a few zombies initially.
        SpawnZombies(zombieCount);
        DrawGrid(cells);
        InvokeRepeating("Tick", 0.1f, 0.1f);
    }

    private void SpawnZombies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int x = Random.Range(0, gridWidth);
            int y = Random.Range(0, gridHeight);
            zombies[y, x] = true;
            cells[y, x] = false; 
        }
    }
    public void Regenerate()
    {
        ClearCells();
        timeElapsed = 0;
        canCount = true;
        cells = InitializeGrid(gridHeight, gridWidth);
        ClearCells();
        SpawnZombies(zombieCount);
        DrawGrid(cells);
        InvokeRepeating("Tick", 0.1f, 0.1f);
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

        if (zombies != null)
        {
            // Revive zombies. 
            for (int y = 0; y < zombies.GetLength(0); y++)
            {
                for (int x = 0; x < zombies.GetLength(1); x++)
                {
                    if (zombies[y, x] == true)
                    {
                        int neighbours = CheckNeighbours(x, y);
                        SurviveHorde(x, y, neighbours);
                    }
                }
            }
        }

        // Update cells to next generation.
        cells = newGrid;
        ClearCells();
        DrawGrid(cells);
    }

    private void Update()
    {
        if (canCount)
        {
            timeElapsed += 1;
        }
        if (timeElapsed >= maxTicks)
        {
            CancelInvoke("Tick");
            canCount = false;
        }
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
                if (zombies[y, x] == true)
                {
                    cell = Instantiate(zombieCell);
                }
                else if (grid[y, x] == true)
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

    private void SurviveHorde(int x, int y, int neighbours)
    {
        // Zombies infect adjacent cells. 
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dy == 0 && dx == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                if (nx < 0 || ny < 0 || nx >= newGrid.GetLength(1) || ny >= newGrid.GetLength(0)) continue;

                // If neighbour is alive, infect it.
                if (newGrid[ny, nx] == true)
                {
                    newGrid[ny, nx] = false;
                    zombies[ny, nx] = true;
                }
            }
        }
        // Zombies die of starvation if they have no living neighbours.
        if (neighbours == 0)
        {
            zombies[y, x] = false;
        }
        else
        {
            zombies[y, x] = true;
        }
    }
}
