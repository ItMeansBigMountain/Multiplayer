using UnityEngine;
using Photon.Pun;
using System.Linq;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private GameObject mapPlane;

    // Public boolean for debugging
    public bool spawnPlayerForDebug = false;

    private void Start()
    {
        LoadCharacterPrefabs();

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
        else
        {
            Debug.LogWarning("Not connected to a Photon room. Player spawn canceled.");
        }

    }

    private void Update()
    {
        if (spawnPlayerForDebug)
        {
            SpawnPlayer();
            spawnPlayerForDebug=false;
        }
    }

    private void LoadCharacterPrefabs()
    {
        // Load all GameObjects from Resources folder and filter by prefix
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("")
            .Where(prefab => prefab.name.StartsWith("Multiplayer-Character-"))
            .ToArray();

        if (loadedPrefabs.Length > 0)
        {
            characterPrefabs = loadedPrefabs;
            Debug.Log($"{characterPrefabs.Length} character prefabs loaded.");
        }
        else
        {
            Debug.LogError("No character prefabs with the prefix 'Multiplayer-Character-' found in Resources.");
        }
    }

    public void SpawnPlayer()
    {
        // Check if mapPlane has a Renderer component
        Renderer mapRenderer = mapPlane.GetComponent<Renderer>();
        if (mapRenderer == null)
        {
            Debug.LogError("Map plane does not have a Renderer component.");
            return;
        }

        Vector3 mapSize = mapRenderer.bounds.size;

        // Calculate a random spawn position near the perimeter of the plane
        float xPos = Random.value > 0.5f ? Random.Range(-mapSize.x / 2, -mapSize.x / 2 + 10f) : Random.Range(mapSize.x / 2 - 10f, mapSize.x / 2);
        float zPos = Random.value > 0.5f ? Random.Range(-mapSize.z / 2, -mapSize.z / 2 + 10f) : Random.Range(mapSize.z / 2 - 10f, mapSize.z / 2);

        Vector3 spawnPosition = new Vector3(xPos, mapPlane.transform.position.y + 1f, zPos);

        // Calculate direction to the center of the map
        Vector3 mapCenter = mapPlane.transform.position;
        Vector3 directionToCenter = (mapCenter - spawnPosition).normalized;
        Quaternion rotationToCenter = Quaternion.LookRotation(directionToCenter, Vector3.up);

        // CHOOSE CHARACTER 
        if (characterPrefabs.Length > 0)
        {
            // RANDOM CHARACTER
            GameObject selectedPrefab = characterPrefabs[Random.Range(0, characterPrefabs.Length)];

            // Instantiate the selected prefab at the calculated position with the calculated rotation
            PhotonNetwork.Instantiate(selectedPrefab.name, spawnPosition, rotationToCenter);
        }
        else
        {
            Debug.LogError("No character prefabs available for spawning.");
        }
    }
}
