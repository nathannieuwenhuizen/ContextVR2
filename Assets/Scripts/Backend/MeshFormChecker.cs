using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFormChecker : MonoBehaviour
{
    [SerializeField]
    private Transform checkPos;

    private GameObject tempSelectedHaircut;
    private GameObject tempReferenceHaircut;
    public Texture2D portaitShot;

    [Header("Other Info")]
    [SerializeField]
    private float cameraSize = 2f;
    [SerializeField]
    private int cellSize = 5;

    private Camera refCamera;

    public float precentageCorrect = 0f;
    private bool calculating = false;

    private Texture2D refTexture;
    [HideInInspector]
    public Texture2D selectedTexture;

    public MeshRenderer testPlane;

    int width = 0;
    int height = 0;

    void Start()
    {
        refCamera = GetComponent<Camera>();
        CameraSize = cameraSize;
    }

    public float CameraSize
    {
        get { return refCamera.orthographicSize * 2f; }
        set
        {
            refCamera.orthographicSize = value / 2f;
            refCamera.nearClipPlane = 0;
            refCamera.farClipPlane = value;
            checkPos.localPosition = new Vector3(0, -value / 4f, value / 2f);
        }
    }

    public IEnumerator getPrecentageFilled(GameObject selected, Sprite sprite)
    {
        calculating = true;

        //clone the objects to checkcamera
        tempSelectedHaircut = Instantiate(selected, checkPos);
        tempSelectedHaircut.transform.localPosition = Vector3.zero;

        tempReferenceHaircut = Instantiate(new GameObject(), checkPos);
        tempReferenceHaircut.transform.localPosition = Vector3.zero;
        tempReferenceHaircut.AddComponent<SpriteRenderer>();

        //scale image accordingly
        if(tempReferenceHaircut.GetComponent<SpriteRenderer>() != null)
        {
            SpriteRenderer sr = tempReferenceHaircut.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
            //Debug.Log("temo scale: " + tempSelectedHaircut.transform.localScale.x);
            tempReferenceHaircut.transform.localScale *= (sprite.pixelsPerUnit / Data.HEAD_SIZE_PIXELS) * tempSelectedHaircut.transform.localScale.x;
        }

        while (tempReferenceHaircut == null || tempSelectedHaircut == null)
        {
            yield return new WaitForFixedUpdate();
        }

        //make textures and take screenshot
        width = Screen.width;
        height = Screen.height;

        refTexture = GetScreenShot(false, width);
        selectedTexture = GetScreenShot(true, width);

        portaitShot = GetScreenShot(true , Mathf.RoundToInt(height / 1.5f));

        //Data.SaveTextureAsPNG(selectedTexture, "");
        testPlane.material.mainTexture = selectedTexture;

        //check the pixels
        cellSize = Mathf.Min(refTexture.height, Mathf.Max(1, cellSize)); //to prevent infinite loop... sort of

        float total = 0;
        float correct = 0;
        for (int y = 0; y < refTexture.height; y+= cellSize)
        {
            for (int x = 0; x < refTexture.width; x+= cellSize)
            {
                Color col = refTexture.GetPixel(x, y);
                Color col2 = selectedTexture.GetPixel(x, y);

                bool colIsFilled = col != Color.white;
                bool col2IsFilled = col2 != Color.white;

                //if both are white
                if (!colIsFilled && !col2IsFilled)
                {
                    continue;
                }
                total++;

                //if both are filled with shapes
                if (col2IsFilled && colIsFilled)
                {
                    correct++;
                }

                
            }
        }

        tempReferenceHaircut.SetActive(true);
        //delete everything
        Destroy(tempReferenceHaircut);
        tempReferenceHaircut = null;
        Destroy(tempSelectedHaircut);
        tempSelectedHaircut = null;

        //refTexture = null;
        //selectedTexture = null;

        //Debug.Log("total: " + total + " | correct: " + correct);
        //update values
        precentageCorrect = correct / total;
        GameManager.instance.OnHairCutCheck();

        calculating = false;
    }

    public Texture2D GetScreenShot(bool selected, int widthOfTexture)
    {
        //Debug.Log("screenshot width: " + widthOfTexture);
        tempSelectedHaircut.SetActive(selected);
        tempReferenceHaircut.SetActive(!selected);

        refCamera.Render();

        Texture2D tex = new Texture2D(widthOfTexture, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(refCamera.rect.x + (width / 2) - (widthOfTexture / 2) , refCamera.rect.y, widthOfTexture, height), 0, 0, false);
        tex.Apply();
        return tex;
    }

    public void CompareMeshes(GameObject selected, Sprite reference)
    {
        if (calculating) { return; }
        else
        {
            calculating = true;
            StartCoroutine(getPrecentageFilled(selected, reference));
        }
    }

    void Update()
    {
        /*
        if (calculating) { return; }
        else
        {
            calculating = true;
            StartCoroutine(getPrecentageFilled(selectedHairCut, referenceHaircut));
        }*/
    }
}
