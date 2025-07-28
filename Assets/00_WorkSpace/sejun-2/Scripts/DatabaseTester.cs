using Firebase.Database;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;

public class DatabaseTester : MonoBehaviour
{
    [SerializeField] Button testButton;
    [SerializeField] Button upButton;
    [SerializeField] Button downButton;

    // Firebase������ ��ųʸ� ����.
    //[SerializeField] Dictionary<string, object> dictionary = new Dictionary<string, object>();

    [SerializeField] PlayerData data;

    private DatabaseReference userlevel;

    //Transform target;

    private void Awake()    // ��ũ��Ʈ�� Ȱ��ȭ�� �� ȣ��Ǵ� �Լ�
    {
        testButton.onClick.AddListener(Test); // ��ư Ŭ�� �� Test() �Լ��� ȣ���ϵ��� �����ʸ� �߰��մϴ�.
        upButton.onClick.AddListener(LevelUp); // Up ��ư Ŭ�� �� Up() �Լ��� ȣ���ϵ��� �����ʸ� �߰��մϴ�.
        downButton.onClick.AddListener(LevelDown); // Down ��ư Ŭ�� �� Down() �Լ��� ȣ���ϵ��� �����ʸ� �߰��մϴ�.
    }

    private void OnEnable() // Firebase ���� ���°� ����� ������ ȣ��Ǵ� �̺�Ʈ�� �����մϴ�.
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;   // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.
        DatabaseReference root = FirebaseManager.Database.RootReference;    // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
        userlevel = root.Child("UserData").Child(user.UserId).Child("level"); // ���� ������� ���� ������ ������ userlevel ������ �Ҵ��մϴ�.

