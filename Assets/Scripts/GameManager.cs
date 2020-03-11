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

    [Header("Enviroment")]
    [SerializeField]
    public Chair chair;
    [SerializeField]
    private ImageGallery gallery;

    [Header("customerPositions")]
    [SerializeField]
    private Transform doorPos;
    [SerializeField]
    private Transform chairPos;

    [Header("hair info")]
    public Sprite govermentHair;
    public Sprite[] customerHaircuts;

    public string[] customerDataQueue;

    private int customerCount = 0;

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
        //spawn customer
        currentCustomer = Instantiate(customerPrefab, doorPos).GetComponent<Customer>();

        //apply data (speak bubble, desired haircut and appearance
        currentCustomer.CustomerData = JsonUtility.FromJson<CustomerData>(Data.LoadJSONFileAsText(customerDataQueue[customerCount % customerDataQueue.Length]));
        chair.customer = currentCustomer;

        //let the customer walk
        StartCoroutine(NextCustomerWalkngIn());

    }

    public IEnumerator NextCustomerWalkngIn()
    {
        yield return StartCoroutine(currentCustomer.Walking(chairPos.position)); 
        yield return StartCoroutine(chair.Spinning(false));
    }

    /// <summary>
    /// Gets called after the form of the haircuts are compared.
    /// </summary>
    public void OnHairCutCheck()
    {
        Settings.TotalPrecentage += formChecker.precentageCorrect;
        Settings.AmountOfCustomers++;
        if (gallery != null)
        {
            gallery.AddFrame(formChecker.portaitShot);
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

    /// <summary>
    /// Finishes the haircut and compares the haircut with what the customer wants.
    /// </summary>
    public void HairCutFinished()
    {
        if (currentCustomer.IsWalking) { return; }
        StartCoroutine(HairCutFinishing());

    }
    IEnumerator HairCutFinishing()
    {
        customerCount++;
        formChecker.CompareMeshes(currentCustomer.Head, currentCustomer.DesiredHead);
        yield return StartCoroutine(chair.Spinning(true));
        yield return StartCoroutine(currentCustomer.Walking(doorPos.position, true));

        NextCustomerWalksIn();
    }

}
