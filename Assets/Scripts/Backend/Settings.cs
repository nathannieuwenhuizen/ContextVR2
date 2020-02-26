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
    public static int amountOfCustomers
    {
        get
        {
            return PlayerPrefs.GetInt("amountCustomers", 1);
        }
        set
        {
            PlayerPrefs.SetInt("amountCustomers", value);
        }
    }
}
