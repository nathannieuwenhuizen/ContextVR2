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

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        NextCustomerWalksIn();
    }
    public void NextCustomerWalksIn()
    {
        currentCustomer = Instantiate(customerPrefab, doorPos).GetComponent<Customer>();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HairCutFinished();
        }
    }

    public void HairCutFinished()
    {
        formChecker.CompareMeshes(currentCustomer.Head, currentCustomer.DesiredHead);
        currentCustomer.Walk(doorPos.position, true);
        NextCustomerWalksIn();
    }

}
