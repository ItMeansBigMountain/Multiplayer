using UnityEngine;

public class material_decay : MonoBehaviour
{
    public float force = 750f;
    public float radius = 4f;
    public float destructionDelay = 5f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[GORILLA TRIGGER] touched: {other.gameObject.name}");

        if (!IsSmashable(other.gameObject))
        {
            Debug.Log($"[SKIP] {other.gameObject.name} is not tagged 'Obstacle'.");
            return;
        }

        Rigidbody rb = other.attachedRigidbody ?? AddRigidbody(other.gameObject);
        ApplySmashForce(rb, other.ClosestPoint(transform.position));
    }

    bool IsSmashable(GameObject obj)
    {
        return obj.CompareTag("obstacle");
    }

    Rigidbody AddRigidbody(GameObject obj)
    {
        Debug.Log($"[RIGIDBODY ADDED] to {obj.name}");
        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        return rb;
    }

    void ApplySmashForce(Rigidbody rb, Vector3 contactPoint)
    {
        if (rb == null) return;

        rb.isKinematic = false;
        rb.AddExplosionForce(force, contactPoint, radius);

        Debug.Log($"[SMASH] {rb.gameObject.name} hit with force {force}");

        Destroy(rb.gameObject, destructionDelay);
    }
}
