using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // If you're using TextMeshPro

public class KillFeedManager : MonoBehaviour
{
    public GameObject killFeedEntryPrefab; // Assign your KillFeedEntry prefab here
    public Transform killFeedContent; // Assign the Content of the Scroll View
    public float entryLifetime = 5f; // Time each entry stays in the feed

    public void AddKillFeedEntry(GameObject killer, GameObject victim)
    {
        // Instantiate a new kill feed entry
        GameObject newEntry = Instantiate(killFeedEntryPrefab, killFeedContent);

        // Set the text for the entry
        TMP_Text entryText = newEntry.GetComponent<TMP_Text>();
        if (entryText != null)
        {
            entryText.text = $"{killer.name} killed {victim.name}";
        }

        // Start coroutine to remove the entry after a delay
        StartCoroutine(RemoveEntryAfterDelay(newEntry, entryLifetime));
    }

    public void AddKillFeedEntry_Hit(GameObject killer, GameObject victim)
    {
        // Instantiate a new kill feed entry
        GameObject newEntry = Instantiate(killFeedEntryPrefab, killFeedContent);

        // Set the text for the entry
        TMP_Text entryText = newEntry.GetComponent<TMP_Text>();
        if (entryText != null)
        {
            entryText.text = $"{killer.name} hit {victim.name}";
        }

        // Start coroutine to remove the entry after a delay
        StartCoroutine(RemoveEntryAfterDelay(newEntry, entryLifetime));
    }



    private IEnumerator RemoveEntryAfterDelay(GameObject entry, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (entry != null)
        {
            Destroy(entry);
        }
    }
}
