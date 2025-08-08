using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHT_DataManager : MonoBehaviour, IManager
{
    #region Data

    public Dictionary<string, AudioClip> audioDic;

    #endregion

    #region Class

    

    #endregion

    #region IManager
    public bool IsDontDestroy => false;

    public void Initialize()
    {

        audioDic = new();
        AudioClip[] objs = Resources.LoadAll<AudioClip>("Sound/UISound");
        for (int i = 0; i < objs.Length; i++)
        {
            audioDic.Add(objs[i].name, objs[i]);
        }

    }

    public void Cleanup() 
    {
        Clean();
    }

    public GameObject GetGameObject() => this.gameObject;

    #endregion

    private void Clean()
    {
        for (int i = 0; i < audioDic.Count; i++)
        {
            audioDic.Clear();
        }
    }


}
