using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Tooltip("The list of all possible obstacle prefabs.")]
    public List<GameObject> obstaclePrefabs;

    [Tooltip("The number of obstacles to spawn.")]
    public int numObstacles;

    [Tooltip("The minimum distance between obstacles.")]
    public float minObstacleDistance;

    [Tooltip("The maximum distance between obstacles.")]
    public float maxObstacleDistance;

    [Tooltip("The plane on which obstacles should be spawned.")]
    public GameObject plane;

    private void Start()
    {
        // Get the size of the plane.
        MeshRenderer planeMeshRenderer = plane.GetComponent<MeshRenderer>();
        if (planeMeshRenderer == null)
        {
            Debug.LogError("Plane GameObject must have a MeshRenderer component.");
            return;
        }

        Vector3 planeSize = planeMeshRenderer.bounds.size;

        // Get the bounds of the plane.
        float halfPlaneWidth = planeSize.x / 2f;
        float halfPlaneLength = planeSize.z / 2f;

        // Spawn the obstacles.
        for (int i = 0; i < numObstacles; i++)
        {
            // Get a random obstacle prefab.
            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];

            // Get a random position for the obstacle within the plane bounds.
            float randomX = Random.Range(-halfPlaneWidth, halfPlaneWidth);
            float randomZ = Random.Range(-halfPlaneLength, halfPlaneLength);
            Vector3 position = new Vector3(randomX, 0f, randomZ); // Ensure Y-axis is set to 0 to be on the plane.

            // Adjust the Y-axis to make sure the obstacle height does not exceed 3.
            position.y = Mathf.Clamp(position.y, 0f, 3f);

            // Random rotation around Y-axis.
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // Spawn the obstacle at the calculated position with random rotation.
            GameObject obstacle = Instantiate(obstaclePrefab, position, randomRotation);

            // Optionally, you can adjust the parent to the plane for better organization.
            obstacle.transform.SetParent(plane.transform);
        }
    }
}
