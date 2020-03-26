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
        AudioListener.volume = Settings.Volume;
        volumeSlider.value = Settings.Volume;
    }

    public void UpdateVolume()
    {
        AudioManager.instance?.PlaySound(AudioEffect.scaleChange, 1);

        Settings.Volume = volumeSlider.value;
        AudioListener.volume = volumeSlider.value;
    }
}
