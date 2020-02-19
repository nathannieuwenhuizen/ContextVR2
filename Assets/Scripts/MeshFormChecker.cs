using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFormChecker : MonoBehaviour
{

    [SerializeField]
    private GameObject selectedHairCut;

    [SerializeField]
    private GameObject referenceHaircut;
    private Texture2D referenceTexture;

    [SerializeField]
    private Camera camera;

    private int mWidth = 1200;
    private int mHeight = 1200;

    [SerializeField]
    private Material blackMaterial;

    public float getPrecentageFilled()
    {
        int width = Screen.width;
        int height = Screen.height;

        referenceHaircut.SetActive(true);
        selectedHairCut.SetActive(false);
        camera.Render();

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(camera.rect.x, camera.rect.y, width, height), 0, 0, false);
        texture.Apply();

        referenceHaircut.SetActive(false);
        selectedHairCut.SetActive(true);
        camera.Render();

        Texture2D texture2 = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture2.ReadPixels(new Rect(camera.rect.x, camera.rect.y, width, height), 0, 0, false);
        texture2.Apply();


        int cellsize = 5;
        float total = 0;
        float correct = 0;
        for (int y = 0; y < texture.height; y+= cellsize)
        {
            for (int x = 0; x < texture.width; x+= cellsize)
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
        referenceHaircut.SetActive(true);
        selectedHairCut.SetActive(true);
        //Debug.Log("total: " + total + " | differs: " + differs);
        //texture.Apply();

        //renderTexture = null; 
        return correct / total;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(wait());
        getPrecentageFilled();
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.5f);
        //getPrecentageFilled();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("precentage: "+ getPrecentageFilled());

    }
}
