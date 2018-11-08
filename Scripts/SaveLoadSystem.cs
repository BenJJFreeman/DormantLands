using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class SaveLoadSystem{

	

    public static void Save(GameData _gameData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameInfo.dat");

        GameData data = new GameData();

        data = _gameData;
        //
        bf.Serialize(file, data);
        file.Close();
    }
    public static GameData Load()
    {
        if (File.Exists(Application.persistentDataPath + "/gameInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameInfo.dat", FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            file.Close();
            return data;
            //
        }
        return new GameData();
    }

    public static void SaveOptions(OptionsData _OptionsData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/options.dat");

        OptionsData data = new OptionsData();

        data = _OptionsData;
        //
        bf.Serialize(file, data);
        file.Close();
    }
    public static OptionsData LoadOptions()
    {
        if (File.Exists(Application.persistentDataPath + "/options.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/options.dat", FileMode.Open);
            OptionsData data = (OptionsData)bf.Deserialize(file);
            file.Close();
            return data;
            //
        }
        return new OptionsData();
    }
}
[System.Serializable]
public struct GameData
{
    public int level;
    public GameData (int _level)
    {
        level = _level;

    }
}
[System.Serializable]
public struct OptionsData
{
    public int resolution;
    public int quality;
    public bool fullscreen;

    public float music,ambience, effects;

    public OptionsData(int _resolution,int _quality,bool _fullscreen,float _music,float _ambience,float _effects)
    {
        resolution = _resolution;
        quality = _quality;
        fullscreen = _fullscreen;
        music = _music;
        ambience = _ambience;
        effects = _effects;

    }
}