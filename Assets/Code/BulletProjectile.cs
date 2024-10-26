using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
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
        Instantiate(vfx_Hit_red, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }



}
