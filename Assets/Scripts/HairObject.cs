using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairObject : MonoBehaviour
{
    private bool grabbed = false;
    private bool hover = false;
    public List<Collision> collissions;
    private Transform parentTransform;

    private Vector3 oldPos;
    private Vector3 deltaPos;
    private float throwValue = 2;
    private float maxThrow = 10;

    private MeshRenderer mr;
    public Material idleMaterial;
    private Material highLightedMaterial;

    public bool Grabbed
    {
        get { return grabbed; }
        set {
            grabbed = value;
        }
    }
    public bool Hover
    {
        get
        {
            return hover;
        }
        set
        {
            Debug.Log("hover: " + value);
            hover = value;
            if (mr != null)
            {
                Color color = mr.material.color;
                mr.material = value ? highLightedMaterial : idleMaterial;
                mr.material.color = color;
            }
            //if (transform.childCount > 1)
            //{
            //    if (transform.GetChild(1).GetComponent<HairObject>())
            //    {
            //        transform.GetChild(1).GetComponent<HairObject>().Hover = value;
            //    }
            //}
        }
    }
    public bool AttachedAtHead
    {
        get { return collissions.Count > 0; }
    }
    public Transform ParentTransform
    {
        get { return parentTransform; }
    }
    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            idleMaterial = mr.material;
        }

        highLightedMaterial = Resources.Load("Outline", typeof(Material)) as Material;
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
            rb.velocity = deltaPos * (Mathf.Min(maxThrow, throwValue) * 1000);
            rb.isKinematic = false;
            Grabbed = value;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //check head
        if (collision.transform.tag == Tags.HEAD)
        {
            parentTransform = collision.transform;
            collissions.Add(collision);
        } else //check hair children of head
        {
            Transform parent = collision.transform;
            while (parent != null)
            {
                if (parent.transform.tag == Tags.HEAD)
                {
                    parentTransform = collision.transform;
                    //add hair children
                    collissions.Add(collision);
                    return;
                }
                parent = parent.parent;

            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collissions.Contains(collision))
        {
            collissions.Remove(collision);
        }
    }
}
