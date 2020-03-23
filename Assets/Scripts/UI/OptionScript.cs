using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionScript : MonoBehaviour
{
    [SerializeField]
    private Slider volumeSlider;

    [SerializeField]
    private Slider musicSlider;

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

    public void UpdateMusic()
    {
        //Idk hoe dit geimplementeerd is
    }

    public void EnableSubs()
    {
        //We hebben geen subtitles
    }

}
