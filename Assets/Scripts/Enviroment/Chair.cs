using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    [SerializeField]
    private float endAngle = 180f;

    [SerializeField]
    private float duration = 2f;

    [HideInInspector]
    public Customer customer;

    public IEnumerator Spinning(bool facesMirror)
    {
        //play talking sound
        AudioManager.instance?.Play3DSound(AudioEffect.chair, 1, transform.position);

        if (customer != null)
        {
            customer.transform.parent = transform;
        }
        float startTime = Time.time;
        while (Mathf.Abs( transform.localRotation.eulerAngles.y -(facesMirror ? 0 : endAngle)) > 0.1f)
        {
            float t = (Time.time - startTime) / duration;
            Vector3 euler = transform.localRotation.eulerAngles;
            euler.y = Mathf.SmoothStep(euler.y, facesMirror ? 0 : endAngle, t);
            transform.localRotation = Quaternion.Euler(euler);
            yield return new WaitForFixedUpdate();
        }
        if (customer != null)
        {
            customer.transform.parent = null;
        }
    }


}
