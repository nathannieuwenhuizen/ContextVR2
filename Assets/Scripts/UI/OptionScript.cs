using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionScript : MonoBehaviour
{
    [SerializeField]
    private Slider volumeSlider;

    private void Start()
    {
        volumeSlider.value = Settings.MusicVolume;
        AudioListener.volume = Settings.MusicVolume;
    }

    public void UpdateVolume()
    {
        AudioManager.instance?.PlaySound(AudioEffect.scaleChange, 1);

        Settings.MusicVolume = volumeSlider.value;
        AudioListener.volume = volumeSlider.value;
    }
}
