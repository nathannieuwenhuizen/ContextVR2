using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairObject : MonoBehaviour
{
    private bool grabbed = false;
    public List<Collision> collissions;
    private Transform head;

    private Vector3 oldPos;
    private Vector3 deltaPos;
    private float throwValue = 1;
    public bool Grabbed
    {
        get { return grabbed; }
        set { grabbed = value; }
    }
    public bool AttachedAtHead
    {
        get { return collissions.Count > 0; }
    }
    public Transform Head
    {
        get { return head; }
    }
    private void Start()
    {
        oldPos = transform.position;
        collissions = new List<Collision>();
    }
    private void Update()
    {
        if (grabbed)
        {
            deltaPos = (transform.position - oldPos) * Time.deltaTime;
            oldPos = transform.position;
        }
        Debug.Log("Collissions length: " + collissions.Count);
    }
    public void Lock(bool value)
    {
        if (GetComponent<Rigidbody>())
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (value)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            } else
            {
                rb.constraints = RigidbodyConstraints.None;
            }
            rb.velocity = deltaPos * throwValue;
            rb.isKinematic = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collissions enter");
        //check head
        if (collision.transform.tag == Tags.HEAD)
        {
            head = collision.transform;
            collissions.Add(collision);
        } else //check hair children of head
        {
            Transform parent = collision.transform.parent;

            if (parent != null)
            {
                if (parent.transform.tag == Tags.HEAD)
                {
                    head = parent;
                    //add hair children
                    collissions.Add(collision);

                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Collissions exit");
        if (collissions.Contains(collision))
        {
            collissions.Remove(collision);
        }
    }
}
