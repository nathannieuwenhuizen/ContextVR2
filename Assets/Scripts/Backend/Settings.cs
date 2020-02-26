using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static float Volume
    {
        get
        {
            return PlayerPrefs.GetFloat("Volume", 1);
        }
        set
        {
            PlayerPrefs.SetFloat("Volume", value);
            AudioListener.volume = value;
        }
    }
    public static float AveragePrecentage
    {
        get { return (float)Settings.TotalPrecentage / (float)Settings.AmountOfCustomers; }
    }
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
    public static float TotalPrecentage
    {
        get
        {
            return PlayerPrefs.GetFloat("totalPrecentage", 0);
        }
        set
        {
            PlayerPrefs.SetFloat("totalPrecentage", value);
        }
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }
}
