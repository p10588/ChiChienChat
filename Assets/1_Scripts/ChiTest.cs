using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChiChien.Core;
using ChiChien.DB;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class ChiTest : MonoBehaviour
{
    public Button TESTBTN;


    public string testId = "5a144bdd-7be0-4e59-bdd9-fe6da4021193";
    private DatabaseReference dbRootRef;

    // Start is called before the first frame update
    void Start()
    {
        this.dbRootRef = FirebaseDatabase.DefaultInstance.RootReference;
        //FriendDataDB ttt = new FriendDataDB(new UserDataDB("123", "123", "123", "123"), false, "qwessd", true," sender");
        TESTBTN.onClick.AddListener(OnTestBtnClick);
    }

    private async void OnTestBtnClick() {

        try {
            Dictionary<string,MessageDataDB> messages
                = await RetriveData<Dictionary<string, MessageDataDB>>(dbRootRef.Child("messageData").Child(testId).Child("messageList"));
            //foreach(KeyValuePair<string, object> m in message)
            Debug.Log(JsonConvert.SerializeObject(messages));
        } catch (Exception e) {
            Debug.Log(e);
        }

    }


    protected virtual async Task<T> RetriveData<T>(DatabaseReference dbRef, Action<T> callback = null, Action<T> fallback = null) {
        // Read the data from the database
        Task<DataSnapshot> t = dbRef.GetValueAsync();
        try {
            DataSnapshot snapshot = await t;
            if (snapshot != null) {
                T data = JsonConvert.DeserializeObject<T>(snapshot.GetRawJsonValue());
                Debug.Log(data);
                if (data == null) {
                    try { fallback.Invoke(data); } catch (Exception e) { Debug.Log(e); }
                    return default(T);
                } else {
                    try { callback.Invoke(data); } catch (Exception e) { Debug.Log(e); }
                    return data;
                }
            }
        } catch (AggregateException e) {
            Debug.LogError("Failed to read data from the database: " + e);
        }
        return default(T);
    }
}