        userlevel.ValueChanged += userLevel_ValueChanged; // �����ͺ��̽��� ���� ����� ������ ȣ��Ǵ� �̺�Ʈ�� �����մϴ�.
    }

    private void OnDisable() // Firebase ���� ���°� ����� ������ ȣ��Ǵ� �̺�Ʈ�� ���� �����մϴ�.
    {
        userlevel.ValueChanged -= userLevel_ValueChanged; // �����ͺ��̽��� ���� ����� ������ ȣ��Ǵ� �̺�Ʈ�� ���� �����մϴ�.
    }

    // ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    private void userLevel_ValueChanged(object sender, ValueChangedEventArgs args)
    {
        DataSnapshot snapshot = args.Snapshot;
        data.level = (int)(long)snapshot.Value; // �����ͺ��̽����� ������ ���� PlayerData�� level�� �����մϴ�.
        Debug.Log($"level�� {data.level} �� �����"); // ������ ����� ������ �α׸� ����մϴ�.
    }

    private void LevelUp()
    {
        userlevel.SetValueAsync(data.level + 1); // ���� ������ 1�� ���Ͽ� �����ͺ��̽��� �����մϴ�.
        Debug.Log("������ �������� ��û��");
    }

    private void LevelDown()
    {
        userlevel.SetValueAsync(data.level - 1); // ���� ������ 1�� ���̳ʽ� �Ͽ� �����ͺ��̽��� �����մϴ�.
        Debug.Log("������ �����ٿ��� ��û��");
    }



    //private void Test() // dictionary ����Ͽ� Firebase�� �����͸� �����ϴ� �Լ�
    //{
    //    // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
    //    DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

    //    dictionary["�̸�"] = "������";
    //    dictionary["����"] = 1;
    //    dictionary["��"] = 10;
    //    dictionary["�ӵ�"] = 20;
    //    dictionary["ũ��Ƽ��"] = 0.2;

    //    reference.SetValueAsync(dictionary);// Firebase �����ͺ��̽��� text ������ �����մϴ�.
    //}


    //private void Test() //PlayerData ��ü�� JSON ���ڿ��� ��� ����� ���� �ϴ� �Լ�
    //{
    //    FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.
    //    // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
    //    DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;
    //    DatabaseReference userInfo = root.Child("UserData").Child(user.UserId);

    //    string json = JsonUtility.ToJson(data); // PlayerData ��ü�� JSON ���ڿ��� ��ȯ�մϴ�.
    //    Debug.Log(json);
    //    userInfo.SetRawJsonValueAsync(json);    // Firebase �����ͺ��̽��� JSON ���ڿ��� ��� ����� �����մϴ�.

    //    //DatabaseReference levelRef = userInfo.Child("level");   // �ϳ��� �ٲٰ� ������
    //    //levelRef.SetValueAsync(3);  

    //}


    //private void Test()     // UpdateChildrenAsync �� Ư�� Ű�� ���� ������ ������Ʈ�ϴ� �Լ�
    //{
    //    FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.
    //    DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference; // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
    //    DatabaseReference userInfo = root.Child("UserData").Child(user.UserId); // UserData �Ʒ��� ���� ������� ID�� ���� ������ ����ϴ�.

    //    Dictionary<string, object> dictionry = new Dictionary<string, object>();
    //    dictionry["level"] = 10;
    //    dictionry["speed"] = 3.5;
    //    dictionry["skill/0"] = "���Ž�"; // ����Ʈ�� ù ��° ���

    //    userInfo.UpdateChildrenAsync(dictionry);    // Ư���� Ű�� ���� ������ ������Ʈ�մϴ�.
    //}


    //private void Test()     // "clear" Ű�� �����ϴ� �Լ� -> null�� �־ ���� ����
    //{
    //    //target = GameObject.Find("Player").transform; // "Player"��� �̸��� ���� ������Ʈ�� ã�Ƽ� �� Ʈ�������� target�� �Ҵ��մϴ�.

    //    FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.
    //    DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference; // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
    //    DatabaseReference userInfo = root.Child("UserData").Child(user.UserId); // UserData �Ʒ��� ���� ������� ID�� ���� ������ ����ϴ�.

    //    //userInfo.Child("clear").SetValueAsync(null);    // "clear" Ű�� ���� null�� �����Ͽ� �ش� Ű�� �����մϴ�.
    //    userInfo.Child("clear").RemoveValueAsync(); // "clear" Ű�� �����ϴ� �� �ٸ� ���
    //}


    //private void Test()     // RunTransaction -> Ʈ������� �����Ͽ� �����ͺ��̽��� ���� ������Ʈ�ϴ� �Լ�
    //{
    //    DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference; // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
    //    DatabaseReference leaderBoardRef = root.Child("LeaderBoard"); // "LeaderBoard"��� ���� ������ ����ϴ�.

    //    leaderBoardRef.RunTransaction(mutableData =>
    //    {
    //        List<object> leaders = mutableData.Value as List<object>; // mutableData�� List<object>�� ��ȯ�մϴ�.
    //        Debug.Log(leaders.Count);

    //        if (leaders == null) // leaders�� null�� ���
    //        {
    //            Debug.Log("�������尡 ��� �ֽ��ϴ�.");
    //            return TransactionResult.Abort(); // Ʈ������� �ߴ��մϴ�. (���� ���)
    //        }


    //        //return TransactionResult.Abort(); // Ʈ������� �ߴ��մϴ�. (���� ���)
    //        return TransactionResult.Success(mutableData);
    //    });
    //}


    //private void Test() // Firebase���� GetValueAsync, GetRawJsonValue�� �����͸� �������� �Լ�
    //{
    //    FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.
    //    DatabaseReference root = FirebaseManager.Database.RootReference;    // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
    //    DatabaseReference userInfo = root.Child("UserData").Child(user.UserId); // UserData �Ʒ��� ���� ������� ID�� ���� ������ ����ϴ�.

    //    userInfo.GetValueAsync().ContinueWithOnMainThread(task => // GetValueAsync �޼��带 ����Ͽ� �����͸� �����ɴϴ�.
    //    {
    //        if (task.IsCanceled) // �۾��� ��ҵ� ���
    //        {
    //            Debug.LogError("������ �������� �۾��� ��ҵǾ����ϴ�.");
    //            return;
    //        }
    //        if (task.IsFaulted) // �۾��� ������ ���
    //        {
    //            Debug.LogError("�����͸� �������� �� �����߽��ϴ�: " + task.Exception);
    //        }

    //        DataSnapshot snapshot = task.Result; // ����� DataSnapshot���� �����ɴϴ�.
    //        Debug.Log($"���ܼ� child count : {snapshot.ChildrenCount}");

    //        //bool clear = (bool)snapshot.Child("clear").Value;   // "clear" Ű�� ���� �����ɴϴ�. ����ȯ �ʿ�.
    //        //Debug.Log($"clear : {clear}"); // "clear" Ű�� ���� ����մϴ�.

    //        //long level = (long)snapshot.Child("level").Value; // "level" Ű�� ���� �����ɴϴ�. ����ȯ �ʿ�.
    //        //Debug.Log($"level : {level}"); // "level" Ű�� ���� ����մϴ�.

    //        //string name = (string)snapshot.Child("name").Value; // "name" Ű�� ���� �����ɴϴ�. ����ȯ �ʿ�.
    //        //Debug.Log($"name : {name}"); // "name" Ű�� ���� ����մϴ�.

    //        //float speed = (float)(double)snapshot.Child("speed").Value; // "speed" Ű�� ���� �����ɴϴ�. ����ȯ �ʿ�.
    //        //Debug.Log($"speed : {speed}"); // "speed" Ű�� ���� ����մϴ�.

    //        //List<object> skill = (List<object>)snapshot.Child("skill").Value; // "skill" Ű�� ���� �����ɴϴ�. ����ȯ �ʿ�.
    //        //for (int i = 0; i < skill.Count; i++) // skill ����Ʈ�� �� ��Ҹ� ����մϴ�.
    //        //{
    //        //    Debug.Log($"skill : {skill[i]}");
    //        //}


    //        string json = snapshot.GetRawJsonValue();   // GetRawJsonValue �޼��带 ����Ͽ� JSON ���ڿ��� �����ɴϴ�.
    //        Debug.Log($"JSON ������: {json}");

    //        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json); // JSON ���ڿ��� PlayerData ��ü�� ��ȯ�մϴ�.
    //        Debug.Log($"Clear: {playerData.clear}");
    //        Debug.Log($"name: {playerData.name}");
    //        Debug.Log($"speed: {playerData.speed}");
    //        Debug.Log($"level: {playerData.level}");
    //        if (playerData.skill != null)
    //        {
    //            for (int i = 0; i < playerData.skill.Count; i++)
    //            {
    //                Debug.Log($"Skill[{i}]: {playerData.skill[i]}");
    //            }
    //        }
    //    });
    //}


    private void Test() // RunTransaction -> Ʈ������� �����Ͽ� �����ͺ��̽��� ���� ������Ʈ�ϴ� �Լ�
    {
         FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.

        DatabaseReference root = FirebaseManager.Database.RootReference; // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
        DatabaseReference userCount = root.Child("GameData").Child("TotalUser"); // "GameData" �Ʒ��� "TotalUser"��� ���� ������ ����ϴ�.

        userCount.RunTransaction(mutableData =>
        {
            if (mutableData.Value == null) // mutableData�� ���� null�� ��� ����ó�� �� �ؾ���.!
            {
                Debug.Log("TotalUser�� ��� �ֽ��ϴ�. �ʱⰪ�� �����մϴ�.");
                mutableData.Value = 1; // �ʱⰪ�� 1���� �����մϴ�. �� �Ѹ� �����״�.
                return TransactionResult.Success(mutableData); // Ʈ������� �������� ��ȯ�մϴ�.
            }

            long totaluserCount = (long)mutableData.Value;
            mutableData.Value = totaluserCount + 1; // ���� ����� ���� ������ŵ�ϴ�.

            return TransactionResult.Success(mutableData); // Ʈ������� �������� ��ȯ�մϴ�.
        });


    }



}



[Serializable]  // Serializable ��Ʈ����Ʈ�� ����Ͽ� �� Ŭ������ JSON���� ����ȭ�� �� �ֵ��� �մϴ�.
public class PlayerData
{
    public string name;
    public int level;
    //public int strength;
    public int speed;
    //public float critical;
    public bool clear;
    public List<string> skill;
}


[Serializable]  // Serializable ��Ʈ����Ʈ�� ����Ͽ� �� Ŭ������ JSON���� ����ȭ�� �� �ֵ��� �մϴ�.
public class LeaderBoardData
{
    public List<Ranking> ranker; // ��ŷ ����Ʈ�� �����ϴ� ����Ʈ

    [Serializable]  // Serializable ��Ʈ����Ʈ�� ����Ͽ� �� Ŭ������ JSON���� ����ȭ�� �� �ֵ��� �մϴ�.
    public class Ranking
    {
        public string name;
        public int winsCount;
    }
}
