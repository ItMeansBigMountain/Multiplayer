using UnityEngine;
using Photon.Pun;

public class network_bullet_projectile : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 20f;         
    [SerializeField] private float lifetime = 3f;       
    [SerializeField] private GameObject hitEffectPrefab;

    private void Start()
    {
        // Only the owner manages the bullet's lifetime
        if (photonView.IsMine)
        {
            Invoke("DestroyBullet", lifetime);
        }
    }

    private void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine) // Ensure only the owner handles collision
        {
            // Check for impact
            if (hitEffectPrefab != null)
            {
                // Instantiate hit effect locally (not networked)
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // Destroy the bullet across the network
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void DestroyBullet()
    {
        // Safely destroy the bullet after the lifetime
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
