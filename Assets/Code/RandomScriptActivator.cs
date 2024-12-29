using UnityEngine;

public class RandomScriptActivator : MonoBehaviour
{
    private MonoBehaviour[] scripts;

    void Start()
    {
        // Find all MonoBehaviours on this GameObject
        scripts = GetComponents<MonoBehaviour>();

        // Filter to find the specific scripts (MapGenerator and SpiralTowerGenerator)
        MonoBehaviour mapGenerator = null;
        MonoBehaviour spiralTowerGenerator = null;

        foreach (MonoBehaviour script in scripts)
        {
            if (script.GetType().Name == "MapGenerator")
                mapGenerator = script;
            else if (script.GetType().Name == "SpiralTowerGenerator")
                spiralTowerGenerator = script;
        }

        // Ensure both scripts are found
        if (mapGenerator != null && spiralTowerGenerator != null)
        {
            // Randomly decide which script to activate
            bool chooseMapGenerator = Random.value > 0.5f;

            if (chooseMapGenerator)
            {
                ActivateScript(mapGenerator);
                DeactivateScript(spiralTowerGenerator);
                Debug.Log("MapGenerator script activated.");
            }
            else
            {
                ActivateScript(spiralTowerGenerator);
                DeactivateScript(mapGenerator);
                Debug.Log("SpiralTowerGenerator script activated.");
            }
        }
        else
        {
            Debug.LogError("Could not find MapGenerator or SpiralTowerGenerator scripts on the prefab.");
        }
    }

    private void ActivateScript(MonoBehaviour script)
    {
        script.enabled = true;
    }

    private void DeactivateScript(MonoBehaviour script)
    {
        script.enabled = false;
    }
}
