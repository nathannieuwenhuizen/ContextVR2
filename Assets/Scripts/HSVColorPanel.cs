using UnityEngine;
using UnityEngine.UI;

public class HSVColorPanel : MonoBehaviour {

    public Color color;
    [SerializeField] Slider Hue;
    [SerializeField] Slider Saturation;
    [SerializeField] Slider Value;

    [SerializeField] InputField hex;
    [SerializeField] Image colorDisplay;
    [SerializeField] Texture2D SaturationBG;
    [SerializeField] Texture2D ValueBG;

    private void Start() {
        ChaingedColor();
        updateSaturation();
        updateValue();
    }

    public void ChaingedColor() {
        color = Color.HSVToRGB(Hue.value, Saturation.value, Value.value);
        colorDisplay.color = color;
        hex.text = "#" + ColorUtility.ToHtmlStringRGB(color);
    }

    public void updateSaturation() {
        for (int i = 0; i < 32; i++) {
            SaturationBG.SetPixel(i, 1, Color.HSVToRGB(Hue.value, i / 32f, Value.value));
        }
        SaturationBG.Apply();
    }

    public void updateValue() {
        for (int i = 0; i < 32; i++) {
            ValueBG.SetPixel(i, 1, Color.HSVToRGB(Hue.value, Saturation.value, i / 32f));
        }
        ValueBG.Apply();
    }
}