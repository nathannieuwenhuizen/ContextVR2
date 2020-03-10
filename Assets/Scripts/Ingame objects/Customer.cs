﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 0.1f;

    [SerializeField] private Sprite desiredHead;

    [SerializeField] private GameObject head;

    [Header("data")]
    [SerializeField]
    public CustomerData customerData;
    [SerializeField] private string dataPath;

    [Header("UI info")]
    [SerializeField] private DialogueHandeler dialogueHandeler;
    [SerializeField] private Transform canvasPivot;
    [SerializeField] private Transform canvasContent;

    [SerializeField]
    private Image desiredHeadImage;

    public GameObject Head {
        get { return head; }
    }
    public Sprite DesiredHead {
        get { return desiredHead; }
        set {
            desiredHead = value;
            desiredHeadImage.sprite = value;
        }
    }
    public bool IsWalking { get; private set; } = false;

    private void Update() {
        AimCanvasToCamera();
    }
    public void Start()
    {
        //customerData = JsonUtility.FromJson<CustomerData>(Data.LoadJSONFileAsText(dataPath));
    }
    public CustomerData CustomerData
    {
        get { return customerData; }
        set {
            customerData = value;
            if (customerData.dialogues.Length > 0)
            {
                dialogueHandeler.UpdateUI(customerData.dialogues[0], customerData.name);
                if (customerData.desiredHaircutID < 0)
                {
                    DesiredHead = GameManager.instance.govermentHair;
                } else
                {
                    DesiredHead = GameManager.instance.customerHaircuts[customerData.desiredHaircutID];
                }
            }
        }
    }

    // Turn the canvas so the  tekstballon looks at the player
    public void AimCanvasToCamera()
    {
        canvasPivot.LookAt(2 * canvasPivot.position - Camera.main.transform.position);

        float s = Vector3.Distance(Camera.main.transform.position, canvasContent.position);
        float a = Vector3.Distance(Camera.main.transform.position, canvasPivot.position);
        float angle = Mathf.Acos(a / s) * Mathf.Rad2Deg;
        canvasPivot.eulerAngles = new Vector3(canvasPivot.eulerAngles.x, canvasPivot.eulerAngles.y + angle, canvasPivot.eulerAngles.z);
    }

    /// <summary>
    /// Makes the customer walks to the postion
    /// </summary>
    /// <param name="destination"> Th</param>
    /// <param name="destroyWhenReached"></param>
    public void Walk(Vector3 destination, bool destroyWhenReached = false) {
        StopAllCoroutines();
        StartCoroutine(Walking(destination, destroyWhenReached));
    }
    IEnumerator Walking(Vector3 destination, bool destroyWhenReached = false) {
        IsWalking = true;
        while (Vector3.Distance(transform.position, destination) > 0.1f) {
            transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * walkSpeed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = destination;
        IsWalking = false;
        if (destroyWhenReached) Destroy(this.gameObject);
    }
}

[System.Serializable]
public class CustomerData
{
    public string name = "name";
    public int desiredHaircutID = -1;
    public int appearanceSeed = 0;
    public Dialogue[] dialogues;
}
[System.Serializable]
public class Dialogue
{
    public string text = "...";
    public Response[] responses;
}

[System.Serializable]
public class Response
{
    public string text;
    public int dialogueID;
}