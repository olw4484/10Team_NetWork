using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{


    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            Firebase.DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("���̾� ���̽� ������ ��� �����Ǿ� ����� �� �ִ� ��Ȳ");
            }
            else
            {
                Debug.LogError($"���̾� ���̽� ������ �������� �ʾ� �����߽��ϴ�. ����: {dependencyStatus}");
            }
        });
    }



}
