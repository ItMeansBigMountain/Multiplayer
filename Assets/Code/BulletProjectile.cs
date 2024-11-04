using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletProjectile : MonoBehaviourPun
{
    [SerializeField] public float speed = 100f;
    [SerializeField] public Transform vfx_Hit_red;

    private Rigidbody bulletRigidBody;

    private void Awake()
    {
        bulletRigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        bulletRigidBody.linearVelocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Only the master client should handle the destruction and instantiation of effects
        if (photonView.IsMine)
        {
            // Instantiate the hit effect
            PhotonNetwork.Instantiate(vfx_Hit_red.name, transform.position, Quaternion.identity);

            // Destroy the bullet across the network
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
