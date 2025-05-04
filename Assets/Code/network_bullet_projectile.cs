using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody), typeof(PhotonRigidbodyView))]
public class network_bullet_projectile : MonoBehaviourPunCallbacks
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 20f;           // Speed of the bullet
    [SerializeField] private float lifetime = 3f;        // Lifetime before destruction
    [SerializeField] private int damage = 20;            // Damage dealt to players
    [SerializeField] private GameObject hitEffectPrefab; // Effect when the bullet hits something

    private Rigidbody rb;

    private void Awake()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Make the Rigidbody kinematic for non-owners to avoid physics conflicts
        if (!photonView.IsMine)
        {
            rb.isKinematic = true;
        }
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            // Set initial velocity in the forward direction
            rb.linearVelocity = transform.forward * speed;

            // Destroy the bullet after a certain time to prevent lingering
            Invoke("DestroyBullet", lifetime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Trigger Bullet hit: {other.name}");


        if (photonView.IsMine)
        {
            // Check if the collided object has the player script
            ArenaShooter_PlayerControls player = other.GetComponent<ArenaShooter_PlayerControls>();

            if (player != null)
            {
                // Apply damage to the player
                player.ApplyDamage(damage);
            }

            // Spawn a hit effect (local only, not networked)
            if (hitEffectPrefab != null)
            {
                PhotonNetwork.Instantiate("Hit_03", transform.position, Quaternion.identity);
            }

            // Destroy the bullet across the network
            PhotonNetwork.Destroy(gameObject);

            Debug.Log($"Bullet hit: {other.name}");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"Collision Bullet hit: {collision.gameObject.name}");


        if (photonView.IsMine)
        {
            // Check if the collided object has the player script
            ArenaShooter_PlayerControls player = collision.gameObject.GetComponentInParent<ArenaShooter_PlayerControls>();

            if (player != null)
            {
                // Apply damage to the player
                player.ApplyDamage(damage);

                if (player.currentHealth > 0)
                {
                    Debug.Log($"{collision.transform.parent.root.gameObject.name} took {damage} damage.");
                }
                else
                {
                    Debug.Log($"{transform.parent.root.gameObject.name} has killed {collision.transform.parent.root.gameObject.name}!");
                }

            }

            // Spawn a hit effect (local only, not networked)
            if (hitEffectPrefab != null)
            {
                PhotonNetwork.Instantiate("Hit_03", transform.position, Quaternion.identity);
            }

            // Destroy the bullet across the network
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void DestroyBullet()
    {
        // Safely destroy the bullet after its lifetime expires
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
