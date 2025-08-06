using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // <씬 전환>
    // 프로젝트에 포함된 다른 씬을 로딩하고 기존의 씬의 내용을 삭제함
    public static void ChageScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    // <씬 추가>
    // 프로젝트에 포함된 다른 씬을 로딩하고 기존의 씬의 내용을 유지함
    public static void AddScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }


}
