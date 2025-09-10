using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Grid properties.
    public int[,] cells;
    int[,] newGrid;
    public GameObject liveCell;
    public GameObject deadCell;
    Vector3 gridOffset;

    private void Start()
    {
        gridOffset = new Vector3(-8, -5, 0);
        cells = InitializeGrid(10, 15);
        DrawGrid(cells);
    }

    private int[,] InitializeGrid(int height, int width)
    {
        cells = new int[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[y, x] = Random.Range(0, 2);
            }
        }
        return cells;
    }

    private void DrawGrid(int[,] grid)
    {
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                GameObject cell;
                if (grid[y, x] == 1)
                {
                    // Cell is alive.
                    cell = Instantiate(liveCell);
                }
                else
                {
                    cell = Instantiate(deadCell);
                }
                cell.transform.position = new Vector3Int(x, y) + gridOffset;
                int neighbourCount = CheckNeighbours(y, x);
            }
        }
    }

    private int CheckNeighbours(int gridY, int gridX)
    {
        newGrid = new int[gridY, gridX];
        int neighbourCount = 0;
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (y == 0 && x == 0) continue; // skip self.

                int neighbourX = gridX + x;
                int neighbourY = gridY + y;

                // Check surrounding neighbours.
                if (newGrid[neighbourX, neighbourY] == 1)
                {
                    neighbourCount++;
                }
            }
        }
        return neighbourCount;
    }

    private void ApplyRules(int[,] grid, int neighbourCount)
    {
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {

            }
        }
        if (neighbourCount == 2 || neighbourCount == 3)
    }
}
