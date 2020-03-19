using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    public float walkSpeed = 0.1f;
    public float rotateSpeed = 2f;

    public bool IsMoving  = false;

    public IEnumerator Orienting(Vector3 destination)
    {
        Vector3 newDirection;

        while (Mathf.Abs(Vector3.Angle(transform.forward, destination - transform.position)) > 1f)
        {
            newDirection = Vector3.RotateTowards(transform.forward, destination - transform.position, Time.deltaTime * rotateSpeed, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            yield return new WaitForFixedUpdate();
        }
        newDirection = Vector3.RotateTowards(transform.forward, destination - transform.position, 1f, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

    }
    public IEnumerator Walking(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.3f)
        {
            transform.Translate(transform.InverseTransformDirection(transform.forward) * walkSpeed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = destination;
    }

    public IEnumerator WalkLoop(Vector3[] positions, bool invert, int startIndex)
    {
        int index = startIndex;
        Vector3 offset = new Vector3(Random.value -.5f, 0, Random.value -.5f);
        IsMoving = true;

        StartCoroutine(Orienting(positions[index] + offset * 2f));
        IsMoving = true;

        yield return StartCoroutine(Walking(positions[index] + offset * 2f));

        //yield return StartCoroutine(GoTo(positions[index] + offset * 2f));
        IsMoving = true;
        StopAllCoroutines();
        index += invert ? -1 : 1;
        index = (index + positions.Length) % positions.Length;
        StartCoroutine(WalkLoop(positions, invert, index));

    }

    public IEnumerator GoTo(Vector3 pos, bool destoryWhenFinish = false)
    {
        if (!IsMoving)
        {
            IsMoving = true;
            yield return StartCoroutine(Orienting(pos));
            yield return StartCoroutine(Walking(pos));
            IsMoving = false;
        }
        if (destoryWhenFinish)
        {
            Destroy(this.gameObject);
        }

    }
}
