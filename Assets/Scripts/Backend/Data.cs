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
    public static string HAIRCUTS_FOLDER_NAME = "/saves";

    //portrait info
    public static string PORTRAITS_FOLDER_NAME = "/saves/portraits";
    public static Color PORTRAIT_BG = new Color32(0xF3, 0xEE, 0xE7, 0xff);

    //govermentHair
    public static string GOVERMENT_FILE_NAME = "goverment.hair";

    //folder for player made haircuts
    public static string PLAYER_HAIRCUTS_FOLDER_NAME = "/saves/madeByPlayer";
    public static int MAX_FILES_IN_PLAYER_FOLDER = 100;

    public static void SavePortrait(Texture2D _texture, string fileName = "Image.png")
    {
        //first Make sure you're using RGB24 as your texture format
        Texture2D texture = new Texture2D(_texture.width, _texture.height, TextureFormat.RGB24, false);

        //then Save To Disk as PNG
        byte[] bytes = _texture.EncodeToPNG();
        var dirPath = Data.DataPath() + Data.PORTRAITS_FOLDER_NAME;
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "/" + fileName, bytes);
        
    }

    public static Texture2D LoadPortrait(string fileName = "Image.png")
    {
        Texture2D tex = null;
        byte[] fileData;

        string path = Data.DataPath() + Data.PORTRAITS_FOLDER_NAME + "/" + fileName;
        fileData = File.ReadAllBytes(path);
        tex = new Texture2D(2, 2);
        try
        {
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            return tex;
        }
        catch
        {
            Debug.LogErrorFormat("From Nathan: Fail to load save file at {0}", path);
            return null;
        }

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

        if (!Directory.Exists(Data.DataPath() + directory))
        {
            Directory.CreateDirectory(Data.DataPath() + directory);
        }

        string path = Data.DataPath() + directory + "/" + fileName;
        FileStream file = File.Create(path);

        formatter.Serialize(file, saveData);
        Debug.Log("It should be saved");
        return true;
    }

    public static string DataPath()
    {
        return Application.streamingAssetsPath;
    }

    public static object LoadHair(string directory = "/saves", string fileName = "testHairSave.hair")
    {
        string path = Data.DataPath() + directory + "/" + fileName;
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

    public static string[] GetFiles(string directory = "/saves")
    {
        if (!Directory.Exists(Data.DataPath() + directory))
        {
            Directory.CreateDirectory(Data.DataPath() + directory);
        }
        string[] files = Directory.GetFiles(Data.DataPath() + directory );

        List<string> result = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Contains(".meta"))
            {
                result.Add ( Path.GetFileName(files[i]) );
            }
        }
        return result.ToArray();
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