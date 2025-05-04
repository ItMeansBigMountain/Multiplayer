using UnityEngine;

public class material_decay : MonoBehaviour
{
    public float force = 750f;
    public float radius = 4f;
    public float destructionDelay = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == this.gameObject) return;

        string topParent = GetRootParentName(other.gameObject);
        Debug.Log($"[DETECTED] Triggered by: {other.name} (Root: {topParent})");

        if (other.CompareTag("floor"))
        {
            Debug.Log($"[SKIP] {other.name} is floor");
            return;
        }

        if (!other.CompareTag("obstacle"))
        {
            Debug.Log($"[SKIP] {other.name} is not tagged 'obstacle'");
            return;
        }

        Debug.Log($"[VALID] Would apply force to: {other.name} (Root: {topParent})");
    }

    void TrySmash(GameObject target, Vector3 hitPoint)
    {
        if (!target.CompareTag("obstacle"))
        {
            Debug.Log($"[SKIP] {target.name} is not tagged 'obstacle'");
            return;
        }

        string topParent = GetRootParentName(target);
        Debug.Log($"[WOULD SMASH] {target.name} (Root: {topParent}) at {hitPoint}");

        // Rigidbody rb = target.GetComponent<Rigidbody>() ?? target.AddComponent<Rigidbody>();
        // rb.isKinematic = false;
        // rb.interpolation = RigidbodyInterpolation.Interpolate;
        // rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        // rb.AddExplosionForce(force, hitPoint, radius);
        // Destroy(target, destructionDelay);
    }

    string GetRootParentName(GameObject obj)
    {
        Transform current = obj.transform;
        while (current.parent != null)
            current = current.parent;
        return current.name;
    }
}
