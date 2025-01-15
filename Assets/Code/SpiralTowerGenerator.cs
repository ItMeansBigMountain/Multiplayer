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

    [Tooltip("Height of the invisible walls.")]
    public float wallHeight = 5f;

    [Tooltip("Height of the ceiling above the plane.")]
    public float ceilingHeight = 3f;


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

        // Calculate the dimensions of the floor
        Vector3 floorSize = floorRenderer.bounds.size;
        float mapWidth = floorSize.x;
        float mapLength = floorSize.z;
        Debug.Log($"Map Dimensions: Width = {mapWidth}, Length = {mapLength}");


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


        // Spawn the invisible walls and ceiling
        float halfPlaneWidth = mapWidth / 2f;
        float halfPlaneLength = mapLength / 2f;
        CreateInvisibleWallsAndCeiling(halfPlaneWidth, halfPlaneLength);
    }

    private void CreateInvisibleWallsAndCeiling(float halfWidth, float halfLength)
    {
        // Create walls and ceiling here using PhotonNetwork.InstantiateRoomObject for non-visible objects
        CreateInvisibleWall(new Vector3(-halfWidth, wallHeight / 2f, 0), new Vector3(0.1f, wallHeight, halfLength * 2));
        CreateInvisibleWall(new Vector3(halfWidth, wallHeight / 2f, 0), new Vector3(0.1f, wallHeight, halfLength * 2));
        CreateInvisibleWall(new Vector3(0, wallHeight / 2f, halfLength), new Vector3(halfWidth * 2, wallHeight, 0.1f));
        CreateInvisibleWall(new Vector3(0, wallHeight / 2f, -halfLength), new Vector3(halfWidth * 2, wallHeight, 0.1f));
        CreateInvisibleWall(new Vector3(0, ceilingHeight, 0), new Vector3(halfWidth * 2, 0.1f, halfLength * 2));
    }

    private void CreateInvisibleWall(Vector3 position, Vector3 scale)
    {
        // Create a new GameObject for the invisible wall
        GameObject wall = new GameObject("Invisible Wall");
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.transform.SetParent(floorPlane.transform);

        // Add a BoxCollider component to the wall
        BoxCollider wallCollider = wall.AddComponent<BoxCollider>();
        wallCollider.isTrigger = false;

        // Add PhotonView and Transform Sync Components
        PhotonView photonView = wall.AddComponent<PhotonView>();
        wall.AddComponent<PhotonTransformView>();

        // Assign a unique PhotonView ID
        if (PhotonNetwork.AllocateViewID(photonView))
        {
            // Successfully assigned a unique ID to the PhotonView
            //Debug.Log($"Successfully allocated PhotonView ID: {photonView.ViewID} for {wall.name}");
        }
        else
        {
            // Failed to allocate a unique ID
            Debug.LogError("Failed to allocate PhotonView ID for invisible wall.");
        }
    }
}
