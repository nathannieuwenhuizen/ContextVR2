﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFormChecker : MonoBehaviour
{
    [SerializeField]
    private Transform checkPos;

    private GameObject tempSelectedHaircut;     // Haircut the player has made
    private GameObject tempReferenceHaircut;    // Haircut the customer wants
    private GameObject tempGovermentHaircut;    // Haircut the government wants
    public Texture2D portaitShot;

    [Header("Other Info")]
    [SerializeField]
    private float cameraSize = 2.5f;
    [SerializeField]
    private int cellSize = 5;

    private Camera refCamera;

    public float desiredPrecentage = 0f;
    public float govermentPrecentage = 0f;
    public float conflictPrecentage = 0f;
    public bool calculating = false;

    [HideInInspector]
    public Texture2D refTexture;
    [HideInInspector]
    public Texture2D govermentTexture;
    [HideInInspector]
    public Texture2D selectedTexture;

    public MeshRenderer testPlane;

    int width = 0;
    int height = 0;

    void Start()
    {
        refCamera = GetComponent<Camera>();
        CameraSize = cameraSize;
        refCamera.backgroundColor = Data.PORTRAIT_BG;
    }

    public float CameraSize
    {
        get { return refCamera.orthographicSize * 2f; }
        set
        {
            refCamera.orthographicSize = value / 2f;
            refCamera.nearClipPlane = 0;
            refCamera.farClipPlane = value * 2;
            checkPos.localPosition = new Vector3(0, -value / 4f, value);
        }
    }

    public IEnumerator getPrecentageFilled(GameObject selected, Sprite sprite, Sprite govermentSprite)
    {
        if (!calculating)
        {


            calculating = true;

            //clone the objects to checkcamera
            tempSelectedHaircut = Instantiate(selected, checkPos);
            tempSelectedHaircut.transform.Rotate(180, 180, 0);

            tempSelectedHaircut.transform.localPosition = Vector3.zero;

            tempReferenceHaircut = spriteObject(sprite);
            tempGovermentHaircut = spriteObject(govermentSprite);

            while (tempReferenceHaircut == null || tempSelectedHaircut == null)
            {
                yield return new WaitForFixedUpdate();
            }

            //make textures and take screenshot
            width = Screen.width;
            height = Screen.height;

            selectedTexture = GetScreenShot(tempSelectedHaircut, width);
            refTexture = GetScreenShot(tempReferenceHaircut, width);
            govermentTexture = GetScreenShot(tempGovermentHaircut, width);


            MakePortraitShot();

            //Data.SaveTextureAsPNG(selectedTexture, "");
            testPlane.material.mainTexture = selectedTexture;

            tempReferenceHaircut.SetActive(true);
            //delete everything
            Destroy(tempReferenceHaircut);
            Destroy(tempSelectedHaircut);
            Destroy(tempGovermentHaircut);

            //tempReferenceHaircut = null;
            //tempSelectedHaircut = null;

            //refTexture = null;
            //selectedTexture = null;

            //update values
            desiredPrecentage = GetPrecentageMatchOfTextures(refTexture, selectedTexture);          // Comparison player-made with customer-wish
            govermentPrecentage = GetPrecentageMatchOfTextures(govermentTexture, selectedTexture);  // Comparison player-made government-wish
            conflictPrecentage = GetPrecentageMatchOfTextures(govermentTexture, refTexture);        // Comparison customer-wish with government-wish

            //Debug.Log(desiredPrecentage + "%");
            //Debug.Log(govermentPrecentage + "%");
            //Debug.Log(conflictPrecentage + "%");

            //GameManager.instance.OnHairCutCheck();

            calculating = false;
        }
    }

    public void MakePortraitShot()
    {
        float angle = 0;
        int delta = Mathf.RoundToInt(Random.Range(-2, 2)) * 45;
        angle = delta;
        tempSelectedHaircut.transform.Rotate(new Vector3(0, 0, angle));
        portaitShot = GetScreenShot(tempSelectedHaircut, Mathf.RoundToInt(height / 1.5f));
        tempSelectedHaircut.transform.Rotate(new Vector3(0, 0, -angle));

    }

    public GameObject spriteObject(Sprite _sprite)
    {
        GameObject tempObject = Instantiate(new GameObject(), checkPos);
        tempObject.transform.localPosition = Vector3.zero;
        tempObject.transform.Rotate(0, 90, 0);
        tempObject.AddComponent<SpriteRenderer>();

        //scale image accordingly
        if (tempObject.GetComponent<SpriteRenderer>() != null)
        {
            SpriteRenderer sr = tempObject.GetComponent<SpriteRenderer>();
            sr.sprite = _sprite;
            //Debug.Log("temo scale: " + tempSelectedHaircut.transform.localScale.x);
            tempObject.transform.localScale *= (_sprite.pixelsPerUnit / Data.HEAD_SIZE_PIXELS) * 1.2f;
        }

        return tempObject;
    }

    public float GetPrecentageMatchOfTextures(Texture2D textureA, Texture2D textureB)
    {
        //check the pixels
        cellSize = Mathf.Min(textureA.height, Mathf.Max(1, cellSize)); //to prevent infinite loop... sort of

        float result = 0;
        float total = 0;
        float correct = 0;

        for (int y = 0; y < textureA.height; y += cellSize)
        {
            for (int x = 0; x < textureA.width; x += cellSize)
            {
                Color col = textureA.GetPixel(x, y);
                Color col2 = textureB.GetPixel(x, y);

                bool colIsFilled = col != Data.PORTRAIT_BG;
                bool col2IsFilled = col2 != Data.PORTRAIT_BG;

                //if both are BG
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
        result = correct / total;
        //Debug.Log("total: " + total + " | correct: " + correct);

        return result;
    }


    public Texture2D GetScreenShot(GameObject selected, int widthOfTexture)
    {
        tempGovermentHaircut.SetActive(false);
        tempSelectedHaircut.SetActive(false);
        tempReferenceHaircut.SetActive(false);

        selected.SetActive(true);

        refCamera.Render();

        Texture2D tex = new Texture2D(widthOfTexture, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(refCamera.rect.x + (width / 2) - (widthOfTexture / 2) , refCamera.rect.y, widthOfTexture, height), 0, 0, false);
        tex.Apply();
        return tex;
    }
}
