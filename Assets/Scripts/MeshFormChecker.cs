using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFormChecker : MonoBehaviour
{
    [Header("Reference objects")]
    [SerializeField]
    private GameObject selectedHairCut;

    [SerializeField]
    private GameObject referenceHaircut;

    [SerializeField]
    private Transform checkPos;

    private GameObject tempSelectedHaircut;
    private GameObject tempReferenceHaircut;

    [Header("Other Info")]
    [SerializeField]
    private float cameraSize = 2f;
    [SerializeField]
    private int cellSize = 5;

    private Texture2D referenceTexture;
    private Camera refCamera;

    private float precentageCorrect = 0f;
    private bool calculating = false;

    public MeshRenderer testPlane;
    
    void Start()
    {
        refCamera = GetComponent<Camera>();
        CameraSize = cameraSize;
        //getPrecentageFilled();
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

    public void CloneHair()
    {
        tempSelectedHaircut = Instantiate(selectedHairCut, checkPos);
        tempSelectedHaircut.transform.localPosition = Vector3.zero;
        tempReferenceHaircut = Instantiate(referenceHaircut, checkPos);
        tempReferenceHaircut.transform.localPosition = Vector3.zero;

    }

    public IEnumerator getPrecentageFilled()
    {
        calculating = true;

        //clone the objects to checkcamera
        CloneHair();
        while (tempReferenceHaircut == null || tempSelectedHaircut == null)
        {
            yield return new WaitForFixedUpdate();
        }

        //make textures and take screenshot
        int width = Screen.width;
        int height = Screen.height;

        tempReferenceHaircut.SetActive(true);
        tempSelectedHaircut.SetActive(false);
        refCamera.Render();

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(refCamera.rect.x, refCamera.rect.y, width, height), 0, 0, false);
        texture.Apply();

        tempReferenceHaircut.SetActive(false);
        tempSelectedHaircut.SetActive(true);
        refCamera.Render();

        Texture2D texture2 = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture2.ReadPixels(new Rect(refCamera.rect.x, refCamera.rect.y, width, height), 0, 0, false);
        texture2.Apply();

        testPlane.material.mainTexture =texture2;

        //check the pixels
        cellSize = Mathf.Min(texture.height, Mathf.Max(1, cellSize)); //to prevent infinite loop... sort of

        float total = 0;
        float correct = 0;
        for (int y = 0; y < texture.height; y+= cellSize)
        {
            for (int x = 0; x < texture.width; x+= cellSize)
            {
                Color col = texture.GetPixel(x, y);
                Color col2 = texture2.GetPixel(x, y);

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

        //delete everything
        Destroy(tempReferenceHaircut);
        tempReferenceHaircut = null;
        Destroy(tempSelectedHaircut);
        tempSelectedHaircut = null;
        texture = null;
        texture2 = null;

        Debug.Log("total: " + total + " | correct: " + correct);
        //update values
        precentageCorrect = correct / total;
        calculating = false;
    }

    void Update()
    {
        if (calculating) { return; }
        else
        {
            calculating = true;
            StartCoroutine(getPrecentageFilled());
        }
    }
}
