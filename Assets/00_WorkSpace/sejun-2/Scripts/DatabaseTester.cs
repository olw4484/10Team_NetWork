using Firebase.Database;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseTester : MonoBehaviour
{
    [SerializeField] Button testButton;

    // Firebase������ ��ųʸ� ����.
    //[SerializeField] Dictionary<string, object> dictionary = new Dictionary<string, object>();

    //[SerializeField] PlayerData data;

    //Transform target;

    private void Awake()    // ��ũ��Ʈ�� Ȱ��ȭ�� �� ȣ��Ǵ� �Լ�
    {
        testButton.onClick.AddListener(Test); // ��ư Ŭ�� �� Test() �Լ��� ȣ���ϵ��� �����ʸ� �߰��մϴ�.
    }

    //private void Test() //��ųʸ��� ����Ͽ� Firebase�� �����͸� �����ϴ� �Լ�
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


    private void Test()     // RunTransaction �Լ� ��� ����
    {
        DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference; // Firebase �����ͺ��̽��� ��Ʈ ������ �����ɴϴ�.
        DatabaseReference leaderBoardRef = root.Child("LeaderBoard"); // "LeaderBoard"��� ���� ������ ����ϴ�.

        leaderBoardRef.RunTransaction(data =>
        {


            return TransactionResult.Success(data); 
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

    //public PlayerData(string name, int level, int strength, int speed, float critical)
    //{
    //    this.name = name;
    //    this.level = level;
    //    this.strength = strength;
    //    this.speed = speed;
    //    this.critical = critical;
    //}
}
