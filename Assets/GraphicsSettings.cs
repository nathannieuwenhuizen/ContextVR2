using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class GraphicsSettings : MonoBehaviour {

    [SerializeField] private Slider resSlider;
    
    public void setGraphics(int tier) {
        XRSettings.eyeTextureResolutionScale = resSlider.value + .5f;
        QualitySettings.SetQualityLevel(tier, true);
    }
}