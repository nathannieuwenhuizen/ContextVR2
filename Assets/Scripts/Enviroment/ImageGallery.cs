using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageGallery : MonoBehaviour
{

    [SerializeField]
    private GameObject framePrefab;
    [SerializeField]
    private List<GameObject> frames;

    [SerializeField]
    private float framesPadding = 1.5f;

    [SerializeField]
    private int maxFrames = 5;

    private int index = 0;

    void Start()
    {
        frames = new List<GameObject>();
        LoadGallery();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddFrame();
        }
    }

    public void LoadGallery()
    {
        string[] fileNames = Data.GetFiles(Data.PORTRAITS_FOLDER_NAME);

        for (int i = 0; i < Mathf.Min(fileNames.Length, maxFrames); i++)
        {

            Texture2D newTexture = Data.LoadPortrait(fileNames[i]);
            if (newTexture == null)
            {
                Debug.Log("no texture");
            }
            else
            {
                AddFrame(newTexture);
            }
        }
    }

    public void AddFrame(Texture2D texture = null)
    {
        GameObject newFrame;

        //update list
        if (index >= maxFrames)
        {
            newFrame = frames[index % maxFrames];

        } else
        {
            newFrame = Instantiate(framePrefab, transform);
            frames.Add(newFrame);
        }

        //save portrait image
        if (texture)
        {
            newFrame.transform.GetChild(1).GetComponent<MeshRenderer>().material.mainTexture = texture;
            Data.SavePortrait(texture, (index % maxFrames) + ".png");
        }

        //set name and location
        newFrame.transform.localPosition = new Vector3((index % maxFrames) * framesPadding, 0, 0);
        newFrame.name = "frame #" + index % maxFrames;

        //animation
        newFrame.transform.Rotate(new Vector3(0, 0, 45));
        newFrame.transform.localScale = Vector3.zero;
        LeanTween.scale(newFrame, Vector3.one, 1f).setEase(LeanTweenType.easeOutCirc);
        LeanTween.rotateZ(newFrame, 0, 2f).setEase(LeanTweenType.easeOutElastic);

        index++;
    }

    public void RemoveFrame(GameObject frame)
    {
        Destroy(frame);
        frames.Remove(frame);
    }

}
