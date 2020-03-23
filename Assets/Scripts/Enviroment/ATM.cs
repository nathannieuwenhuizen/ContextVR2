using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATM : MonoBehaviour
{

    private Vector3 startPos;
    private Vector3 scale;
    private Quaternion quaternion;
    [SerializeField]
    private bool tooFar = false;
    private float tooFarDistance = 5f;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        scale = transform.localScale;
        quaternion = transform.rotation;
    }
    public void ResetPosition()
    {
        tooFar = false;
        transform.parent = null;
        transform.position = startPos;
        transform.localScale = scale;
        transform.rotation = quaternion;
        if (GetComponent<HairObject>())
        {
            GetComponent<HairObject>().ToggleRigidBody(true);
            GetComponent<HairObject>().Grabbed = false;
        }
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        //check head
        if (collision.transform.tag == Tags.HEAD)
        {
            GameManager.instance?.HairCutFinished();
        }
        else //check hair children of head
        {
            Transform parent = collision.transform;
            while (parent != null)
            {
                if (parent.transform.tag == Tags.HEAD)
                {
                    GameManager.instance?.HairCutFinished();
                    return;
                }
                parent = parent.parent;


            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!tooFar)
        {
            if (GetComponent<HairObject>())
            {
                Debug.Log(" dist: " + Vector3.Distance(transform.position, startPos));
                if (Vector3.Distance(transform.position, startPos) > tooFarDistance)
                {
                    tooFar = true;
                    StartCoroutine(Resetting());
                }
            }
        }
    }
    IEnumerator Resetting()
    {
        yield return new WaitForSeconds(5f);
        if (Vector3.Distance(transform.position, startPos) > tooFarDistance)
        {
            ResetPosition();
        }
    }
}
