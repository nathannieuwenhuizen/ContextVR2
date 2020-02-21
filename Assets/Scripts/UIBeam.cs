using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class UIBeam : MonoBehaviour {
    [SerializeField]
    private bool grabbed = false;
    private GameObject grabbedObject;
    private LineRenderer lr;

    [Header("pull info")]
    [Range(1, 50)]
    [SerializeField]
    private float grabDistance = 50f;

    [Range(0.1f, 5f)]
    [SerializeField]
    private float holdDistance = 1f;

    [Range(0.1f, 10f)]
    [SerializeField]
    private float moveToHandSpeed = 2f;

    [SerializeField]
    private GameObject grabPointIndicator;

    [Header("close info")]
    [Range(0.01f, 1f)]
    [SerializeField]
    private float closeInfo;

    void Start() {
        lr = GetComponent<LineRenderer>();
    }

    public void Press() {
        if (Grabbed) { return;  }

        //search for the collider
        GameObject focusedObject = RayCastedObject();

        if (focusedObject == null) { return; }
        
        grabbedObject = focusedObject;
        Grabbed = true;
        StartCoroutine(MoveToGrabber());
    }

    public void Release() {
        if (!Grabbed) { return; }
        Grabbed = false;
    }

    IEnumerator MoveToGrabber() {
        while (Vector3.Distance(grabbedObject.transform.position, transform.position) > holdDistance && Grabbed) {
            grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, transform.position, Time.deltaTime * moveToHandSpeed);
            yield return new WaitForFixedUpdate();
        }
    }

    public GameObject RayCastedObject() {
        RaycastHit objectHitHover;
        if (Physics.Raycast(transform.position, transform.forward, out objectHitHover, grabDistance)) {
            grabPointIndicator.SetActive(true);
            grabPointIndicator.transform.position = objectHitHover.point;
            lr.enabled = true;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, grabPointIndicator.transform.position);
            if (objectHitHover.collider.tag == Tags.GRABABLE) {
                grabPointIndicator.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0);
                return objectHitHover.collider.gameObject;
            }
            else {
                grabPointIndicator.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
            }
        }
        else {
            grabPointIndicator.SetActive(false);
            lr.enabled = false;
        }
        return null;
    }

    public bool Grabbed {
        get { return grabbed; }
        set {
            grabbed = value;
            lr.enabled = !value;
            grabPointIndicator.SetActive(!value);
        }
    }

    void Update() {
        if (!grabbed) {
            if (null == null) {
                RayCastedObject();
            } else {
                lr.enabled = false;
                grabPointIndicator.SetActive(false);
            }
        }
    }
}