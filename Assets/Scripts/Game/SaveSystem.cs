using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem<T> where T : class, new() {
    private static string EncryptWord = "$2a$12$bFGaHUjLQsB12SXjTPGzIuh1Gqc93EA4Wow0ka93uWxZ31X1sfdKy";
    private static string directory = "/Saves";
    public static void Save(T data, bool useEncrypt = false) {
        if(!Directory.Exists(Application.persistentDataPath + directory)) {
            Directory.CreateDirectory(Application.persistentDataPath + directory);
        }
        string json = JsonUtility.ToJson(data);
        string path = Application.persistentDataPath + $"{directory}/{typeof(T).Name}.sav";
        if(useEncrypt) {
            json = Encrypt(json);
        }
        File.WriteAllText(path, json);

    }

    public static T Load(bool useEncrypt = false) {
        string path = Application.persistentDataPath + $"{directory}/{typeof(T).Name}.sav";
        if(File.Exists(path)) {
            string json = File.ReadAllText(path);
            if(useEncrypt) {
                json = Encrypt(json);
            }
            T data = JsonUtility.FromJson<T>(json);
            return data;
        } else {
            T newData = new T();
            Save(newData, useEncrypt);
            return newData;
        }
    }

    private static string Encrypt (string text) {
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
    public void Save() {
        SaveSystem<PlayerData>.Save(this, true);
    }
    
    public static PlayerData Load() {
        return SaveSystem<PlayerData>.Load(true);
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