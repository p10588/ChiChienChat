using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;
namespace ChiChien.DB {

    public class FirebaseDB {

        public enum ChildEvent{
            ChildAdded,
            ChildChanged,
            ChildMoved,
            ChildRemoved,
        }

        public static async Task<T> GetData<T>(DatabaseReference dbRef,
                                                   Action<T> callback = null,
                                                   Action<T> fallback = null) {
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

        public static async void AddData<T>(DatabaseReference dbRef, T obj,
                                              Action<T> callback = null,
                                              Action<T> fallback = null) {
            // Write the data to the database
            string dataJson = JsonConvert.SerializeObject(obj);

            await dbRef.Push().SetRawJsonValueAsync(dataJson).ContinueWith(task => {
                if (task.IsFaulted) {
                    Debug.LogError("Failed to write data to the database: " + task.Exception);
                    try { fallback.Invoke((T)obj); } catch (Exception e) { Debug.Log(e); }
                } else if (task.IsCompleted) {
                    Debug.Log("Data written to the database successfully!");
                    try { callback.Invoke((T)obj); } catch (Exception e) { Debug.Log(e); }
                }
            });
        }

        public static async void UpdateData<T>(DatabaseReference dbRef, T obj,
                                              Action<T> callback = null,
                                              Action<T> fallback = null) {
            // Write the data to the database
            string friendDataJson = JsonConvert.SerializeObject(obj);

            await dbRef.SetRawJsonValueAsync(friendDataJson).ContinueWith(task => {
                if (task.IsFaulted) {
                    Debug.LogError("Failed to write data to the database: " + task.Exception);
                    try { fallback.Invoke((T)obj); } catch (Exception e) { Debug.Log(e); }
                } else if (task.IsCompleted) {
                    Debug.Log("Data written to the database successfully!");
                    try { callback.Invoke((T)obj); } catch (Exception e) { Debug.Log(e); }
                }
            });
        }

        public static async void RemoveData(DatabaseReference dbRef) {
            await dbRef.RemoveValueAsync();
        }

        public static void ReqiestorEventListener(DatabaseReference dbRef, ChildEvent eventType,
                                         EventHandler<ChildChangedEventArgs> callback) {
            switch (eventType) {
                case ChildEvent.ChildAdded:
                    dbRef.ChildAdded += callback;
                    break;
                case ChildEvent.ChildChanged:
                    dbRef.ChildChanged += callback;
                    break;
                case ChildEvent.ChildMoved:
                    dbRef.ChildMoved += callback;
                    break;
                case ChildEvent.ChildRemoved:
                    dbRef.ChildRemoved += callback;
                    break;
            }
        }

        public static void ReqiestorEventListener(DatabaseReference dbRef,
                                         EventHandler<ValueChangedEventArgs> callback) {
            dbRef.ValueChanged += callback;
        }

        public static void ReqiestorEventListener(Query dbQuery, ChildEvent eventType,
                                         EventHandler<ChildChangedEventArgs> callback) {
            switch (eventType) {
                case ChildEvent.ChildAdded:
                dbQuery.ChildAdded += callback;
                break;
                case ChildEvent.ChildChanged:
                dbQuery.ChildChanged += callback;
                break;
                case ChildEvent.ChildMoved:
                dbQuery.ChildMoved += callback;
                break;
                case ChildEvent.ChildRemoved:
                dbQuery.ChildRemoved += callback;
                break;
            }
        }

        public static void ReqiestorEventListener(Query dbQuery,
                                         EventHandler<ValueChangedEventArgs> callback) {
            dbQuery.ValueChanged += callback;
        }


        public static void UnreqiestorEventListener(DatabaseReference dbRef, ChildEvent eventType,
                                         EventHandler<ChildChangedEventArgs> callback) {
            switch (eventType) {
                case ChildEvent.ChildAdded:
                dbRef.ChildAdded -= callback;
                break;
                case ChildEvent.ChildChanged:
                dbRef.ChildChanged -= callback;
                break;
                case ChildEvent.ChildMoved:
                dbRef.ChildMoved -= callback;
                break;
                case ChildEvent.ChildRemoved:
                dbRef.ChildRemoved -= callback;
                break;
            }
        }

        public static void UnreqiestorEventListener(DatabaseReference dbRef,
                                         EventHandler<ValueChangedEventArgs> callback) {
            dbRef.ValueChanged -= callback;
        }

        public static void UnreqiestorEventListener(Query dbQuery, ChildEvent eventType,
                                         EventHandler<ChildChangedEventArgs> callback) {
            switch (eventType) {
                case ChildEvent.ChildAdded:
                dbQuery.ChildAdded -= callback;
                break;
                case ChildEvent.ChildChanged:
                dbQuery.ChildChanged -= callback;
                break;
                case ChildEvent.ChildMoved:
                dbQuery.ChildMoved -= callback;
                break;
                case ChildEvent.ChildRemoved:
                dbQuery.ChildRemoved -= callback;
                break;
            }
        }

        public static void UnreqiestorEventListener(Query dbQuery,
                                         EventHandler<ValueChangedEventArgs> callback) {
            dbQuery.ValueChanged -= callback;
        }

    }

}








