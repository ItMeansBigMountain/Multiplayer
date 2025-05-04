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
    public float heightStep = 0.5f;

    [Tooltip("Radius of the spiral.")]
    public float spiralRadius = 8f;

    [Tooltip("Angle increment between platforms.")]
    public float angleStep = 10f;

    [Tooltip("The floor plane for positioning.")]
    public GameObject floorPlane;

    [Tooltip("Height of the invisible walls.")]
    public float wallHeight = 5f;

    [Tooltip("Height of the ceiling above the plane.")]
    public float ceilingHeight = 3f;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateSpiralTower();
        }
    }

    private void GenerateSpiralTower()
    {
        Renderer floorRenderer = floorPlane.GetComponent<Renderer>();
        if (floorRenderer == null)
        {
            Debug.LogError("Floor plane must have a Renderer component.");
            return;
        }

        Vector3 floorSize = floorRenderer.bounds.size;
        float mapWidth = floorSize.x;
        float mapLength = floorSize.z;
        Debug.Log($"Map Dimensions: Width = {mapWidth}, Length = {mapLength}");

        Vector3 floorCenter = floorRenderer.bounds.center;
        float currentAngle = 0f;
        float currentHeight = floorCenter.y + 1f;

        for (int i = 0; i < platformCount; i++)
        {
            GameObject platformPrefab = platformPrefabs[Random.Range(0, platformPrefabs.Count)];

            float x = floorCenter.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * spiralRadius;
            float z = floorCenter.z + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * spiralRadius;
            Vector3 position = new Vector3(x, currentHeight, z);

            GameObject platform = PhotonNetwork.Instantiate(platformPrefab.name, position, Quaternion.identity);

            StripRigidbody(platform);

            platform.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            currentAngle += angleStep;
            currentHeight += heightStep;
        }

        Debug.Log("Spiral Tower generated successfully at the center of the floor!");

        float halfPlaneWidth = mapWidth / 2f;
        float halfPlaneLength = mapLength / 2f;
        CreateInvisibleWallsAndCeiling(halfPlaneWidth, halfPlaneLength);
    }

    private void StripRigidbody(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);
    }

    private void CreateInvisibleWallsAndCeiling(float halfWidth, float halfLength)
    {
        CreateInvisibleWall(new Vector3(-halfWidth, wallHeight / 2f, 0), new Vector3(0.1f, wallHeight, halfLength * 2));
        CreateInvisibleWall(new Vector3(halfWidth, wallHeight / 2f, 0), new Vector3(0.1f, wallHeight, halfLength * 2));
        CreateInvisibleWall(new Vector3(0, wallHeight / 2f, halfLength), new Vector3(halfWidth * 2, wallHeight, 0.1f));
        CreateInvisibleWall(new Vector3(0, wallHeight / 2f, -halfLength), new Vector3(halfWidth * 2, wallHeight, 0.1f));
        CreateInvisibleWall(new Vector3(0, ceilingHeight, 0), new Vector3(halfWidth * 2, 0.1f, halfLength * 2));
    }

    private void CreateInvisibleWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = new GameObject("Invisible Wall");
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.transform.SetParent(floorPlane.transform);

        BoxCollider wallCollider = wall.AddComponent<BoxCollider>();
        wallCollider.isTrigger = false;

        PhotonView photonView = wall.AddComponent<PhotonView>();
        wall.AddComponent<PhotonTransformView>();

        if (!PhotonNetwork.AllocateViewID(photonView))
        {
            Debug.LogError("Failed to allocate PhotonView ID for invisible wall.");
        }
    }
}
