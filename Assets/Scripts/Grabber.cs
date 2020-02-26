using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Grabber : MonoBehaviour {

    [SerializeField] private Grabber otherHand;
    [SerializeField] private bool grabbed = false;
    public GameObject grabbedObject;
    private GameObject scalingObject;
    private float scaleMultip;
    private float startDistance;

    [SerializeField] private float scaleMin = 0.01f;
    [SerializeField] private float scaleMax = .5f;

    public void Grab() {
        if (Grabbed) { return;  }

        GameObject focusedObject = SphereCastedObject(Tags.GRABABLE, transform);
        if (focusedObject == null) { return; }

        if (focusedObject.GetComponent<spawner>()) focusedObject = focusedObject.GetComponent<spawner>().spawnObject();

        if (focusedObject.transform.parent == otherHand.transform) {
            scalingObject = focusedObject;
            scaleMultip = scalingObject.transform.localScale.x;
            startDistance = Vector3.Distance(transform.position, otherHand.transform.position);
            return;
        }

        grabbedObject = focusedObject;
        Grabbed = true;
        grabbedObject.transform.parent = transform;

        if (grabbedObject.GetComponent<HairObject>() == null) {
            grabbedObject.AddComponent<HairObject>();
        }
        grabbedObject.GetComponent<HairObject>().Lock(true);
    }

    public void Release() {
        if (scalingObject != null) {
            scalingObject = null;
            return;
        }
        if (!Grabbed) { return; }
        Grabbed = false;
        
        Rigidbody grabbedRB = grabbedObject.GetComponent<Rigidbody>();

        //no other controller grabs it, so you can do things, else ignore.
        if (grabbedObject.transform.parent == transform)  {
            grabbedObject.transform.parent = null;
            
            if (grabbedObject.GetComponent<HairObject>().AttachedAtHead) {
                grabbedObject.transform.parent = grabbedObject.GetComponent<HairObject>().ParentTransform;
            } 
            else {
                //set gravity on
                if (grabbedRB != null) {
                    grabbedObject.GetComponent<HairObject>().Lock(false);

                }
            }
        }
    }

    // public void ScaleObject(float val) {
    //     if (grabbedObject == null){ return; }

    //     Vector3 tempScale = grabbedObject.transform.localScale + Vector3.one * scaleSpeed * val;

    //     if (tempScale.x < scaleMin) tempScale = new Vector3(scaleMin, scaleMin, scaleMin);
    //     if (tempScale.x > scaleMax) tempScale = new Vector3(scaleMax, scaleMax, scaleMax);

    //     grabbedObject.transform.localScale = tempScale;
    // }

    public GameObject SphereCastedObject(string _tag, Transform _transform) {

        
        Collider[] hitColliders;
        if (_transform.GetComponent<BoxCollider>())
        {
            hitColliders = Physics.OverlapBox(_transform.position, _transform.GetComponent<BoxCollider>().size, _transform.rotation);
        }
        else
        {
            hitColliders = Physics.OverlapSphere(_transform.position, 0);
        }


        //i dont know if this works...
        hitColliders.OrderBy(a => Vector3.Distance(_transform.position, a.transform.position));

        foreach (Collider col in hitColliders) {
            
            if (col.tag == _tag && col.bounds.Contains(_transform.position)) {
                return col.gameObject;
            }
        }
        return null;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G)) Grab();
        if (Input.GetKeyDown(KeyCode.R)) Release();

        if(scalingObject != null) {
            float scale = (Vector3.Distance(transform.position, otherHand.transform.position) / startDistance) * scaleMultip;
            scale = Mathf.Clamp(scale, scaleMin, scaleMax);
            scalingObject.transform.localScale = new Vector3(scale,scale,scale);
        }

        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * 0.3f);
    }

    public bool Grabbed {
        get { return grabbed; }
        set {
            grabbed = value;
        }
    }
}