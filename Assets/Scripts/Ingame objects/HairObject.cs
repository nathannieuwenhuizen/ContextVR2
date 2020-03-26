using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairObject : MonoBehaviour
{
    public HairData hairData;

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
            if (value == false)
            {
                Hover = false;
            }
            collissions.Clear();
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
            //Debug.Log("hover: " + value);
            hover = value;
            if (mr != null)
            {
                Color color = mr.material.color;
                mr.material = value ? highLightedMaterial : idleMaterial;
                mr.material.color = color;
            }

            for (int i= 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<HairObject>())
                {
                    transform.GetChild(i).GetComponent<HairObject>().Hover = value;
                }
            }
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
        if (GetComponent<ATM>())
        {
            highLightedMaterial = Resources.Load("ATM hover", typeof(Material)) as Material;
        }
    }
    private void Start()
    {

        if (hairData == null)
        {
            hairData = new HairData();
            if (string.IsNullOrEmpty(hairData.id))
            {
                hairData.id = System.DateTime.Now.ToLongDateString() + System.DateTime.Now.ToLongDateString() + Random.Range(0, int.MaxValue).ToString();
            }

        }

        switch (GetComponent<MeshFilter>().mesh.name.Substring(0,4))
        {
            case "Cube":
                hairData.meshType = PrimitiveType.Cube;
                break;
            case "Sphe":
                hairData.meshType = PrimitiveType.Sphere;
                break;
            case "Cyli":
                hairData.meshType = PrimitiveType.Cylinder;
                break;
            default:
                hairData.meshType = PrimitiveType.Capsule;
                break;
        }
        if (gameObject.name.Contains("prop") || gameObject.name.Contains("atm") || gameObject.name.Contains("book") || gameObject.name.Contains("sandwitch") || gameObject.name.Contains("doughnut") )
        {
            Debug.Log(" it is a prop!");
            hairData.meshType = PrimitiveType.Capsule;
        }

        oldPos = transform.position;
        collissions = new List<Collision>();
    }
    private void Update()
    {

        if (grabbed)
        {
            deltaPos = (transform.position - oldPos) * Time.deltaTime;
            oldPos = transform.position;

            hairData.color = mr.material.color;
        }
    }
    public void ToggleRigidBody(bool value, bool hasConstaints = false)
    {
        if (!value)
        {
            if (GetComponent<Rigidbody>())
            {
                Destroy(GetComponent<Rigidbody>());
            }
        }
        else
        {
            
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.velocity = deltaPos * (Mathf.Min(maxThrow, throwValue) * 1000);
            //rb.isKinematic = hasConstaints ? true : false;
            rb.constraints = hasConstaints ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
            rb.isKinematic = false;            
            collissions = new List<Collision>();
            parentTransform = null;
        }
    }

    private void CheckCollissionToAttach(Collision collision)
    {
        if (collissions.Contains(collision)) { return; }
        //check head
        if (collision.transform.tag == Tags.HEAD)
        {
            parentTransform = collision.transform;
            collissions.Add(collision);
        }
        else //check hair children of head
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

    private void OnCollisionEnter(Collision collision)
    {
        if (grabbed)
        {
            CheckCollissionToAttach(collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (grabbed)
        {
            CheckCollissionToAttach(collision);
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
