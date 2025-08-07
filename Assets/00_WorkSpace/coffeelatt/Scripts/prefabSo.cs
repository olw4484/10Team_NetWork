using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class prefabSo : ScriptableObject
{
    
    public List<GameObject> prefabList;
    public void createpool()
    {
        for(int i =0; i < prefabList.Count; i++)
        {
            YSJ_PoolManager.Instance.CreatePool("prefabPool", prefabList[i], 1);
        }
       
    }
    public GameObject GetRandomPrefab()
    {
        if (prefabList == null || prefabList.Count == 0)
        {
            Debug.LogError("Prefab list is empty or not initialized.");
            return null;
        }

        int randomIndex = Random.Range(0, prefabList.Count);
        return prefabList[randomIndex];
    }

    public int Count => prefabList.Count;
}

