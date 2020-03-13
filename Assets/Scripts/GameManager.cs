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

    [Header("Enviroment")]
    [SerializeField]
    public Chair chair;
    [SerializeField]
    private ImageGallery gallery;
    [SerializeField]
    private ResultTerminal resultTerminal;

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

    public int money = 0;

    private bool editMode = false;

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
        //customer walks to chair position
        yield return StartCoroutine(currentCustomer.Walking(chairPos.position)); 
        
        //chair rotates
        yield return StartCoroutine(chair.Spinning(false));
        editMode = true;
    }

    /// <summary>
    /// Gets called after the form of the haircuts are compared.
    /// </summary>
    public void UpdateStore()
    {
        Settings.TotalPrecentage += formChecker.desiredPrecentage;
        Settings.AmountOfCustomers++;

        if (gallery != null)
        {
            gallery.AddFrame(formChecker.portaitShot);
            //gallery.AddFrame(formChecker.refTexture);
            //gallery.AddFrame(formChecker.selectedTexture);
        }
        int price = currentCustomer.CustomerData.basePrice;
        int tip = 0;
        if (formChecker.desiredPrecentage > 0.7f)
        {
            tip += (int)(formChecker.desiredPrecentage * currentCustomer.CustomerData.maxTip);
        }
        money += price + tip;

        //update terminal ui
        resultTerminal.ShowResult(formChecker.govermentPrecentage, formChecker.desiredPrecentage, price, tip);
        Debug.Log("money: " + money + "$");
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
        if (!editMode || currentCustomer.IsWalking) { return; }
        editMode = false;
        StartCoroutine(HairCutFinishing());

    }
    IEnumerator HairCutFinishing()
    {
        //char is spinning
        yield return StartCoroutine(chair.Spinning(true));

        //compares the haircut
        yield return StartCoroutine(formChecker.getPrecentageFilled(currentCustomer.Head, currentCustomer.DesiredHead, govermentHair));

        yield return new WaitForSeconds(0.5f);

        //updates the money and gallery
        UpdateStore();

        //customer walks out of store
        yield return StartCoroutine(currentCustomer.Walking(doorPos.position, true));

        customerCount++;

        //next customer walks in
        NextCustomerWalksIn();

    }

}
