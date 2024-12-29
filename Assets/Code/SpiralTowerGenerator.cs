using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpiralTowerGenerator : MonoBehaviour
{
    [Header("Tower Settings")]
    [Tooltip("The list of all possible platform prefabs.")]
    public List<GameObject> platformPrefabs;

    [Tooltip("Number of platforms in the spiral.")]
    public int platformCount = 50;

    [Tooltip("Vertical distance between each platform.")]
    public float heightStep = 2f;

    [Tooltip("Radius of the spiral.")]
    public float spiralRadius = 5f;

    [Tooltip("Angle increment between platforms.")]
    public float angleStep = 20f;

    [Tooltip("The floor plane for positioning.")]
    public GameObject floorPlane;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) // Ensure only the Master Client generates the map
        {
            GenerateSpiralTower();
        }
    }

    private void GenerateSpiralTower()
    {
        // Get the center of the floor plane
        Renderer floorRenderer = floorPlane.GetComponent<Renderer>();
        if (floorRenderer == null)
        {
            Debug.LogError("Floor plane must have a Renderer component.");
            return;
        }

        Vector3 floorCenter = floorRenderer.bounds.center;

        float currentAngle = 0f; // Angle for spiral placement
        float currentHeight = floorCenter.y + 1f; // Slight offset above the floor

        for (int i = 0; i < platformCount; i++)
        {
            // Select a random platform prefab
            GameObject platformPrefab = platformPrefabs[Random.Range(0, platformPrefabs.Count)];

            // Calculate position relative to the center of the floor
            float x = floorCenter.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * spiralRadius;
            float z = floorCenter.z + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * spiralRadius;
            Vector3 position = new Vector3(x, currentHeight, z);

            // Spawn platform across the network
            GameObject platform = PhotonNetwork.Instantiate(platformPrefab.name, position, Quaternion.identity);

            // Optional: Slight random rotation for visual variation
            platform.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // Increment angle and height for spiral effect
            currentAngle += angleStep;
            currentHeight += heightStep;
        }

        Debug.Log("Spiral Tower generated successfully at the center of the floor!");
    }
}
