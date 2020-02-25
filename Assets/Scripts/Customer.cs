using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 0.1f;

    [SerializeField]
    private GameObject desiredHead;

    public GameObject Head
    {
        get { return transform.GetChild(1).gameObject; }
    }
    public GameObject DesiredHead
    {
        get { return desiredHead; }
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
