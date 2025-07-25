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

    // Firebase에서는 딕셔너리 지원.
    //[SerializeField] Dictionary<string, object> dictionary = new Dictionary<string, object>();

    //[SerializeField] PlayerData data;

    //Transform target;

    private void Awake()    // 스크립트가 활성화될 때 호출되는 함수
    {
        testButton.onClick.AddListener(Test); // 버튼 클릭 시 Test() 함수를 호출하도록 리스너를 추가합니다.
    }

    //private void Test() //딕셔너리를 사용하여 Firebase에 데이터를 저장하는 함수
    //{
    //    // Firebase 데이터베이스의 루트 참조를 가져옵니다.
    //    DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

    //    dictionary["이름"] = "김전사";
    //    dictionary["레벨"] = 1;
    //    dictionary["힘"] = 10;
    //    dictionary["속도"] = 20;
    //    dictionary["크리티컬"] = 0.2;

    //    reference.SetValueAsync(dictionary);// Firebase 데이터베이스에 text 변수를 저장합니다.
    //}

    //private void Test() //PlayerData 객체를 JSON 문자열을 모두 덮어씌워 저장 하는 함수
    //{
    //    FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // 현재 로그인된 Firebase 사용자를 가져옵니다.
    //    // Firebase 데이터베이스의 루트 참조를 가져옵니다.
    //    DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;
    //    DatabaseReference userInfo = root.Child("UserData").Child(user.UserId);

    //    string json = JsonUtility.ToJson(data); // PlayerData 객체를 JSON 문자열로 변환합니다.
    //    Debug.Log(json);
    //    userInfo.SetRawJsonValueAsync(json);    // Firebase 데이터베이스에 JSON 문자열을 모두 덮어씌워 저장합니다.

    //    //DatabaseReference levelRef = userInfo.Child("level");   // 하나만 바꾸고 싶을때
    //    //levelRef.SetValueAsync(3);  

    //}


    //private void Test()     // UpdateChildrenAsync 로 특정 키에 대한 값만을 업데이트하는 함수
    //{
    //    FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // 현재 로그인된 Firebase 사용자를 가져옵니다.
    //    DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference; // Firebase 데이터베이스의 루트 참조를 가져옵니다.
    //    DatabaseReference userInfo = root.Child("UserData").Child(user.UserId); // UserData 아래에 현재 사용자의 ID로 하위 참조를 만듭니다.

    //    Dictionary<string, object> dictionry = new Dictionary<string, object>();
    //    dictionry["level"] = 10;
    //    dictionry["speed"] = 3.5;
    //    dictionry["skill/0"] = "스매쉬"; // 리스트의 첫 번째 요소

    //    userInfo.UpdateChildrenAsync(dictionry);    // 특정된 키에 대한 값만을 업데이트합니다.
    //}


    //private void Test()     // "clear" 키를 삭제하는 함수 -> null을 넣어서 삭제 가능
    //{
    //    //target = GameObject.Find("Player").transform; // "Player"라는 이름의 게임 오브젝트를 찾아서 그 트랜스폼을 target에 할당합니다.

    //    FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // 현재 로그인된 Firebase 사용자를 가져옵니다.
    //    DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference; // Firebase 데이터베이스의 루트 참조를 가져옵니다.
    //    DatabaseReference userInfo = root.Child("UserData").Child(user.UserId); // UserData 아래에 현재 사용자의 ID로 하위 참조를 만듭니다.

    //    //userInfo.Child("clear").SetValueAsync(null);    // "clear" 키의 값을 null로 설정하여 해당 키를 삭제합니다.
    //    userInfo.Child("clear").RemoveValueAsync(); // "clear" 키를 삭제하는 또 다른 방법
    //}


    private void Test()     // RunTransaction 함수 사용 예제
    {
        DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference; // Firebase 데이터베이스의 루트 참조를 가져옵니다.
        DatabaseReference leaderBoardRef = root.Child("LeaderBoard"); // "LeaderBoard"라는 하위 참조를 만듭니다.

        leaderBoardRef.RunTransaction(data =>
        {


            return TransactionResult.Success(data); 
        });
    }

}

[Serializable]  // Serializable 어트리뷰트를 사용하여 이 클래스가 JSON으로 직렬화될 수 있도록 합니다.
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
