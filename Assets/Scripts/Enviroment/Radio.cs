using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Radio : MonoBehaviour
{

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] clips;

    private int index = 0;
    //private List<AudioClip> clipList;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Shuffle();
        PlayClip();

        //clipList = new List<AudioClip>();
        //for (int i = 0; i < clips.Length; i++)
        //{
        //    clipList.Add(clips[i]);
        //}

        //shuffle
    }

    public void PlayClip()
    {
        audioSource.clip = clips[index];
        audioSource.Play();
    }


    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            index = (index + 1) % clips.Length;
            PlayClip();
        }
    }

    public void Shuffle()
    {
        for (int i = 0; i < clips.Length; i++)
        {
            int rnd = Mathf.FloorToInt(UnityEngine.Random.Range(0, clips.Length));
            AudioClip tempGO = clips[rnd];
            clips[rnd] = clips[i];
            clips[i] = tempGO;
        }
    }
}
