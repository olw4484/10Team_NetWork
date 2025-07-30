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

    private static FirebaseApp app; // FirebaseApp 인스턴스
    public static FirebaseApp App { get { return app; } }   // FirebaseApp 인스턴스에 접근하기 위한 프로퍼티

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
                Debug.Log("파이어 베이스 설정이 모두 충족되어 사용할 수 있는 상황");
                app = FirebaseApp.DefaultInstance;  // FirebaseApp 인스턴스 초기화
                auth = FirebaseAuth.DefaultInstance;    // FirebaseAuth 인스턴스 초기화
                database = FirebaseDatabase.DefaultInstance;    // FirebaseDatabase 인스턴스 초기화
            }
            else
            {
                Debug.LogError($"파이어 베이스 설정이 충족되지 않아 실패했습니다. 이유: {dependencyStatus}");
                app = null;
                auth = null;
                database = null;
            }
        });

    }



}
