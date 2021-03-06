﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup mainScreen;
    [SerializeField]
    private CanvasGroup loadScreen;
    [SerializeField]
    private CanvasGroup settingScreen;
    [SerializeField]
    private CanvasGroup howToPlayScreen;

    private CanvasGroup activeScreen;
    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private Slider volumeSlider;

    public enum MenuScreens
    {
        main,
        load,
        settings,
        howto
    }
    private void Start()
    {

        activeScreen = mainScreen;

        AudioListener.volume = Settings.MusicVolume;
        volumeSlider.value = Settings.MusicVolume;

    }

    public void goToScreen(CanvasGroup toggleScreen)
    {
        if (activeScreen == toggleScreen)
        {
            return;
        }
        AudioManager.instance?.PlaySound(AudioEffect.uiClick, 1);

        activeScreen.interactable = false;
        activeScreen.blocksRaycasts = false;
        LeanTween.alphaCanvas(activeScreen, 0, fadeTime).setOnComplete(() =>
        {
            activeScreen = toggleScreen;
            activeScreen.interactable = true;
            activeScreen.blocksRaycasts = true;
            LeanTween.alphaCanvas(activeScreen, 1, fadeTime);
        });
    }

    public void UpdateVolume()
    {
        AudioManager.instance?.PlaySound(AudioEffect.scaleChange, 1);

        Settings.MusicVolume = volumeSlider.value;
        AudioListener.volume = volumeSlider.value;
    }
}
