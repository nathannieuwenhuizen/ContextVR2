using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Grabber : MonoBehaviour {

    [SerializeField] private bool grabbed = false;
    private GameObject grabbedObject;

    [Header("close info")] [Range(0.01f, 1f)]
    [SerializeField] private float closeInfo;

    [Range(0f, .1f)]
    [SerializeField] private float scaleSpeed = 0.1f;

    public void Grab() {
        if (Grabbed) { return;  }

        //search for the collider
        GameObject focusedObject = SphereCastedObject();
        if (focusedObject == null) { return; }
        
        grabbedObject = focusedObject;
        Grabbed = true;
        grabbedObject.transform.parent = transform;

        Rigidbody grabbedRB = grabbedObject.GetComponent<Rigidbody>();
        if (grabbedRB != null) grabbedRB.isKinematic = true;
    }

    public void Release() {
        if (!Grabbed) { return; }

        Rigidbody grabbedRB = grabbedObject.GetComponent<Rigidbody>();
        if (grabbedRB != null && grabbedObject.transform.parent == transform) grabbedRB.isKinematic = false;

        Grabbed = false;
        if (grabbedObject.transform.parent == transform) grabbedObject.transform.parent = null;
    }

    public void ScaleObject(float val) {
        if (grabbedObject == null){ return; }

        Vector3 tempScale = grabbedObject.transform.localScale;

        tempScale.x = (Mathf.Max(0.1f, tempScale.x + scaleSpeed * val));
        tempScale.y = (Mathf.Max(0.1f, tempScale.y + scaleSpeed * val));
        tempScale.z = (Mathf.Max(0.1f, tempScale.z + scaleSpeed * val));

        grabbedObject.transform.localScale = tempScale;
    }

    public GameObject SphereCastedObject() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, closeInfo);

        //i dont know if this works...
        hitColliders.OrderBy(a => Vector3.Distance(transform.position, a.transform.position));

        foreach (Collider col in hitColliders) {
            if (col.tag == Tags.GRABABLE && col.bounds.Contains(transform.position)) {
                return col.gameObject;
            }
        }
        
        return null;
    }

    public bool Grabbed {
        get { return grabbed; }
        set {
            grabbed = value;
        }
    }
}
