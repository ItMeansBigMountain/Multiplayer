using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        float percent = Mathf.Clamp01((float)currentHealth / maxHealth);
        transform.localScale = new Vector3(originalScale.x * percent, originalScale.y, originalScale.z);
    }
}
