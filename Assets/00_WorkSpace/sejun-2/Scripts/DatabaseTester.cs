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

    // Firebase에서는 딕셔너리 지원.
    //[SerializeField] Dictionary<string, object> dictionary = new Dictionary<string, object>();

    [SerializeField] PlayerData data;   // PlayerData 객체를 직렬화하여 Firebase에 저장할 데이터 구조입니다.

    private DatabaseReference userlevel;
    private DatabaseReference gameCount;    // 게임 횟수를 저장하는 데이터베이스 참조
    private DatabaseReference winsCount;    // 승리 횟수를 저장하는 데이터베이스 참조

    //Transform target;

    private void Awake()    // 스크립트가 활성화될 때 호출되는 함수
    {
        GetUserinfo(); // Firebase에서 사용자 정보를 가져오는 함수를 호출합니다.

        testButton.onClick.AddListener(CheckLeaderBoard); 
        upButton.onClick.AddListener(LevelUp); 
        downButton.onClick.AddListener(LevelDown); 
    }

    private void OnEnable() // Firebase 인증 상태가 변경될 때마다 호출되는 이벤트를 구독합니다.
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;   // 현재 로그인된 Firebase 사용자를 가져옵니다.
        DatabaseReference root = FirebaseManager.Database.RootReference;    // Firebase 데이터베이스의 루트 참조를 가져옵니다.
        userlevel = root.Child("UserData").Child(user.UserId).Child("level"); // 현재 사용자의 레벨 정보를 가져와 userlevel 변수에 할당합니다.
        gameCount = root.Child("UserData").Child(user.UserId).Child("gameCount");
        winsCount = root.Child("UserData").Child(user.UserId).Child("winsCount");

        userlevel.ValueChanged += userLevel_ValueChanged; // 데이터베이스의 값이 변경될 때마다 호출되는 이벤트를 구독합니다.
        gameCount.ValueChanged += gameCount_ValueChanged; // 게임 횟수 데이터베이스의 값이 변경될 때마다 호출되는 이벤트를 구독합니다.
        winsCount.ValueChanged += winsCount_ValueChanged; // 승리 횟수 데이터베이스의 값이 변경될 때마다 호출되는 이벤트를 구독합니다.
    }

    private void OnDisable() // Firebase 인증 상태가 변경될 때마다 호출되는 이벤트를 구독 해제합니다.
    {
        userlevel.ValueChanged -= userLevel_ValueChanged; // 데이터베이스의 값이 변경될 때마다 호출되는 이벤트를 구독 해제합니다.
        gameCount.ValueChanged -= gameCount_ValueChanged; // 게임 횟수 데이터베이스의 값이 변경될 때마다 호출되는 이벤트를 구독 해제합니다.
        winsCount.ValueChanged -= winsCount_ValueChanged; // 승리 횟수 데이터베이스의 값이 변경될 때마다 호출되는 이벤트를 구독 해제합니다.
    }

    // 승리 횟수 데이터베이스의 값이 변경될 때마다 호출되는 이벤트 핸들러입니다.
    private void winsCount_ValueChanged(object sender, ValueChangedEventArgs args)
    {
        DataSnapshot snapshot = args.Snapshot;
        data.winsCount = (int)(long)snapshot.Value; // 게임 횟수를 가져와 PlayerData의 gameCount에 저장합니다.
        Debug.Log($"winsCount가 {data.gameCount} 로 변경됨"); // 게임 횟수가 변경될 때마다 로그를 출력합니다.
    }

    // 게임 횟수 데이터베이스의 값이 변경될 때마다 호출되는 이벤트 핸들러입니다.
    private void gameCount_ValueChanged(object sender, ValueChangedEventArgs args)
    {
        DataSnapshot snapshot = args.Snapshot;
        data.gameCount = (int)(long)snapshot.Value; // 게임 횟수를 가져와 PlayerData의 gameCount에 저장합니다.
        Debug.Log($"gameCount가 {data.gameCount} 로 변경됨"); // 게임 횟수가 변경될 때마다 로그를 출력합니다.
    }

    // 데이터베이스의 레벨 값이 변경될 때마다 호출되는 이벤트 핸들러입니다.
    private void userLevel_ValueChanged(object sender, ValueChangedEventArgs args)
    {
        DataSnapshot snapshot = args.Snapshot;
        data.level = (int)(long)snapshot.Value; // 데이터베이스에서 가져온 값을 PlayerData의 level에 저장합니다.
        Debug.Log($"level이 {data.level} 로 변경됨"); // 레벨이 변경될 때마다 로그를 출력합니다.
    }

    private void LevelUp()  // 레벨업 버튼 클릭 시 호출되는 함수
    {
        userlevel.SetValueAsync(data.level + 1); // 현재 레벨에 1을 더하여 데이터베이스에 저장합니다.
        gameCount.SetValueAsync(data.gameCount + 1); // 게임 횟수를 1 증가시킵니다.
        winsCount.SetValueAsync(data.winsCount + 1); // 승리 횟수를 1 증가시킵니다.
        Debug.Log("서버에 레벨업을 신청함");
    }

    private void LevelDown()    // 레벨다운 버튼 클릭 시 호출되는 함수
    {
        userlevel.SetValueAsync(data.level - 1); // 현재 레벨에 1을 마이너스 하여 데이터베이스에 저장합니다.
        gameCount.SetValueAsync(data.gameCount - 1); // 게임 횟수를 1 감소시킵니다.
        winsCount.SetValueAsync(data.winsCount - 1); // 승리 횟수를 1 감소시킵니다.
        Debug.Log("서버에 레벨다운을 신청함");
    }



    //private void SetData() // dictionary 사용하여 Firebase에 데이터를 저장하는 함수
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


    //private void SetJsonData() //PlayerData 객체를 JSON 문자열을 모두 덮어씌워 저장 하는 함수
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


    //private void DataUpdate()     // UpdateChildrenAsync 로 특정 키에 대한 값만을 업데이트하는 함수
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


    //private void Delete()     // "clear" 키를 삭제하는 함수 -> null을 넣어서 삭제 가능
    //{
    //    //target = GameObject.Find("Player").transform; // "Player"라는 이름의 게임 오브젝트를 찾아서 그 트랜스폼을 target에 할당합니다.

    //    FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // 현재 로그인된 Firebase 사용자를 가져옵니다.
    //    DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference; // Firebase 데이터베이스의 루트 참조를 가져옵니다.
    //    DatabaseReference userInfo = root.Child("UserData").Child(user.UserId); // UserData 아래에 현재 사용자의 ID로 하위 참조를 만듭니다.

    //    //userInfo.Child("clear").SetValueAsync(null);    // "clear" 키의 값을 null로 설정하여 해당 키를 삭제합니다.
    //    userInfo.Child("clear").RemoveValueAsync(); // "clear" 키를 삭제하는 또 다른 방법
    //}


    //private void DataTransaction()     // RunTransaction -> 트랜잭션을 실행하여 데이터베이스의 값을 업데이트하는 함수
    //{
    //    DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference; // Firebase 데이터베이스의 루트 참조를 가져옵니다.
    //    DatabaseReference leaderBoardRef = root.Child("LeaderBoard"); // "LeaderBoard"라는 하위 참조를 만듭니다.

    //    leaderBoardRef.RunTransaction(mutableData =>
    //    {
    //        List<object> leaders = mutableData.Value as List<object>; // mutableData를 List<object>로 변환합니다.
    //        Debug.Log(leaders.Count);

    //        if (leaders == null) // leaders가 null인 경우
    //        {
    //            Debug.Log("리더보드가 비어 있습니다.");
    //            return TransactionResult.Abort(); // 트랜잭션을 중단합니다. (변경 취소)
    //        }


    //        //return TransactionResult.Abort(); // 트랜잭션을 중단합니다. (변경 취소)
    //        return TransactionResult.Success(mutableData);
    //    });
    //}


    private void GetUserinfo() // Firebase에서 GetValueAsync, GetRawJsonValue로 데이터를 가져오는 함수
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // 현재 로그인된 Firebase 사용자를 가져옵니다.
        DatabaseReference root = FirebaseManager.Database.RootReference;    // Firebase 데이터베이스의 루트 참조를 가져옵니다.
        DatabaseReference userInfo = root.Child("UserData").Child(user.UserId); // UserData 아래에 현재 사용자의 ID로 하위 참조를 만듭니다.

        userInfo.GetValueAsync().ContinueWithOnMainThread(task => // GetValueAsync 메서드를 사용하여 데이터를 가져옵니다.
        {
            if (task.IsCanceled) // 작업이 취소된 경우
            {
                Debug.LogError("데이터 가져오기 작업이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted) // 작업이 실패한 경우
            {
                Debug.LogError("데이터를 가져오는 데 실패했습니다: " + task.Exception);
            }

            DataSnapshot snapshot = task.Result; // 결과를 DataSnapshot으로 가져옵니다.

            //Debug.Log($"스넵샷 child count : {snapshot.ChildrenCount}");
            //bool clear = (bool)snapshot.Child("clear").Value;   // "clear" 키의 값을 가져옵니다. 형변환 필요.
            //Debug.Log($"clear : {clear}"); // "clear" 키의 값을 출력합니다.

            //long level = (long)snapshot.Child("level").Value; // "level" 키의 값을 가져옵니다. 형변환 필요.
            //Debug.Log($"level : {level}"); // "level" 키의 값을 출력합니다.

            //string name = (string)snapshot.Child("name").Value; // "name" 키의 값을 가져옵니다. 형변환 필요.
            //Debug.Log($"name : {name}"); // "name" 키의 값을 출력합니다.

            //float speed = (float)(double)snapshot.Child("speed").Value; // "speed" 키의 값을 가져옵니다. 형변환 필요.
            //Debug.Log($"speed : {speed}"); // "speed" 키의 값을 출력합니다.

            //List<object> skill = (List<object>)snapshot.Child("skill").Value; // "skill" 키의 값을 가져옵니다. 형변환 필요.
            //for (int i = 0; i < skill.Count; i++) // skill 리스트의 각 요소를 출력합니다.
            //{
            //    Debug.Log($"skill : {skill[i]}");
            //}

            string json = snapshot.GetRawJsonValue();   // GetRawJsonValue 메서드를 사용하여 JSON 문자열을 가져옵니다.
            //Debug.Log($"JSON 데이터: {json}");
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json); // JSON 문자열을 PlayerData 객체로 변환합니다.
            data = playerData; // 가져온 데이터를 data 변수에 저장합니다.
            Debug.Log($"Clear: {playerData.clear}");
            Debug.Log($"name: {playerData.name}");
            Debug.Log($"speed: {playerData.speed}");
            Debug.Log($"level: {playerData.level}");
            if (playerData.skill != null)
            {
                for (int i = 0; i < playerData.skill.Count; i++)
                {
                    Debug.Log($"Skill[{i}]: {playerData.skill[i]}");
                }
            }
        });
    }


    private void UpdateTransaction() // RunTransaction -> 트랜잭션을 실행하여 데이터베이스의 값을 업데이트하는 함수
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // 현재 로그인된 Firebase 사용자를 가져옵니다.

        DatabaseReference root = FirebaseManager.Database.RootReference; // Firebase 데이터베이스의 루트 참조를 가져옵니다.
        DatabaseReference userCount = root.Child("GameData").Child("TotalUser"); // "GameData" 아래에 "TotalUser"라는 하위 참조를 만듭니다.

        userCount.RunTransaction(mutableData =>
        {
            if (mutableData.Value == null) // mutableData의 값이 null인 경우 예외처리 꼭 해야함.!
            {
                Debug.Log("TotalUser가 비어 있습니다. 초기값을 설정합니다.");
                mutableData.Value = 1; // 초기값을 1으로 설정합니다. 나 한명 있을테니.
                return TransactionResult.Success(mutableData); // 트랜잭션을 성공으로 반환합니다.
            }

            long totaluserCount = (long)mutableData.Value;
            mutableData.Value = totaluserCount + 1; // 현재 사용자 수를 증가시킵니다.

            return TransactionResult.Success(mutableData); // 트랜잭션을 성공으로 반환합니다.
        });
    }


    private void CheckLeaderBoard() // 버튼 클릭 시 호출되는 함수
    {
        DatabaseReference root = FirebaseManager.Database.RootReference; // Firebase 데이터베이스의 루트 참조를 가져옵니다.
        DatabaseReference wordRef = root.Child("UserData"); // "Word"라는 하위 참조를 만듭니다.

        wordRef.OrderByChild("winsCount").GetValueAsync().ContinueWithOnMainThread(task => // GetValueAsync 메서드를 사용하여 데이터를 가져옵니다.
        {
            if (task.IsCanceled) // 작업이 취소된 경우
            {
                Debug.LogError("데이터 가져오기 작업이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted) // 작업이 실패한 경우
            {
                Debug.LogError("데이터를 가져오는 데 실패했습니다: " + task.Exception);
                return;
            }
            Debug.Log("데이터 베이스 읽기 성공");
            DataSnapshot snapshot = task.Result; // 결과를 DataSnapshot으로 가져옵니다.

            // DataSnapshot의 자식들을 역순으로 가져옵니다.
            var children = snapshot.Children.Reverse().ToList(); // .ToList() 로 변환해야 for문에서 사용할 수 있습니다.
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                Debug.Log($"{i+1}등 {child.Child("name").Value} :  {child.Child("winsCount").Value} 승");
            }
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
    public int gameCount;
    public int winsCount;
    public bool clear;
    public List<string> skill;
}


[Serializable]  // Serializable 어트리뷰트를 사용하여 이 클래스가 JSON으로 직렬화될 수 있도록 합니다.
public class LeaderBoardData
{
    public List<Ranking> ranker; // 랭킹 리스트를 저장하는 리스트

    [Serializable]  // Serializable 어트리뷰트를 사용하여 이 클래스가 JSON으로 직렬화될 수 있도록 합니다.
    public class Ranking
    {
        public string name;
        public int winsCount;
    }
}
