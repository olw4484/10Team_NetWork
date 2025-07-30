using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    private static FirebaseManager instance;
    public static FirebaseManager Instance {  get { return instance; } }

    private static FirebaseApp app; // FirebaseApp �ν��Ͻ�
    public static FirebaseApp App { get { return app; } }   // FirebaseApp �ν��Ͻ��� �����ϱ� ���� ������Ƽ

    private static FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return auth; } }

    private static FirebaseDatabase database;
    public static FirebaseDatabase Database { get { return database; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            Firebase.DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("���̾� ���̽� ������ ��� �����Ǿ� ����� �� �ִ� ��Ȳ");
                app = FirebaseApp.DefaultInstance;  // FirebaseApp �ν��Ͻ� �ʱ�ȭ
                auth = FirebaseAuth.DefaultInstance;    // FirebaseAuth �ν��Ͻ� �ʱ�ȭ
                database = FirebaseDatabase.DefaultInstance;    // FirebaseDatabase �ν��Ͻ� �ʱ�ȭ
            }
            else
            {
                Debug.LogError($"���̾� ���̽� ������ �������� �ʾ� �����߽��ϴ�. ����: {dependencyStatus}");
                app = null;
                auth = null;
                database = null;
            }
        });

    }



}
