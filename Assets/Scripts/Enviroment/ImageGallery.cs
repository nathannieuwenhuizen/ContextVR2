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

    void Start()
    {
        //LoadGalleryFromFiles("/../SaveImages/");
        frames = new List<GameObject>();
    }

    public void LoadGalleryFromFiles(string basePath)
    {
        for (int i = 0; i < maxFrames; i++)
        {

            Texture2D newTexture = Data.LoadPNG(basePath + i + ".png");
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

    public void AddFrame(Texture2D texture)
    {
        GameObject newFrame = Instantiate(framePrefab, transform);
        newFrame.transform.GetChild(1).GetComponent<MeshRenderer>().material.mainTexture = texture;


        if (frames.Count > maxFrames)
        {
            frames.Insert(0, newFrame);
            RemoveFrame(frames[frames.Count - 1]);

        } else
        {
            frames.Add(newFrame);
        }
        UpdatePos();
    }

    public void RemoveFrame(GameObject frame)
    {
        Destroy(frame);
        frames.Remove(frame);
    }

    public void UpdatePos()
    {
        for(int i = 0; i < frames.Count; i++)
        {
            frames[i].name = "frame #" + frames.IndexOf(frames[i]);
            frames[i].transform.localPosition = new Vector3(i * framesPadding, 0, 0);
        }
    }

}
