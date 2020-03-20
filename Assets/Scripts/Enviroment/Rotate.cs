using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed = .5f;
    // Start is called before the first frame update

    [SerializeField]
    private float waveAmplitude = .1f;

    [SerializeField]
    private float waveFrequency = .5f;

    private float startY;
    void Start()
    {
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed, 0));

        Vector3 wavePos = transform.position;

        wavePos.y = startY + waveAmplitude * Mathf.Sin(Time.time * waveFrequency);
        transform.position = wavePos;
    }
}
