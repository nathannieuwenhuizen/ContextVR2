using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Valve.VR.InteractionSystem;

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

    private float oldSclale = 0;
    //Throw Physics Shit
    private Vector3 currentGrabbedLocation;

    private bool didHideDefaultController;
    //public GameObject capsule;
    //private Vector3 controllerCentreOfMass;
    //private Vector3 grabbedObjectCentreOfMass;
    //private Vector3 grabbedObjectPosOffset;
    //private Vector3 controllerVelocityCross;


    private void Start()
    {
        //Throw Physics Shit
        currentGrabbedLocation = new Vector3();
        //controllerCentreOfMass = capsule.GetComponent<Rigidbody>().centerOfMass;
    }

    public void Grab() {
        Debug.Log(" Grab hand!");

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

        //play grab sound
        AudioManager.instance?.Play3DSound(AudioEffect.grabProp, 1, transform.position);

        //make hand dissapear
        HideHands();

        //add haircomponent
        if (grabbedObject.GetComponent<HairObject>() == null) {
            grabbedObject.AddComponent<HairObject>();
        }

        //lock rigidbody but still exist for collission detection with head and hair
        grabbedObject.GetComponent<HairObject>().ToggleRigidBody(true, true);
        //grabbedObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        //grabbedObject.GetComponent<Rigidbody>().isKinematic = true;

        grabbedObject.GetComponent<HairObject>().Grabbed = true;
        grabbedObject.GetComponent<HairObject>().Hover = true;

        //update slider
        if (grabbedObject.GetComponent<ATM>() == null)
        {
            HSVColorPanel.instance.SelectedObject = grabbedObject;
        }

        //Throw Physics Shit
        //grabbedObjectCentreOfMass = grabbedObject.GetComponent<Rigidbody>().centerOfMass;
    }

    void scaleCheck() {

        //check if other hand also grabs it
        if (otherHand.grabbedObject != null) {
            scalingObject = otherHand.grabbedObject;
            scaleMultip = scalingObject.transform.localScale.x;
            startDistance = Vector3.Distance(transform.position, otherHand.transform.position);
            return;
        }
    }

    public void Release() {
        //Make hands reappear
        ShowHands();

        if (scalingObject != null) {
            scalingObject = null;
            return;
        }
        if (!Grabbed) { return; }
        Grabbed = false;
        
        //no other controller grabs it, so you can do things, else ignore.
        if (grabbedObject.transform.parent == transform)  {
            grabbedObject.transform.parent = null;

            //update color panel
            HSVColorPanel.instance.SelectedObject = null;

            //if collides with head, attach it, otherwise fall on ground
            if (grabbedObject.GetComponent<HairObject>().AttachedAtHead) {

                //play release at head sound
                AudioManager.instance?.Play3DSound(AudioEffect.releaseWithHead, 1, transform.position);

                //grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbedObject.GetComponent<HairObject>().Grabbed = false;
                grabbedObject.GetComponent<HairObject>().ToggleRigidBody(false);

                grabbedObject.transform.parent = grabbedObject.GetComponent<HairObject>().ParentTransform;
                //grabbedObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
            }
            else {
                //play release without head sound
                AudioManager.instance?.Play3DSound(AudioEffect.releaseWithoutHead, 1, transform.position);

                //Throw Physics Shit
                Vector3 throwVector = grabbedObject.transform.position - currentGrabbedLocation;
                //grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbedObject.GetComponent<HairObject>().ToggleRigidBody(true, false);
                grabbedObject.GetComponent<Rigidbody>().AddForce(throwVector * 75, ForceMode.Impulse);
                //grabbedObject.GetComponent<Rigidbody>().velocity = capsule.GetComponent<Rigidbody>().velocity + controllerVelocityCross;
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

                if (Math.Abs(oldSclale - scale) > 0.05f )
                {
                    //play release without head sound
                    float pitchValue = (scaleMax - scaleMin) / (scale - scaleMin);
                    AudioManager.instance?.Play3DSound(AudioEffect.scaleChange, 1, scalingObject.transform.position, false, .2f + pitchValue * 1.3f);
                    oldSclale = scale;
                }

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

        //Throw Physics Shit
        if (grabbed) currentGrabbedLocation = grabbedObject.transform.position;
        //if (grabbed)
        //{
        //    grabbedObjectPosOffset = grabbedObjectCentreOfMass - controllerCentreOfMass;
        //    controllerVelocityCross = Vector3.Cross(capsule.GetComponent<Rigidbody>().angularVelocity, grabbedObjectPosOffset);
        //}
    }

    public bool Grabbed {
        get { return grabbed; }
        set {
            grabbed = value;
        }
    }

    void HideHands()
    {
        this.transform.parent.GetChild(6).gameObject.SetActive(false);
    }

    void ShowHands()
    {
        this.transform.parent.GetChild(6).gameObject.SetActive(true);
    }

    // void OnDrawGizmos() {
    //     Gizmos.DrawSphere(transform.position, GrabberRange);
    // }
}