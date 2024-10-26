using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Tooltip("The wall prefab used to build the maze.")]
    public GameObject wallPrefab;

    [Tooltip("The plane on which the maze should be generated.")]
    public GameObject plane;

    [Tooltip("The dimensions of the maze (width and length in cells).")]
    public int mazeWidth = 10;
    public int mazeLength = 10;

    [Tooltip("Size of each cell in the maze.")]
    public float cellSize = 2f;

    [Tooltip("Height of the walls in the maze.")]
    public float wallHeight = 3f;

    private int[,] mazeGrid;

    private void Start()
    {
        GenerateMaze();
    }

    private void GenerateMaze()
    {
        // Initialize the grid with all walls.
        mazeGrid = new int[mazeWidth, mazeLength];
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeLength; z++)
            {
                mazeGrid[x, z] = 1; // 1 represents a wall
            }
        }

        // Start carving passages from the top-left corner.
        CarvePassages(0, 0);

        // Get the plane's bounds.
        MeshRenderer planeMeshRenderer = plane.GetComponent<MeshRenderer>();
        if (planeMeshRenderer == null)
        {
            Debug.LogError("Plane GameObject must have a MeshRenderer component.");
            return;
        }

        // Convert the maze grid to walls and passages.
        RenderMaze();
    }

    private void CarvePassages(int x, int z)
    {
        // Define directions: up, right, down, left
        int[] dx = { 0, 1, 0, -1 };
        int[] dz = { -1, 0, 1, 0 };

        // Shuffle directions to add randomness
        for (int i = 0; i < 4; i++)
        {
            int swapIndex = Random.Range(0, 4);
            int tempX = dx[i];
            int tempZ = dz[i];
            dx[i] = dx[swapIndex];
            dz[i] = dz[swapIndex];
            dx[swapIndex] = tempX;
            dz[swapIndex] = tempZ;
        }

        mazeGrid[x, z] = 0; // 0 represents a passage

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i] * 2;
            int nz = z + dz[i] * 2;

            if (nx >= 0 && nx < mazeWidth && nz >= 0 && nz < mazeLength && mazeGrid[nx, nz] == 1)
            {
                mazeGrid[x + dx[i], z + dz[i]] = 0; // Carve a passage between cells
                CarvePassages(nx, nz);
            }
        }
    }

    private void RenderMaze()
    {
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeLength; z++)
            {
                if (mazeGrid[x, z] == 1) // If it's a wall
                {
                    Vector3 position = new Vector3(x * cellSize - mazeWidth * cellSize / 2, wallHeight / 2, z * cellSize - mazeLength * cellSize / 2);
                    GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
                    wall.transform.localScale = new Vector3(cellSize, wallHeight, cellSize);
                    wall.transform.SetParent(plane.transform);
                }
            }
        }
    }
}
