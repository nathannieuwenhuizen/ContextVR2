using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private MeshFormChecker formChecker;

    //customer info
    private Customer currentCustomer;
    [SerializeField]
    private GameObject customerPrefab;

    [SerializeField]
    private Text precentageUI;

    [SerializeField]
    private ImageGallery gallery;

    

    [Header("customerPositions")]
    [SerializeField]
    private Transform doorPos;
    [SerializeField]
    private Transform chairPos;

    [Header("hair info")]
    [SerializeField]
    private Sprite govermentHair;
    [SerializeField]
    private Image govermentImage;
    [SerializeField]
    private Sprite[] customerHairQueue;
    private int customerCount = 0;

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        govermentImage.sprite = govermentHair;
        NextCustomerWalksIn();
    }
    public void NextCustomerWalksIn()
    {
        currentCustomer = Instantiate(customerPrefab, doorPos).GetComponent<Customer>();
        currentCustomer.DesiredHead = customerHairQueue[customerCount % customerHairQueue.Length];
        currentCustomer.Walk(chairPos.position);

    }

    public void OnHairCutCheck()
    {
        Settings.TotalPrecentage += formChecker.precentageCorrect;
        Settings.AmountOfCustomers++;
        if (gallery != null)
        {
            gallery.AddFrame(formChecker.selectedTexture);
        }
        precentageUI.text = Mathf.Round(formChecker.precentageCorrect * 100) + "%";
    }

    private void Update()
    {

        //just for testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HairCutFinished();
        }
    }

    public void HairCutFinished()
    {
        customerCount++;
        formChecker.CompareMeshes(currentCustomer.Head, currentCustomer.DesiredHead);
        currentCustomer.Walk(doorPos.position, true);
        NextCustomerWalksIn();
    }

}
