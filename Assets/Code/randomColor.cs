using UnityEngine;

public class RandomAppearance : MonoBehaviour
{
    [Header("Random Appearance Settings")]
    public bool randomizeColor = true;
    public bool randomizeMaterial = true;

    [Tooltip("List of materials for different surfaces (metal, plastic, etc.)")]
    public Material[] materialOptions;

    // List of possible random colors
    private Color[] colorOptions = {
        Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.black
    };

    private Renderer objectRenderer;

    void Start()
    {
        // Get the Renderer component
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer component found on the GameObject.");
            return;
        }

        // Randomize material
        if (randomizeMaterial && materialOptions.Length > 0)
        {
            ApplyRandomMaterial();
        }

        // Randomize color
        if (randomizeColor)
        {
            ApplyRandomColor();
        }
    }

    private void ApplyRandomMaterial()
    {
        // Choose a random material and apply it to the object
        Material randomMaterial = materialOptions[Random.Range(0, materialOptions.Length)];
        objectRenderer.material = randomMaterial;
    }

    private void ApplyRandomColor()
    {
        // Randomize color within the current material
        Color randomColor = colorOptions[Random.Range(0, colorOptions.Length)];
        objectRenderer.material.color = randomColor;
    }
}
