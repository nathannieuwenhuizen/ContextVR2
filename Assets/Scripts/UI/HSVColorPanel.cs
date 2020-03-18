using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HSVColorPanel : MonoBehaviour {

    public Color color;
    [SerializeField] GameObject HSV;
    [SerializeField] GameObject SWATCHES;

    [Header("HSV SLIDERS")]
    [SerializeField] Slider Hue;
    [SerializeField] Slider Saturation;
    [SerializeField] Slider Value;
    [SerializeField] Image[] colorDisplay;
    [SerializeField] Texture2D SaturationBG;
    [SerializeField] Texture2D ValueBG;

    [Header("SWATCHES")]
    [SerializeField] int selectedColorIndex = 0;
    [SerializeField] int swatchesWidth;
    [SerializeField] float swatchesDistanceX;
    [SerializeField] float swatchesDistanceY;
    [SerializeField] Color[] swatches;
    [SerializeField] GameObject swatchPrefab;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite selectedSprite;
    GameObject[] swatchbuttons;

    [Header("PARTICLE")]
    [SerializeField]
    private GameObject particlePrefab;
    private GameObject particleInstance;
    public GameObject selectedObject;

    private float oldPitch = 0;
    public static HSVColorPanel instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start() {
        ChaingedColor();
        updateSaturation();
        updateValue();
        resetSwatches();

        //StartCoroutine(test());
    }

    void resetSwatches() {
        foreach (Transform child in SWATCHES.transform) GameObject.Destroy(child.gameObject);

        swatchbuttons = new GameObject[swatches.Length];

        for (int i = 0; i < swatches.Length; i++) {
            swatchbuttons[i] = GameObject.Instantiate(swatchPrefab, SWATCHES.transform);
            int xp = i;
            int yp = i / swatchesWidth;
            while (xp >= swatchesWidth) xp -= swatchesWidth;
            swatchbuttons[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(xp * swatchesDistanceX, -yp * swatchesDistanceY);
            swatchbuttons[i].GetComponent<Image>().color = swatches[i];

            int ii = i; //WHY DOES THIS WORK
            swatchbuttons[i].GetComponent<Button>().onClick.AddListener(delegate { PressSwatch(ii); });
        }
    }

    //for testing
    public IEnumerator test()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            SelectedColor = new Color(Random.value, Random.value, Random.value);
        }
    }

    public GameObject SelectedObject
    {
        set
        {
            //Debug.Log(" selected object: " + value);
            selectedObject = value;
            if (selectedObject != null)
            {
                SelectedColor = selectedObject.GetComponent<MeshRenderer>().material.color;
            }
        }
    }

    public Color SelectedColor
    {
        get { return color; }
        set
        {
            color = value;

            Color.RGBToHSV(value, out float H, out float S, out float V);
            Hue.value = H;

            updateSaturation();
            Saturation.value = S;

            updateValue();
            Value.value = V;

            //play color change sound
            AudioManager.instance?.Play3DSound(AudioEffect.colorChange, 1, HSV.transform.position);


            if (selectedObject != null)
            {
                selectedObject.GetComponent<MeshRenderer>().material.color = color;
                SpawnParticle(selectedObject);
            }
        }
    }
    public void SpawnParticle(GameObject atObject)
    {
        if (particleInstance == null)
        {
            particleInstance = Instantiate(particlePrefab);
        }
        particleInstance.GetComponent<ParticleSystem>().Play();
        particleInstance.transform.position =  atObject.transform.position;
        particleInstance.GetComponent<ParticleSystem>().startColor = color;
    }

    public void PressSwatch(int index) {
        swatchbuttons[selectedColorIndex].GetComponent<Image>().sprite = normalSprite;
        swatchbuttons[index].GetComponent<Image>().sprite = selectedSprite;
        selectedColorIndex = index;
        SelectedColor = swatches[index];
    }

    public void ChaingedColor() {

        color = Color.HSVToRGB(Hue.value, Saturation.value, Value.value);


        //sound effect with pitch
        float pitchValue = (Hue.value + Saturation.value + Value.value) / 3f;
        if (Mathf.Abs(pitchValue - oldPitch) > 0.02f)
        {
            oldPitch = pitchValue;
            AudioManager.instance?.Play3DSound(AudioEffect.colorChange, 1, transform.position, false, .5f + pitchValue);
        }

        for (int i = 0; i < colorDisplay.Length; i++) {
            colorDisplay[i].color = color;
        }

        if (selectedObject != null)
        {
            selectedObject.GetComponent<MeshRenderer>().material.color = color;
        }
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

    public void switchPanel() {
        AudioManager.instance?.Play3DSound(AudioEffect.uiClick, 1, transform.position);

        if (HSV.active) {
            HSV.SetActive(false);
            SWATCHES.SetActive(true);
        }
        else {
            HSV.SetActive(true);
            SWATCHES.SetActive(false);
        }
    }
}