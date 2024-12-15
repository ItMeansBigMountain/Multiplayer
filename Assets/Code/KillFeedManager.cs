using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // If you're using TextMeshPro

public class KillFeedManager : MonoBehaviour
{
    public GameObject killFeedEntryPrefab; // Assign your KillFeedEntry prefab here
    public Transform killFeedContent; // Assign the Content of the Scroll View
    public float entryLifetime = 5f; // Time each entry stays in the feed

    // Method to add a new kill message
    public void AddKillFeedEntry(string killer, string victim)
    {
        // Instantiate a new kill feed entry
        GameObject newEntry = Instantiate(killFeedEntryPrefab, killFeedContent);

        // Set the text for the entry
        TMP_Text entryText = newEntry.GetComponent<TMP_Text>();
        if (entryText != null)
        {
            entryText.text = $"{killer} killed {victim}";
        }

        // Start coroutine to remove the entry after a delay
        StartCoroutine(RemoveEntryAfterDelay(newEntry, entryLifetime));
    }

    // Coroutine to remove the entry
    private IEnumerator RemoveEntryAfterDelay(GameObject entry, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (entry != null)
        {
            Destroy(entry);
        }
    }
}
