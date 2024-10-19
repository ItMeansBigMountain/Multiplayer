using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; 
    [SerializeField] private GameObject mapPlane; 

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        // Get the bounds of the map plane
        Renderer mapRenderer = mapPlane.GetComponent<Renderer>();
        Vector3 mapSize = mapRenderer.bounds.size;

        // Calculate a random spawn position near the perimeter of the plane
        float xPos = Random.value > 0.5f ? Random.Range(-mapSize.x / 2, -mapSize.x / 2 + 10f) : Random.Range(mapSize.x / 2 - 10f, mapSize.x / 2);
        float zPos = Random.value > 0.5f ? Random.Range(-mapSize.z / 2, -mapSize.z / 2 + 10f) : Random.Range(mapSize.z / 2 - 10f, mapSize.z / 2);

        Vector3 spawnPosition = new Vector3(xPos, mapPlane.transform.position.y + 1f, zPos);

        // Instantiate the player at the calculated position
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
    }
}
