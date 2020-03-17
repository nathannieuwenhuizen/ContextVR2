using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Hair Data")]
    public string fileName = "testHairSave.hair";
    public string directory = "/saves";
    [Space]
    [Space]
    [Space]

    [SerializeField]
    private MeshFormChecker formChecker;

    //customer info
    [HideInInspector]
    public Customer currentCustomer;
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

    public DialogueData[] customerDataQueue;

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


        //load/apply data (dialogue, desired haircut and appearance)
        loadCustomerData();

        //set customer to chair for spinning function.
        chair.customer = currentCustomer;

        //let the customer walk
        StartCoroutine(NextCustomerWalkngIn());

    }

    public void loadCustomerData()
    {
        //get next dialogue data.
        DialogueData nextDialogueData = customerDataQueue[customerCount % customerDataQueue.Length];

        string jsonData = nextDialogueData.fileNames[0];
        //check if it is recurring character.
        if (nextDialogueData.recurringCharacter)
        {
            if (nextDialogueData.fileNames.Length > 1) //more than one file?
            {
                //recurring match is more than 0.8, then load positive.
                if (Data.recurringCharacterMatch > 0.8f)
                {
                    jsonData = nextDialogueData.fileNames[1];
                }
            }
        }
        //load json file as a c# object
        currentCustomer.CustomerData = JsonUtility.FromJson<CustomerData>(
            Data.LoadJSONFileAsText(jsonData)
        );

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
        //add frame picture
        if (gallery != null)
        {
            gallery.AddFrame(formChecker.portaitShot);
            //gallery.AddFrame(formChecker.refTexture);
            //gallery.AddFrame(formChecker.selectedTexture);
        }

        //calculate price
        int price = currentCustomer.CustomerData.basePrice;
        int tip = 0;
        if (formChecker.desiredPrecentage > 0.7f)
        {
            tip += (int)(formChecker.desiredPrecentage * currentCustomer.CustomerData.maxTip);
        }
        money += price + tip;

        //update terminal ui
        resultTerminal.ShowResult(formChecker.govermentPrecentage, formChecker.desiredPrecentage, price, tip);
    }

    private void Update()
    {

        //just for testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HairCutFinished();
        }
        //just for testing
        if (Input.GetKeyDown(KeyCode.L))
        {
            //currentCustomer.LoadHair();
        }
    }

    /// <summary>
    /// Finishes the haircut and compares the haircut with what the customer wants.
    /// </summary>
    public void HairCutFinished()
    {
        //if out of edit mode/haircut mode or the customer is walking, you can't finish.
        if (!editMode || currentCustomer.IsWalking) { return; }
        editMode = false;

        //you can save the hair
        //currentCustomer.SaveHair();

        StartCoroutine(HairCutFinishing());

    }

    IEnumerator HairCutFinishing()
    {
        //char is spinning
        yield return StartCoroutine(chair.Spinning(true));

        //compares the haircut
        yield return StartCoroutine(formChecker.getPrecentageFilled(currentCustomer.Head, currentCustomer.DesiredHead, govermentHair));

        //if recurring character, save the desired match data.
        if (customerDataQueue[customerCount % customerDataQueue.Length].recurringCharacter)
        {
            Data.recurringCharacterMatch = formChecker.desiredPrecentage;
        }

        //wait for a time.
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
