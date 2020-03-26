using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class GraphicsSettings : MonoBehaviour {

    [SerializeField] private Slider resSlider;
    [SerializeField] private GameObject[] shadows;

    public void setGraphics(int tier) {
        XRSettings.eyeTextureResolutionScale = resSlider.value + .5f;
        QualitySettings.SetQualityLevel(tier, true);
        shadows[0].SetActive(false);
        shadows[1].SetActive(false);
        shadows[2].SetActive(false);
        shadows[tier].SetActive(true);
    }
}