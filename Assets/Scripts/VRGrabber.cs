using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRGrabber : MonoBehaviour
{
    private bool grabbed = false;
    private GameObject grabbedObject;
    private LineRenderer lr;

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

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void Grab()
    {
        if (Grabbed) { return;  }

        GameObject focusedObject = RayCastedObject();
        if (focusedObject == null) { return; }

        Grabbed = true;
        grabbedObject = focusedObject;

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
            if (objectHitHover.collider.tag == "Grabable")
            {
                grabPointIndicator.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0);
                return objectHitHover.collider.gameObject;
            } else
            {
                grabPointIndicator.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
            }
        } else
        {
            grabPointIndicator.SetActive(false);
            lr.enabled = false;
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
            RayCastedObject();
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
