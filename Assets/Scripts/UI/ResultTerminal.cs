using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultTerminal : MonoBehaviour
{
    [SerializeField]
    private LeanTweenType barTransitionType;
    [SerializeField]
    private LeanTweenType moneyTransitionType;
    [SerializeField]
    private float sliderTime = 1.5f;
    [SerializeField]
    private float moneyTime = 1.5f;
    [SerializeField]
    private Image govermentMatch;
    [SerializeField]
    private Text govermentMatchText;

    [SerializeField]
    private Text basePrice;
    [SerializeField]
    private Text customerTip;
    [SerializeField]
    private Text currentBalance;

    private int current = 0;

    private LTDescr govermentTween;
    private LTDescr desiredTween;
    private LTDescr priceTween;
    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }
    private void Update()
    {
        //currentBalance.text = GameManager.instance.money + "$";
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    ShowResult(.5f, .8f, 5, 20);
        //}
    }

    public void ShowResult(float _govermentPrecentage, float _desiredPrecentage, int _price, int _tip)
    {
        Reset();

        //play terminal sound
        AudioManager.instance?.Play3DSound(AudioEffect.computerTerminal, 1, transform.position);

        //goverment slider
        govermentTween = LeanTween.value(gameObject, 0, _govermentPrecentage, sliderTime).setOnUpdate((float val) =>
        {
            UpdateSlider(govermentMatch, govermentMatchText, val);
        });
        govermentTween.setEase(barTransitionType);
        govermentTween.setOnComplete(() =>
        {
            basePrice.text = "$" + _price;
        });

        //desired slider
        //desiredTween = LeanTween.value(gameObject, 0, _desiredPrecentage, sliderTime).setOnUpdate((float val) =>
        //{
        //    UpdateSlider(desiredMatch, desiredMatchText, val);
        //});
        //desiredTween.setOnComplete(() =>
        //{
        //    customerTip.text = "$" + _tip;
        //});
        //desiredTween.setEase(barTransitionType);
        //desiredTween.delay = sliderTime;
        //desired slider


        priceTween = LeanTween.value(gameObject, 0, 1, moneyTime).setOnUpdate((float val) =>
        {
            currentBalance.text = "$" + Mathf.Round(current + (val * (_price + _tip)));
            basePrice.text = "$" + Mathf.Round((1 - val) * _price);
            customerTip.text = "$" + Mathf.Round((1 - val) * _tip);
        });
        priceTween.setOnComplete(() =>
        {
            current += _price + _tip;
        });
        priceTween.setEase(moneyTransitionType);
        priceTween.delay = sliderTime * 2.5f;


    }

    public void Reset()
    {
        currentBalance.text = "$" + current;
        basePrice.text = "$0";
        customerTip.text = "$0";
        UpdateSlider(govermentMatch, govermentMatchText, 0);
        //UpdateSlider(desiredMatch, desiredMatchText, 0);
    }

    private void UpdateSlider (Image slider, Text precentageText, float val)
    {
        float _NewVal = 0;
        if (val < 0.7f)
        {
            _NewVal = Random.Range(0.0f, 0.2f);
        }
        else
        {
            _NewVal = val;
        }
        slider.fillAmount = _NewVal;
        precentageText.text = Mathf.Round(_NewVal * 100) + "%";
    }
}
