using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class UIBeam : MonoBehaviour {
    [HideInInspector] public bool drawline = false;
    private bool held;
    private LineRenderer lr;
    [SerializeField] private float LRLength = 10;
    [SerializeField] private GameObject grabPointIndicator;
    [SerializeField] public VRInputModule eventSystem;

    [HideInInspector] public Camera cam;

    void Start() {
        eventSystem.currentCamera = cam = GetComponent<Camera>();
        lr = GetComponent<LineRenderer>();
    }

    private void Update() {
        if (held || drawline) drawLine();
        if (lr != null) {
            lr.enabled = ((eventSystem.currentCamera == cam && held) || (eventSystem.currentCamera == cam && drawline)) ? true : false;
        }

        if (!held && eventSystem.currentCamera == cam)
        {
            drawLine(true);
            //lr.enabled = hover;
            //grabPointIndicator.SetActive(hover);
        }
    }

    public void Press() {
        if (held) { return; }

        held = true;
        grabPointIndicator.SetActive(true);
        eventSystem.setCam(cam);
        eventSystem.ProcessPress();
    }

    public void Release() {
        if (!held) { return; }

        held = false;
        grabPointIndicator.SetActive(false);
        eventSystem.ProcessRelease();
    }

    private RaycastHit CreateRaycast(float length) {
        RaycastHit hit;
        Ray ray = new Ray(transform.position,transform.forward);
        Physics.Raycast(ray,out hit, length);
        return hit;
    }

    private void drawLine(bool hover = false) {
        PointerEventData data = eventSystem.getData();
        float targetLength = data.pointerCurrentRaycast.distance == 0 ? LRLength : data.pointerCurrentRaycast.distance;

        RaycastHit hit = CreateRaycast(targetLength);

        Vector3 endPos = transform.position + transform.forward * targetLength;

        if (hit.collider != null)
        {
            endPos = hit.point;
            if (hover)
            {
                lr.enabled = false;
                grabPointIndicator.SetActive(false);
            }
        } else
        {
            if (hover) {
                lr.enabled = true;
                grabPointIndicator.SetActive(true);
            }
        }
        grabPointIndicator.transform.position = endPos;

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, endPos);
    }
}