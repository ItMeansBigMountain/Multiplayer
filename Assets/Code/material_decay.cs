using UnityEngine;

public class material_decay : MonoBehaviour
{
    public float force = 750f;
    public float radius = 4f;
    public float destructionDelay = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsSmashable(collision.gameObject)) return;

        ApplySmashForce(collision.rigidbody, collision.contacts[0].point);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsSmashable(other.gameObject)) return;

        ApplySmashForce(other.attachedRigidbody, other.ClosestPoint(transform.position));
    }

    bool IsSmashable(GameObject obj)
    {
        return obj.CompareTag("Destructible") && !obj.CompareTag("Floor");
    }

    void ApplySmashForce(Rigidbody rb, Vector3 contactPoint)
    {
        if (rb == null) return;

        rb.isKinematic = false;
        rb.AddExplosionForce(force, contactPoint, radius);

        Destroy(rb.gameObject, destructionDelay);
    }
}
