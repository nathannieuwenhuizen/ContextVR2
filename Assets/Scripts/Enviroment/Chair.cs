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
    [SerializeField]
    private LeanTweenType spinAnimationType = LeanTweenType.easeInOutBack;

    public IEnumerator Spinning(bool facesMirror)
    {
        //play talking sound
        AudioManager.instance?.Play3DSound(AudioEffect.chair, 1, transform.position);

        if (customer != null)
        {
            customer.transform.parent = transform;
        }
        LeanTween.rotate(gameObject, new Vector3(0, facesMirror ? 0 : endAngle, 0), duration).setEase(spinAnimationType);

        yield return new WaitForSeconds(duration);
        if (customer != null)
        {
            customer.transform.parent = null;
        }
    }


}
