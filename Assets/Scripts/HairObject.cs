using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairObject : MonoBehaviour
{
    private bool grabbed = false;
    public List<Collision> collissions;
    private Transform head;

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
        collissions = new List<Collision>();
    }
    private void Update()
    {
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
