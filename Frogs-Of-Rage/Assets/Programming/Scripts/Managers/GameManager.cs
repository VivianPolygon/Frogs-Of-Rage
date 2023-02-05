using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    //public int flys;


    ////Save Player Data
    //public void SaveGame()
    //{
    //    PlayerData playerData = CreateSaveGameObject();
    //    //Serializing the PlayerData instance
    //    BinaryFormatter bf = new BinaryFormatter();
    //    FileStream file = File.Create(Application.persistentDataPath + "/playerData.dat");
    //    bf.Serialize(file, playerData);

    //    Debug.Log(playerData.flyCount);

    //    file.Close();
    //}

    ////Load Player Data
    //public void LoadGame()
    //{
    //    if (File.Exists(Application.persistentDataPath + "/playerData.dat"))
    //    {
    //        BinaryFormatter bf = new BinaryFormatter();
    //        FileStream file = File.Open(Application.persistentDataPath + "/playerData.dat", FileMode.Open);
    //        PlayerData playerData = (PlayerData)bf.Deserialize(file);

    //        Debug.Log(playerData.flyCount);

    //        file.Close();
    //    }
    //}

    //private PlayerData CreateSaveGameObject()
    //{
    //    PlayerData save = new PlayerData();
        
    //    save.flyCount = flys;

    //    return save;
    //}

}
