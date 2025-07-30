using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseTester : MonoBehaviour
{
    [SerializeField] Button testButton;
    [SerializeField] Button upButton;
    [SerializeField] Button downButton;

    // Firebase������ ��ųʸ� ����.
    //[SerializeField] Dictionary<string, object> dictionary = new Dictionary<string, object>();

    [SerializeField] PlayerData data;   // PlayerData ��ü�� ����ȭ�Ͽ� Firebase�� ������ ������ �����Դϴ�.

    private DatabaseReference userlevel;
    private DatabaseReference gameCount;    // ���� Ƚ���� �����ϴ� �����ͺ��̽� ����
    private DatabaseReference winsCount;    // �¸� Ƚ���� �����ϴ� �����ͺ��̽� ����

    //Transform target;

    private void Awake()    // ��ũ��Ʈ�� Ȱ��ȭ�� �� ȣ��Ǵ� �Լ�
    {
        GetUserinfo(); // Firebase���� ����� ������ �������� �Լ��� ȣ���մϴ�.

        testButton.onClick.AddListener(SetJsonData);
    }

    private void OnEnable() // Firebase ���� ���°� ����� ������ ȣ��Ǵ� �̺�Ʈ�� �����մϴ�.
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;   // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.
        DatabaseReference root = FirebaseManager.Database.RootReference;    // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
        gameCount = root.Child("UserData").Child(user.UserId).Child("gameCount");   // ���� Ƚ�� ������ ������ gameCount ������ �Ҵ��մϴ�.
        winsCount = root.Child("UserData").Child(user.UserId).Child("winsCount");

        gameCount.ValueChanged += gameCount_ValueChanged; // ���� Ƚ�� �����ͺ��̽��� ���� ����� ������ ȣ��Ǵ� �̺�Ʈ�� �����մϴ�.
        winsCount.ValueChanged += winsCount_ValueChanged; // �¸� Ƚ�� �����ͺ��̽��� ���� ����� ������ ȣ��Ǵ� �̺�Ʈ�� �����մϴ�.
    }

    private void OnDisable() // Firebase ���� ���°� ����� ������ ȣ��Ǵ� �̺�Ʈ�� ���� �����մϴ�.
    {
        gameCount.ValueChanged -= gameCount_ValueChanged; // ���� Ƚ�� �����ͺ��̽��� ���� ����� ������ ȣ��Ǵ� �̺�Ʈ�� ���� �����մϴ�.
        winsCount.ValueChanged -= winsCount_ValueChanged; // �¸� Ƚ�� �����ͺ��̽��� ���� ����� ������ ȣ��Ǵ� �̺�Ʈ�� ���� �����մϴ�.
    }

    // �¸� Ƚ�� �����ͺ��̽��� ���� ����� ������ ȣ��Ǵ� �̺�Ʈ �ڵ鷯�Դϴ�.
    private void winsCount_ValueChanged(object sender, ValueChangedEventArgs args)
    {
        DataSnapshot snapshot = args.Snapshot;
        data.winsCount = (int)(long)snapshot.Value; // ���� Ƚ���� ������ PlayerData�� gameCount�� �����մϴ�.
        Debug.Log($"winsCount�� {data.gameCount} �� �����"); // ���� Ƚ���� ����� ������ �α׸� ����մϴ�.
    }

    // ���� Ƚ�� �����ͺ��̽��� ���� ����� ������ ȣ��Ǵ� �̺�Ʈ �ڵ鷯�Դϴ�.
    private void gameCount_ValueChanged(object sender, ValueChangedEventArgs args)
    {
        DataSnapshot snapshot = args.Snapshot;
        data.gameCount = (int)(long)snapshot.Value; // ���� Ƚ���� ������ PlayerData�� gameCount�� �����մϴ�.
        Debug.Log($"gameCount�� {data.gameCount} �� �����"); // ���� Ƚ���� ����� ������ �α׸� ����մϴ�.
    }

    // �����ͺ��̽��� ���� ���� ����� ������ ȣ��Ǵ� �̺�Ʈ �ڵ鷯�Դϴ�.
    private void userLevel_ValueChanged(object sender, ValueChangedEventArgs args)
    {
        DataSnapshot snapshot = args.Snapshot;
        data.level = (int)(long)snapshot.Value; // �����ͺ��̽����� ������ ���� PlayerData�� level�� �����մϴ�.
        Debug.Log($"level�� {data.level} �� �����"); // ������ ����� ������ �α׸� ����մϴ�.
    }

    private void LevelUp()  // ������ �������� ��û��
    {
        userlevel.SetValueAsync(data.level + 1); // ���� ������ 1�� ���Ͽ� �����ͺ��̽��� �����մϴ�.
    }

    private void LevelDown()    // ������ �����ٿ��� ��û��
    {
        userlevel.SetValueAsync(data.level - 1); // ���� ������ 1�� ���̳ʽ� �Ͽ� �����ͺ��̽��� �����մϴ�.
    }

    private void GameCountUp()
    {
        gameCount.SetValueAsync(data.gameCount + 1); // ���� Ƚ���� 1 ������ŵ�ϴ�.
        //gameCount.SetValueAsync(data.gameCount - 1); // ���� Ƚ���� 1 ���ҽ�ŵ�ϴ�.
    }

    private void WinsCountUp()
    {
        winsCount.SetValueAsync(data.winsCount + 1); // �¸� Ƚ���� 1 ������ŵ�ϴ�.
        //winsCount.SetValueAsync(data.winsCount - 1); // �¸� Ƚ���� 1 ���ҽ�ŵ�ϴ�.
    }


    //private void SetData() // dictionary ����Ͽ� Firebase�� �����͸� �����ϴ� �Լ�
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


    private void SetJsonData() //PlayerData ��ü�� JSON ���ڿ��� ��� ����� ���� �ϴ� �Լ�
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.
        // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
        DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;
        DatabaseReference userInfo = root.Child("UserData").Child(user.UserId);

        string json = JsonUtility.ToJson(data); // PlayerData ��ü�� JSON ���ڿ��� ��ȯ�մϴ�.
        Debug.Log(json);
        userInfo.SetRawJsonValueAsync(json);    // Firebase �����ͺ��̽��� JSON ���ڿ��� ��� ����� �����մϴ�.

        //DatabaseReference levelRef = userInfo.Child("level");   // �ϳ��� �ٲٰ� ������
        //levelRef.SetValueAsync(3);  
    }


    //private void DataUpdate()     // UpdateChildrenAsync �� Ư�� Ű�� ���� ������ ������Ʈ�ϴ� �Լ�
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


    private void Delete(string value)     // "value" Ű�� �����ϴ� �Լ� -> null�� �־ ���� ����
    {
        //target = GameObject.Find("Player").transform; // "Player"��� �̸��� ���� ������Ʈ�� ã�Ƽ� �� Ʈ�������� target�� �Ҵ��մϴ�.

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.
        DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference; // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
        DatabaseReference userInfo = root.Child("UserData").Child(user.UserId); // UserData �Ʒ��� ���� ������� ID�� ���� ������ ����ϴ�.

        //userInfo.Child("value").SetValueAsync(null);    // "clear" Ű�� ���� null�� �����Ͽ� �ش� Ű�� �����մϴ�.
        userInfo.Child("value").RemoveValueAsync(); // "value" Ű�� �����ϴ� �� �ٸ� ���
    }


    private void GetUserinfo() // Firebase���� GetValueAsync, GetRawJsonValue�� �����͸� �������� �Լ�
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.
        DatabaseReference root = FirebaseManager.Database.RootReference;    // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
        DatabaseReference userInfo = root.Child("UserData").Child(user.UserId); // UserData �Ʒ��� ���� ������� ID�� ���� ������ ����ϴ�.

        userInfo.GetValueAsync().ContinueWithOnMainThread(task => // GetValueAsync �޼��带 ����Ͽ� �����͸� �����ɴϴ�.
        {
            if (task.IsCanceled) // �۾��� ��ҵ� ���
            {
                Debug.LogError("������ �������� �۾��� ��ҵǾ����ϴ�.");
                return;
            }
            if (task.IsFaulted) // �۾��� ������ ���
            {
                Debug.LogError("�����͸� �������� �� �����߽��ϴ�: " + task.Exception);
            }

            DataSnapshot snapshot = task.Result; // ����� DataSnapshot���� �����ɴϴ�.

            //Debug.Log($"���ܼ� child count : {snapshot.ChildrenCount}");
            //bool clear = (bool)snapshot.Child("clear").Value;   // "clear" Ű�� ���� �����ɴϴ�. ����ȯ �ʿ�.
            //Debug.Log($"clear : {clear}"); // "clear" Ű�� ���� ����մϴ�.
            //long level = (long)snapshot.Child("level").Value; // "level" Ű�� ���� �����ɴϴ�. ����ȯ �ʿ�.
            //Debug.Log($"level : {level}"); // "level" Ű�� ���� ����մϴ�.
            //string name = (string)snapshot.Child("name").Value; // "name" Ű�� ���� �����ɴϴ�. ����ȯ �ʿ�.
            //Debug.Log($"name : {name}"); // "name" Ű�� ���� ����մϴ�.
            //float speed = (float)(double)snapshot.Child("speed").Value; // "speed" Ű�� ���� �����ɴϴ�. ����ȯ �ʿ�.
            //Debug.Log($"speed : {speed}"); // "speed" Ű�� ���� ����մϴ�.
            //List<object> skill = (List<object>)snapshot.Child("skill").Value; // "skill" Ű�� ���� �����ɴϴ�. ����ȯ �ʿ�.
            //for (int i = 0; i < skill.Count; i++) // skill ����Ʈ�� �� ��Ҹ� ����մϴ�.
            //{
            //    Debug.Log($"skill : {skill[i]}");
            //}

            string json = snapshot.GetRawJsonValue();   // GetRawJsonValue �޼��带 ����Ͽ� JSON ���ڿ��� �����ɴϴ�.
            //Debug.Log($"JSON ������: {json}");
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json); // JSON ���ڿ��� PlayerData ��ü�� ��ȯ�մϴ�.
            data = playerData; // ������ �����͸� data ������ �����մϴ�.

            //Debug.Log($"name: {playerData.name}");
            //Debug.Log($"level: {playerData.level}");
            //if (playerData.skill != null)
            //{
            //    for (int i = 0; i < playerData.skill.Count; i++)
            //    {
            //        Debug.Log($"Skill[{i}]: {playerData.skill[i]}");
            //    }
            //}
        });
    }


    //private void UpdateTransaction() // RunTransaction -> Ʈ������� �����Ͽ� �����ͺ��̽��� ���� ������Ʈ�ϴ� �Լ�
    //{
    //    FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // ���� �α��ε� Firebase ����ڸ� �����ɴϴ�.

    //    DatabaseReference root = FirebaseManager.Database.RootReference; // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
    //    DatabaseReference userCount = root.Child("GameData").Child("TotalUser"); // "GameData" �Ʒ��� "TotalUser"��� ���� ������ ����ϴ�.

    //    userCount.RunTransaction(mutableData =>
    //    {
    //        if (mutableData.Value == null) // mutableData�� ���� null�� ��� ����ó�� �� �ؾ���.!
    //        {
    //            Debug.Log("TotalUser�� ��� �ֽ��ϴ�. �ʱⰪ�� �����մϴ�.");
    //            mutableData.Value = 1; // �ʱⰪ�� 1���� �����մϴ�. �� �Ѹ� �����״�.
    //            return TransactionResult.Success(mutableData); // Ʈ������� �������� ��ȯ�մϴ�.
    //        }

    //        long totaluserCount = (long)mutableData.Value;
    //        mutableData.Value = totaluserCount + 1; // ���� ����� ���� ������ŵ�ϴ�.

    //        return TransactionResult.Success(mutableData); // Ʈ������� �������� ��ȯ�մϴ�.
    //    });
    //}


    private void CheckLeaderBoard()     // Firebase���� OrderByChild, GetValueAsync�� ����Ͽ� ��ŷ �����͸� �������� �Լ�
    {
        DatabaseReference root = FirebaseManager.Database.RootReference; // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
        DatabaseReference UserRef = root.Child("UserData"); // "UserData"��� ���� ������ wordRef�� �Ҵ��մϴ�.

        // wordRef�� OrderByChild("winsCount")�� ����Ͽ� "winsCount" Ű�� �������� ���ĵ� �����͸� �����ɴϴ�.
        // LimitToFirst(10)�� �߰��Ͽ� ���� 10���� �����͸� ������ �� �ִ�.
        // .StartAt("B").EndAt("E") B���� E������ ������ ������ �� �ִ�.
        UserRef.OrderByChild("winsCount").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled) // �۾��� ��ҵ� ���
            {
                Debug.LogError("������ �������� �۾��� ��ҵǾ����ϴ�.");
                return;
            }
            if (task.IsFaulted) // �۾��� ������ ���
            {
                Debug.LogError("�����͸� �������� �� �����߽��ϴ�: " + task.Exception);
                return;
            }
            Debug.Log("������ ���̽� �б� ����");
            DataSnapshot snapshot = task.Result; // ����� DataSnapshot���� �����ɴϴ�.

            // DataSnapshot�� �ڽĵ��� �������� �����ɴϴ�.
            var children = snapshot.Children.Reverse().ToList(); // .ToList() �� ��ȯ�ؾ� for������ ����� �� �ֽ��ϴ�.
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                Debug.Log($"{i + 1}�� {child.Child("name").Value} :  {child.Child("winsCount").Value} ��");
            }
        });
    }
}


[Serializable]  // Serializable ��Ʈ����Ʈ�� ����Ͽ� �� Ŭ������ JSON���� ����ȭ�� �� �ֵ��� �մϴ�.
public class PlayerData
{
    public string name;
    public int level;
    public int gameCount;
    public int winsCount;
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
