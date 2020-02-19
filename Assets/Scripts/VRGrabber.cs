using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class VRGrabber : MonoBehaviour
{
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
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void Grab()
    {
        if (Grabbed) { return;  }

        //search for the collider
        GameObject focusedObject = SphereCastedObject();
        if (focusedObject == null)
        {
            focusedObject = RayCastedObject();
        }

        if (focusedObject == null) { return; }
        
        grabbedObject = focusedObject;
        Grabbed = true;

        grabbedObject.transform.parent = transform;
        StartCoroutine(MoveToGrabber());
        //grabbedObject.transform.position = transform.position;
    }
    IEnumerator MoveToGrabber()
    {
        while (Vector3.Distance(grabbedObject.transform.position, transform.position) > holdDistance && Grabbed)
        {
            grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, transform.position, Time.deltaTime * moveToHandSpeed);
            yield return new WaitForFixedUpdate();
        }
    }

    public void Release()
    {
        if (!Grabbed) { return; }

        Grabbed = false;
        grabbedObject.transform.parent = null;
    }

    private List<Collision> colliders = new List<Collision>();
    public List<Collision> GetColliders() { return colliders; }

    private void OnCollisionEnter(Collision other)
    {
        if (!colliders.Contains(other)) { colliders.Add(other); }
    }
    private void OnCollisionExit(Collision other)
    {
        colliders.Remove(other);
    }


    public GameObject RayCastedObject()
    {
        RaycastHit objectHitHover;
        if (Physics.Raycast(transform.position, transform.forward, out objectHitHover, grabDistance))
        {
            grabPointIndicator.SetActive(true);
            grabPointIndicator.transform.position = objectHitHover.point;
            lr.enabled = true;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, grabPointIndicator.transform.position);
            if (objectHitHover.collider.tag == Tags.GRABABLE)
            {
                grabPointIndicator.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0);
                return objectHitHover.collider.gameObject;
            }
            else
            {
                grabPointIndicator.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
            }
        }
        else
        {
            grabPointIndicator.SetActive(false);
            lr.enabled = false;
        }
        return null;
    }
    public GameObject SphereCastedObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, closeInfo);

        //i dont know if this works...
        hitColliders.OrderBy(a => Vector3.Distance(transform.position, a.transform.position));

        foreach (Collider col in hitColliders)
        {
            if (col.tag == Tags.GRABABLE && col.bounds.Contains(transform.position))
            {
                return col.gameObject;
            }
        }
        
        return null;
    }

    public bool Grabbed
    {
        get { return grabbed; }
        set {
            grabbed = value;
            lr.enabled = !value;
            grabPointIndicator.SetActive(!value);

        }
    }
    void Update()
    {
        if (!grabbed)
        {
            if (SphereCastedObject() == null)
            {
                RayCastedObject();
            } else
            {
                lr.enabled = false;
                grabPointIndicator.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Grab");
            Grab();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Release();
        }
    }


    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, .2f);
    }
}
