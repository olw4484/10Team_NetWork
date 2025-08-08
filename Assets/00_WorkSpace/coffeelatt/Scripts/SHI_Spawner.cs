using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public class Spawner : MonoBehaviour
{
    [Header("스폰할 프리팹들")]
    //public List<SHI_ItembaseData> ItembaseData;
    //[SerializeField] private GameObject _itemPrefab;
    public prefabSo prefab;
    public List<Vector3> _spwanPoint;
    [Header("스폰 주기 (초)")]
    public float spawnInterval = 120f;

    [Header("스폰 대상 바닥 오브젝트")]
    public GameObject groundObject;

    [Header("스폰 개수 (주기마다 생성할 수량)")]
    public int spawnCount = 3;

    private Bounds groundBounds;
    public int dropheight = 10; // 드랍 높이 (기본값 10)
    public PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        if (!PhotonNetwork.IsMasterClient) return; // 내 객체가 아니라면 RPC 호출 안 함

        if (groundObject.TryGetComponent<Collider>(out Collider col))
            groundBounds = col.bounds;
        else if (groundObject.TryGetComponent<Renderer>(out Renderer rend))
            groundBounds = rend.bounds;
        else
        {
            Debug.LogError("GroundObject에 Collider 또는 Renderer가 없습니다.");
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

                // 모든 클라이언트에게 스폰 정보 전달
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