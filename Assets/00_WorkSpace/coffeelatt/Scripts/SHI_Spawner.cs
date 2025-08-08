using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public class Spawner : MonoBehaviour
{
    [Header("������ �����յ�")]
    //public List<SHI_ItembaseData> ItembaseData;
    //[SerializeField] private GameObject _itemPrefab;
    public prefabSo prefab;
    public List<Vector3> _spwanPoint;
    [Header("���� �ֱ� (��)")]
    public float spawnInterval = 120f;

    [Header("���� ��� �ٴ� ������Ʈ")]
    public GameObject groundObject;

    [Header("���� ���� (�ֱ⸶�� ������ ����)")]
    public int spawnCount = 3;

    private Bounds groundBounds;
    public int dropheight = 10; // ��� ���� (�⺻�� 10)
    public PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        if (!PhotonNetwork.IsMasterClient) return; // �� ��ü�� �ƴ϶�� RPC ȣ�� �� ��

        if (groundObject.TryGetComponent<Collider>(out Collider col))
            groundBounds = col.bounds;
        else if (groundObject.TryGetComponent<Renderer>(out Renderer rend))
            groundBounds = rend.bounds;
        else
        {
            Debug.LogError("GroundObject�� Collider �Ǵ� Renderer�� �����ϴ�.");
            enabled = false;
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        StartCoroutine(SpawnLoop());
    }
    IEnumerator Destory(GameObject prefab)
    {
        yield return new WaitForSeconds(spawnInterval / 2);
        prefab.SetActive(false);
    }

    public void setspawn()
    {
        _spwanPoint = SHI_RandomPosCreater.RandomPosList(groundBounds.min, groundBounds.max, spawnCount);
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            List<Vector3> spawnPoints = SHI_RandomPosCreater.RandomPosList(groundBounds.min, groundBounds.max, spawnCount);

            for (int i = 0; i < spawnCount; i++)
            {
                GameObject itemPrefab = prefab.GetRandomPrefab();
                string prefabName = itemPrefab.name;
                Vector3 spawnPos = spawnPoints[i];

                // ��� Ŭ���̾�Ʈ���� ���� ���� ����
                pv.RPC(nameof(SpawnItem), RpcTarget.All, prefabName, spawnPos);
            }

            //_spwanPoint = SHI_RandomPosCreater.RandomPosList(groundBounds.min, groundBounds.max, spawnCount);


            // SpawnRandom();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    [PunRPC]
    void SpawnItem(string prefabName, Vector3 position)
    {
        GameObject spawnItem = PhotonNetwork.Instantiate(prefabName, position + Vector3.up * dropheight, Quaternion.identity);
        spawnItem.transform.parent = this.transform;
    }

    [PunRPC]
    void SpawnRandom()
    {
        if (PhotonNetwork.IsMasterClient)

            for (int i = 0; i < spawnCount; i++)
            {
                GameObject item = prefab.GetRandomPrefab();
                int random = Random.Range(0, _spwanPoint.Count);
                Vector3 spawnpoint = _spwanPoint[random];

                GameObject spawnitem = Instantiate(item, spawnpoint, item.transform.rotation);
                spawnitem.transform.parent = this.gameObject.transform;
                //SHI_ItemBase itemBase = spawnitem.GetComponent<SHI_ItemBase>();
                //itemBase.get(ItembaseData[Random.Range(0, ItembaseData.Count)]);
                //itemBase._Image = itemBase.data._Image;
                //itemBase.GetComponent<SpriteRenderer>().sprite = itemBase._Image;


                _spwanPoint.RemoveAt(random);
                Destory(spawnitem);
                //Instantiate(prefab, spawnpoint, prefab.transform.rotation);
                //GameObject create = Instantiate(prefab, spawnpoint, prefab.transform.rotation);
                StartCoroutine(Destory(spawnitem));
            }
    }

   //[PunRPC]
   //public void RPC_SpawnItemSync(Vector3 pos, int prefabIndex)
   //{
   //    GameObject item = prefab.GetPrefabAtIndex(prefabIndex);
   //    GameObject spawnitem = Instantiate(item, pos, item.transform.rotation);
   //    spawnitem.transform.parent = this.transform;
   //    StartCoroutine(DestroyAfterTime(spawnitem));
   //}
}