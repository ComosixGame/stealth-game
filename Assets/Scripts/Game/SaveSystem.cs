using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem<T> where T : class, new() {
    private static string EncryptWord = "$2a$12$bFGaHUjLQsB12SXjTPGzIuh1Gqc93EA4Wow0ka93uWxZ31X1sfdKy";
    private static string directory = "/Saves";
    public static void Save(T data) {
        if(!Directory.Exists(Application.persistentDataPath + directory)) {
            Directory.CreateDirectory(Application.persistentDataPath + directory);
        }
        string json = JsonUtility.ToJson(data);
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"{directory}/{typeof(T).Name}.sav";
        FileStream file = File.Create(path);
        formatter.Serialize(file, Encrypt(json));
        file.Close(); 

    }

    public static T Load() {
        string path = Application.persistentDataPath + $"{directory}/{typeof(T).Name}.sav";
        if(File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            string json = formatter.Deserialize(file) as string;
            T data = JsonUtility.FromJson<T>(Encrypt(json));
            file.Close();
            return data;
        } else {
            T newData = new T();
            Save(newData);
            return newData;
        }
    }

    private static string Encrypt(string text) {
        string modifiedData = "";
        for(int i = 0; i<text.Length; i++) {
            modifiedData += (char) (text[i] ^ EncryptWord[i % EncryptWord.Length]);
        }

        return modifiedData;
    }
}

[System.Serializable]
public class PlayerData
{
    public int money;
    public List<int> levels;


    public PlayerData() {
        money = 0;
        levels = new List<int>();
    }
    
    public static PlayerData Load() {
        return SaveSystem<PlayerData>.Load();
    }

    public void Save() {
        SaveSystem<PlayerData>.Save(this);
    }

}

[System.Serializable]
public class SettingData
{
    public bool mute;

    public SettingData() {
        mute = false;
    }
    
    public static SettingData Load() {
        return SaveSystem<SettingData>.Load();
    }

    public void Save() {
        SaveSystem<SettingData>.Save(this);
    }

}
