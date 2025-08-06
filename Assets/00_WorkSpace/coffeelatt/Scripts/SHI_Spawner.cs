using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class Spawner : MonoBehaviour
{
    [Header("스폰할 프리팹들")]
    public List<GameObject> prefabList;
    public List<Vector3> _spwanPoint;
    [Header("스폰 주기 (초)")]
    public float spawnInterval = 120f;

    [Header("스폰 대상 바닥 오브젝트")]
    public GameObject groundObject;

    [Header("스폰 개수 (주기마다 생성할 수량)")]
    public int spawnCount = 3;

    private Bounds groundBounds;
    public int dropheight = 10; // 드랍 높이 (기본값 10)

    void Start()
    {
        // 바닥의 Bounds 가져오기 (Collider > Renderer 우선)
        if (groundObject.TryGetComponent<Collider>(out Collider col))
        {
            groundBounds = col.bounds;
        }
        else if (groundObject.TryGetComponent<Renderer>(out Renderer rend))
        {
            groundBounds = rend.bounds;
        }
        else
        {
            Debug.LogError("GroundObject에 Collider 또는 Renderer가 없습니다.");
            enabled = false;
            return;
        }

        // 스폰 반복 시작
        StartCoroutine(SpawnLoop());
    }
    IEnumerator destory(GameObject prefab)
    {
        yield return new WaitForSeconds(spawnInterval/2);
        Destroy(prefab);
    }
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            
                _spwanPoint = SHI_RandomPosCreater.RandomPosList(groundBounds.min, groundBounds.max, spawnCount);
                SpawnRandom();
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRandom()
    {
        
        for(int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = prefabList[Random.Range(0, prefabList.Count)];
            int random = Random.Range(0, _spwanPoint.Count);
            prefab.transform.position = _spwanPoint[random];
            _spwanPoint.RemoveAt(random);
            Instantiate(prefab, prefab.transform.position,prefab.transform.rotation);
            StartCoroutine(destory(prefab));
        }
        
       
    }
}