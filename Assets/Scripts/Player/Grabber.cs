using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Grabber : MonoBehaviour {

    [SerializeField] private Grabber otherHand;
    [SerializeField] float GrabberRange = .1f;
    [SerializeField] private bool grabbed = false;
    public GameObject grabbedObject;
    private GameObject scalingObject;
    private float scaleMultip;
    private float startDistance;

    private GameObject hoverObject;

    [SerializeField] private float scaleMin = 0.01f;
    [SerializeField] private float scaleMax = .5f;

    public void Grab() {
        if (Grabbed) { return;  }

        //check object with tag
        GameObject focusedObject = SphereCastedObject(Tags.GRABABLE, transform);
        if (focusedObject == null) { scaleCheck(); return; }

        //check if spawnerobject
        if (focusedObject.GetComponent<spawner>()) focusedObject = focusedObject.GetComponent<spawner>().spawnObject();

        //apply variavble and parent
        grabbedObject = focusedObject;
        Grabbed = true;
        grabbedObject.transform.parent = transform;

        //add haircomponent
        if (grabbedObject.GetComponent<HairObject>() == null) {
            grabbedObject.AddComponent<HairObject>();
        }

        //lock rigidbody but still exist for collission detection with head and hair
        grabbedObject.GetComponent<HairObject>().ToggleRigidBody(true, true);

        grabbedObject.GetComponent<HairObject>().Grabbed = true;
        grabbedObject.GetComponent<HairObject>().Hover = true;
    }

    void scaleCheck() {
        if (otherHand.grabbedObject != null) {
            scalingObject = otherHand.grabbedObject;
            scaleMultip = scalingObject.transform.localScale.x;
            startDistance = Vector3.Distance(transform.position, otherHand.transform.position);
            return;
        }
    }

    public void Release() {
        if (scalingObject != null) {
            scalingObject = null;
            return;
        }
        if (!Grabbed) { return; }
        Grabbed = false;
        
        //no other controller grabs it, so you can do things, else ignore.
        if (grabbedObject.transform.parent == transform)  {
            grabbedObject.transform.parent = null;
            
            //if collides with head, attach it, otherwise fall on ground
            if (grabbedObject.GetComponent<HairObject>().AttachedAtHead) {
                grabbedObject.GetComponent<HairObject>().Grabbed = false;
                grabbedObject.GetComponent<HairObject>().ToggleRigidBody(false);

                grabbedObject.transform.parent = grabbedObject.GetComponent<HairObject>().ParentTransform;
            } 
            else {
                //set gravity on
                grabbedObject.GetComponent<HairObject>().ToggleRigidBody(true);
            }
        }
    }

    public GameObject SphereCastedObject(string _tag, Transform _transform) {
        Collider[] hitColliders = Physics.OverlapSphere(_transform.position, GrabberRange);
        foreach (Collider collider in hitColliders)
        {
            if (collider.tag == _tag)
            {
                return collider.gameObject;
            }
        }
        return null;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G)) Grab();
        if (Input.GetKeyDown(KeyCode.R)) Release();
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * 0.3f);


        if (scalingObject != null ) {
            if (scalingObject.GetComponent<HairObject>().Grabbed)
            {
                float scale = (Vector3.Distance(transform.position, otherHand.transform.position) / startDistance) * scaleMultip;
                scale = Mathf.Clamp(scale, scaleMin, scaleMax);
                scalingObject.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        if (!grabbed)
        {
            GameObject sphereCastedObject = SphereCastedObject(Tags.GRABABLE, transform);
            if (hoverObject != sphereCastedObject)
            {
                if (hoverObject != null)
                {
                    if (hoverObject.GetComponent<HairObject>())
                    {
                        hoverObject.GetComponent<HairObject>().Hover = false;
                    }
                }

                hoverObject = sphereCastedObject;

                if (hoverObject) {
                    if (!hoverObject.GetComponent<HairObject>())
                    {
                        hoverObject.AddComponent<HairObject>();
                    }
                    hoverObject.GetComponent<HairObject>().Hover = true;
                }
            } 
        }

    }

    public bool Grabbed {
        get { return grabbed; }
        set {
            grabbed = value;
        }
    }

    // void OnDrawGizmos() {
    //     Gizmos.DrawSphere(transform.position, GrabberRange);
    // }
}