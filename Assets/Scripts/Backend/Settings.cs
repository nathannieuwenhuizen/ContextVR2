using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("Music", 1);
        }
        set
        {
            PlayerPrefs.SetFloat("Music", value);
            AudioListener.volume = value;
        }
    }
    public static float SoundEffectVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("SoundEffect", 1);
        }
        set
        {
            PlayerPrefs.SetFloat("SoundEffect", value);
            AudioListener.volume = value;
        }
    }
    //public static float AveragePrecentage
    //{
    //    get { return (float)Settings.TotalPrecentage / (float)Settings.AmountOfCustomers; }
    //}
    public static int AmountOfCustomers
    {
        get
        {
            return PlayerPrefs.GetInt("amountCustomers", 0);
        }
        set
        {
            PlayerPrefs.SetInt("amountCustomers", value);
        }
    }
    //public static float TotalPrecentage
    //{
    //    get
    //    {
    //        return PlayerPrefs.GetFloat("totalPrecentage", 0);
    //    }
    //    set
    //    {
    //        PlayerPrefs.SetFloat("totalPrecentage", value);
    //    }
    //}

    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }
}
