using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class Spawner : MonoBehaviour
{
    [Header("������ �����յ�")]
    public List<GameObject> prefabList;
    public List<Vector3> _spwanPoint;
    [Header("���� �ֱ� (��)")]
    public float spawnInterval = 120f;

    [Header("���� ��� �ٴ� ������Ʈ")]
    public GameObject groundObject;

    [Header("���� ���� (�ֱ⸶�� ������ ����)")]
    public int spawnCount = 3;

    private Bounds groundBounds;
    public int dropheight = 10; // ��� ���� (�⺻�� 10)

    void Start()
    {
        // �ٴ��� Bounds �������� (Collider > Renderer �켱)
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
            Debug.LogError("GroundObject�� Collider �Ǵ� Renderer�� �����ϴ�.");
            enabled = false;
            return;
        }

        // ���� �ݺ� ����
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