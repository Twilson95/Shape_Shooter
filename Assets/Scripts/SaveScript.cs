using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;

[Serializable]
public class GameData
{
    //level manager
    public int baseLevel;
    public int currentLevel;
    public int highestLevel;
    //score manager
    public int score;
    //premium score manager
    public int premiumScore;
    //player stats
    public int weaponLevel;
    public int magnetLevel;
    public int baseStatsLevel;
}

public static class SaveScript
{
    public static void SaveGameData(GameData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameData.dat");
        bf.Serialize(file, data);
        file.Close();
    }

    // public static IEnumerator SaveGameDataAsync(GameData data)
    // {
    //     BinaryFormatter bf = new BinaryFormatter();
    //     string path = Application.persistentDataPath + "/gameData.dat";

    //     // Serialize the data asynchronously
    //     using (FileStream file = File.Create(path))
    //     {
    //         bf.Serialize(file, data);
    //         yield return null; // Yield once to give other coroutines a chance to run
    //         file.Flush(true); // Flush the data to disk
    //         file.Close();
    //     }
    // }

    private static readonly object saveLock = new object();

    public static IEnumerator SaveGameDataAsync(GameData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string filePath = Application.persistentDataPath + "/gameData.dat";

        // Acquire the lock to ensure that only one thread is accessing the file at a time
        lock (saveLock)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create))
            {
                // Serialize the data asynchronously
                bf.Serialize(file, data);

                // Flush and close the file asynchronously
                file.Flush();
                file.Close();
            }
        }

        yield return null;
    }

    // public static async Task SaveGameDataAsync(GameData data)
    // {
    //     BinaryFormatter bf = new BinaryFormatter();
    //     using (FileStream file = new FileStream(Application.persistentDataPath + "/gameData.dat", FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
    //     {
    //         await bf.SerializeAsync(file, data);
    //         await file.FlushAsync();
    //     }
    // }

    // public static async void SaveGameDataAsync(GameData data)
    // {
    //     await Task.Run(() =>
    //     {
    //         BinaryFormatter bf = new BinaryFormatter();
    //         FileStream file = File.Create(Application.persistentDataPath + "/gameData.dat");

    //         bf.Serialize(file, data);
    //         file.Flush();
    //         file.Close();
    //     });
    // }
 
    // public static IEnumerator SaveGameDataAsync(GameData data)
    // {
    //     BinaryFormatter bf = new BinaryFormatter();
    //     FileStream file = File.Create(Application.persistentDataPath + "/gameData.dat");

    //     // Serialize the data asynchronously
    //     //yield return new WaitForBackgroundThread();
    //     bf.Serialize(file, data);

    //     // Flush and close the file asynchronously
    //     //yield return new WaitForBackgroundThread();
    //     file.Flush();
    //     file.Close();
    // }

    public static GameData LoadGameData()
    {
        if (File.Exists(Application.persistentDataPath + "/gameData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameData.dat", FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            file.Close();
            return data;
        }
        else
        {
            return null;
        }
    }
}
