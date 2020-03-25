using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private float maxAngle = 120;

    [SerializeField]
    private float openTime = 1f;
    [SerializeField]
    private float closeTime = 1f;
    private LTDescr openTween;
    private LTDescr closeTween;

    private float startAngle;
    private bool animating = false;

    public void Start()
    {
        startAngle = transform.rotation.eulerAngles.y;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Open(true);
        }
        if (GameManager.instance?.currentCustomer != null)
        {
            float distance = GameManager.instance.currentCustomer.transform.position.z - transform.position.z;
            if (Mathf.Abs(distance) < 1f)
            {
                if (animating)
                {
                    return;
                }
                Open(distance < 0);
            }
        }

    }
    public void Open (bool inWards)
    {
        if (animating) { return; }
        animating = true;

        //play doorbell sound
        AudioManager.instance?.Play3DSound(AudioEffect.doorbell, 1, transform.position);

        openTween = LeanTween.rotateY(gameObject, startAngle +  maxAngle * (inWards ? -1 : 1), openTime);
        openTween.setEase(LeanTweenType.easeOutCirc);
        openTween.setOnComplete(() => { Close(); });
    }


    private void Close()
    {

        closeTween = LeanTween.rotateY(gameObject, startAngle, closeTime);
        closeTween.setEase(LeanTweenType.easeOutElastic);
        closeTween.setOnComplete(() =>
        {
            animating = false;
        });
    }
}
