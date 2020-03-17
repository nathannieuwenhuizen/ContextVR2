using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Runtime.Serialization;


public static class Data
{
    //formchecker data
    public static int HEAD_SIZE_PIXELS = 160;
    public static int HAIR_TOTAL_SIZE_PIXELS = 512;

    //recuring character data
    public static bool RECURRING_CHARACTER_IS_POSITIVE_SINCE_LAST_VISIT = true;
    public static string RECURRING_CHARACTER_HAIRCUT_CURRENT_FILENAME = "recurringCharacter.hair";
    public static int RECURING_CHARACTER_VISITS = 0;

    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        //first Make sure you're using RGB24 as your texture format
        Texture2D texture = new Texture2D(_texture.width, _texture.height, TextureFormat.RGB24, false);

        //then Save To Disk as PNG
        byte[] bytes = _texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".png", bytes);
        
    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        Debug.Log("Application path: " + Application.dataPath);
        
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        
        return tex;
    }

    /// <summary>
    /// Loads JSON file as a text variable
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string LoadJSONFileAsText(string path)
    {

        string filePath = "Dialogues/" + path.Replace(".json", "");

        TextAsset targetFile = Resources.Load<TextAsset>(filePath);

        //Debug.Log(targetFile.text);
        return targetFile.text;
    }


    public static bool SaveHair(string directory = "/saves", string fileName = "testHairSave.hair", object saveData = null)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.dataPath + directory))
        {
            Directory.CreateDirectory(Application.dataPath + directory);
        }

        string path = Application.dataPath + directory + "/" + fileName;
        FileStream file = File.Create(path);

        formatter.Serialize(file, saveData);
        Debug.Log("It should be saved");
        return true;
    }

    public static object LoadHair(string directory = "/saves", string fileName = "testHairSave.hair")
    {
        string path = Application.dataPath + directory + "/" + fileName;
        if (!File.Exists(path)) {
            Debug.LogErrorFormat("Form nathan: THe file doesnt exist at {0}", path);
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("From Nathan: Fail to load save file at {0}", path);
            file.Close();
            return null;
        }
    }

    public static string[] GetHairFiles(string directory = "/saves")
    {
        if (!Directory.Exists(Application.dataPath + directory))
        {
            Directory.CreateDirectory(Application.dataPath + directory);
        }
        return Directory.GetFiles(Application.dataPath + directory);
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        Vector3SerializationSurrogate vector3Surrogate = new Vector3SerializationSurrogate();
        QuaternionSerializationSurrogate quaternionSurrogate = new QuaternionSerializationSurrogate();
        ColorSerializationSurrogate colorSurrogate = new ColorSerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);
        selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }


}


[System.Serializable]
public class DialogueData{
    public bool recurringCharacter = false;
    [Header("If multiple files, the first one is negative and the last one positive.")]
    public string[] fileNames;
}