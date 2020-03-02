﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 0.1f;

    [SerializeField] private Sprite desiredHead;

    [SerializeField] private GameObject head;


    [Header("UI info")]
    [SerializeField] private Transform canvasPivot;
    [SerializeField] private Transform canvasContent;

    [SerializeField]
    private Image desiredHeadImage;


    public GameObject Head {
        get { return head; }
    }
    public Sprite DesiredHead {
        get { return desiredHead; }
        set {
            desiredHead = value;
            desiredHeadImage.sprite = value;
        }
    }

    private void Update() {
        canvasPivot.LookAt(2* canvasPivot.position - Camera.main.transform.position);
        // Turn the canvas so the  tekstballon looks at the player
        float s = Vector3.Distance(Camera.main.transform.position,canvasContent.position);
        float a = Vector3.Distance(Camera.main.transform.position, canvasPivot.position);
        float angle = Mathf.Acos(a/s) * Mathf.Rad2Deg;
        canvasPivot.eulerAngles = new Vector3(canvasPivot.eulerAngles.x,canvasPivot.eulerAngles.y + angle,canvasPivot.eulerAngles.z);
    }

    public void Walk(Vector3 destination, bool destroyWhenReached = false) {
        StopAllCoroutines();
        StartCoroutine(Walking(destination, destroyWhenReached));
    }
    IEnumerator Walking(Vector3 destination, bool destroyWhenReached = false) {
        while (Vector3.Distance(transform.position, destination) > 0.1f) {
            transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * walkSpeed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = destination;

        if (destroyWhenReached) Destroy(this.gameObject);
    }
}