using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody), typeof(PhotonRigidbodyView))]
public class network_bullet_projectile : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 20f;         
    [SerializeField] private float lifetime = 3f;       
    [SerializeField] private GameObject hitEffectPrefab;

    private Rigidbody rb;

    private void Awake()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Ensure the Rigidbody is kinematic for non-owners to avoid physics conflicts
        if (!photonView.IsMine)
        {
            rb.isKinematic = true;
        }
    }

    private void Start()
    {
        // Set the initial velocity in the forward direction
        if (photonView.IsMine)
        {
            // USING TRANSFORM
            // transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // USING GRAVITY
            rb.linearVelocity = transform.forward * speed;
            Invoke("DestroyBullet", lifetime);
        }
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
