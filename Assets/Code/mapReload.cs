using UnityEngine;
using Photon.Pun;

public class mapReload : MonoBehaviour
{
    [Header("Debugging")]
    public bool reload = false;
    public bool spiralMap = false;
    public bool arenaMap = false;

    public SpiralTowerGenerator spiralGenerator;
    public random_gen_arena arenaGenerator;

    void Start()
    {
        // Automatically fetch references from attached components
        spiralGenerator = GetComponent<SpiralTowerGenerator>();
        arenaGenerator = GetComponent<random_gen_arena>();
    }

    void Update()
    {
        if (reload)
        {
            reload = false;
            ReloadMap();
        }
    }

    private void ReloadMap()
    {
        Debug.Log("[MAP RELOAD] Started");

        DestroyAllTagged("obstacle");

        if (arenaMap) arenaGenerator.GenerateMap();
        if (arenaMap) spiralGenerator.GenerateSpiralTower();


        Debug.Log("[MAP RELOAD] Done.");
    }

    private void DestroyAllTagged(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in targets)
        {
            PhotonNetwork.Destroy(obj);
        }
    }
}
