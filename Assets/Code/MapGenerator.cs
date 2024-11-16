using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;

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

    [Tooltip("Height of the invisible walls.")]
    public float wallHeight = 5f;

    [Tooltip("Height of the ceiling above the plane.")]
    public float ceilingHeight = 3f;

    private void Start()
    {
        // Only the Master Client generates and synchronizes the map
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        // Get the size of the plane
        MeshRenderer planeMeshRenderer = plane.GetComponent<MeshRenderer>();
        if (planeMeshRenderer == null)
        {
            Debug.LogError("Plane GameObject must have a MeshRenderer component.");
            return;
        }

        Vector3 planeSize = planeMeshRenderer.bounds.size;
        float halfPlaneWidth = planeSize.x / 2f;
        float halfPlaneLength = planeSize.z / 2f;

        // Spawn obstacles using PhotonNetwork.Instantiate
        for (int i = 0; i < numObstacles; i++)
        {
            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
            float randomX = Random.Range(-halfPlaneWidth, halfPlaneWidth);
            float randomZ = Random.Range(-halfPlaneLength, halfPlaneLength);
            Vector3 position = new Vector3(randomX, 0f, randomZ);
            position.y = Mathf.Clamp(position.y, 0f, 3f);
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            GameObject obstacle = PhotonNetwork.Instantiate(obstaclePrefab.name, position, randomRotation);
            // obstacle.transform.SetParent(plane.transform);
        }

        // Spawn the invisible walls and ceiling
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
        wall.transform.SetParent(plane.transform);

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
