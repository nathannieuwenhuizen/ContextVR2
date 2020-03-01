using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFormChecker : MonoBehaviour
{
    [SerializeField]
    private Transform checkPos;

    private GameObject tempSelectedHaircut;
    private GameObject tempReferenceHaircut;

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
            checkPos.localPosition = new Vector3(0, 0, value / 2f);
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
            Debug.Log("temo scale: " + tempSelectedHaircut.transform.localScale.x);
            tempReferenceHaircut.transform.localScale *= (sprite.pixelsPerUnit / Data.HEAD_SIZE_PIXELS) * tempSelectedHaircut.transform.localScale.x;
        }

        while (tempReferenceHaircut == null || tempSelectedHaircut == null)
        {
            yield return new WaitForFixedUpdate();
        }

        //make textures and take screenshot
        int width = Screen.width;
        int height = Screen.height;

        tempReferenceHaircut.SetActive(true);
        tempSelectedHaircut.SetActive(false);
        //yield return new WaitForEndOfFrame();
        refCamera.Render();

        refTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        refTexture.ReadPixels(new Rect(refCamera.rect.x, refCamera.rect.y, width, height), 0, 0, false);
        refTexture.Apply();

        tempReferenceHaircut.SetActive(false);
        tempSelectedHaircut.SetActive(true);
        //yield return new WaitForEndOfFrame();
        refCamera.Render();

        selectedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        selectedTexture.ReadPixels(new Rect(refCamera.rect.x, refCamera.rect.y, width, height), 0, 0, false);
        selectedTexture.Apply();

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

        Debug.Log("total: " + total + " | correct: " + correct);
        //update values
        precentageCorrect = correct / total;
        GameManager.instance.OnHairCutCheck();

        calculating = false;
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
