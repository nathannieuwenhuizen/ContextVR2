using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 0.1f;

    [SerializeField]
    private GameObject desiredHead;

    [SerializeField]
    private GameObject head;


    [Header("UI info")]
    [SerializeField]
    private Transform canvasPivot;
    [SerializeField]
    private float rotationDamping;

    public GameObject Head
    {
        get { return head; }
    }
    public GameObject DesiredHead
    {
        get { return desiredHead; }
    }

    private void Update()
    {
        var lookPos = transform.position - Camera.main.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        canvasPivot.rotation = Quaternion.Slerp(canvasPivot.rotation, rotation, Time.deltaTime * rotationDamping);
    }

    public void Walk(Vector3 destination, bool destroyWhenReached = false)
    {
        StopAllCoroutines();
        StartCoroutine(Walking(destination, destroyWhenReached));
    }
    IEnumerator Walking(Vector3 destination, bool destroyWhenReached = false)
    {
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * walkSpeed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = destination;

        if (destroyWhenReached) Destroy(this.gameObject);
    }
}
