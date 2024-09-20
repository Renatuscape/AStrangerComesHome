using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionsMemories : MonoBehaviour
{
    public GameObject memoryPrefab;
    public GameObject prefabContainer;
    public List<GameObject> prefabs;

    private void OnEnable()
    {
        Clear();
        StartCoroutine(SpawnMemoryEntries());
    }
    private void OnDisable()
    {
        Clear();
    }

    IEnumerator SpawnMemoryEntries()
    {
        foreach (var memory in Memories.all)
        {
            var newMemory = Instantiate(memoryPrefab);

            prefabs.Add(newMemory);
            newMemory.transform.SetParent(prefabContainer.transform);
            int strength = Player.GetCount(memory.objectID, name);

            if (strength > 0)
            {
                var script = newMemory.GetComponent<CollectionsMemoryEntry>();
                script.EnableMemory(memory, strength);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void Clear()
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            Destroy(prefabs[i]);
        }

        prefabs.Clear();
    }
}
