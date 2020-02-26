using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Data
{

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
}
