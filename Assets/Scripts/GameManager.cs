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
    [SerializeField]
    private ATM atm;

    [Header("customerPositions")]
    [SerializeField]
    private Transform doorPos;
    [SerializeField]
    private Transform greetingPos;
    [SerializeField]
    private Transform chairPos;
    [SerializeField]
    private Transform spawnPos;

    [Header("hair info")]
    [Tooltip("Minimum percentage for when a haircut is close enough to be a government haircut.")]
    [SerializeField] private float governmentHairThreshold = 0.5f;
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
        //NextCustomerWalksIn();
    }


    public void NextCustomerWalksIn()
    {
        //spawn customer
        currentCustomer = Instantiate(customerPrefab).GetComponent<Customer>();
        currentCustomer.transform.position = spawnPos.position;

        VRInputModule.instance.customerCanvas = currentCustomer.canvas;

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
            //load hair if character visited before
            if (Data.RECURING_CHARACTER_VISITS > 0)
            {
                currentCustomer.LoadHair(Data.HAIRCUTS_FOLDER_NAME, Data.RECURRING_CHARACTER_HAIRCUT_CURRENT_FILENAME);
            } else
            {
                currentCustomer.LoadHair(Data.HAIRCUTS_FOLDER_NAME, Data.PREVIOUS_GOVERMENT_FILE_NAME);
            }

            if (nextDialogueData.fileNames.Length > 1) //more than one file?
            {
                //recurring match is more than 0.8, then load positive.
                if (Data.RECURRING_CHARACTER_IS_GIVEN_GOVERMENT_HAIR_SINCE_LAST_VISIT == false)
                {
                    jsonData = nextDialogueData.fileNames[1];
                }
            }
        } else
        {
            currentCustomer.LoadHair(Data.HAIRCUTS_FOLDER_NAME, Data.PREVIOUS_GOVERMENT_FILE_NAME);
        }

        //load json file as a c# object
        currentCustomer.CustomerData = JsonUtility.FromJson<CustomerData>(
            Data.LoadJSONFileAsText(jsonData)
        );

    }

    public IEnumerator NextCustomerWalkngIn()
    {
        //customer walks to greeting pos through the door
        yield return StartCoroutine(currentCustomer.movement.GoTo(doorPos.position));
        yield return StartCoroutine(currentCustomer.movement.GoTo(greetingPos.position));

        //wait for player to respond
        yield return StartCoroutine(currentCustomer.Greeting());

        //customer walks to chair position
        yield return StartCoroutine(currentCustomer.movement.GoTo(chairPos.position) );
        yield return StartCoroutine(currentCustomer.movement.Orienting(currentCustomer.transform.position + new Vector3(10f, 0, 0)));

        currentCustomer.Sit(true);

        //chair rotates
        yield return StartCoroutine(chair.Spinning(false));

        EditMode = true;
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
        //resets the atm position
        atm.ResetPosition();

        //calculate price
        int price = currentCustomer.CustomerData.basePrice;
        int tip = 0;
        //if customer is happy, he/she gives a fine tip!
        // If customer wanted and got government hair, tip as much as it looks alike.
        if (currentCustomer.customerData.wantsGovernmentHair && formChecker.govermentPrecentage > governmentHairThreshold)
        {
            tip += (int)(formChecker.govermentPrecentage * currentCustomer.CustomerData.maxTip);
        }
        // If customer didn't want government hair and didn't get it, tip max.
        else if (currentCustomer.customerData.wantsGovernmentHair && formChecker.govermentPrecentage < governmentHairThreshold)
        {
            tip += currentCustomer.customerData.maxTip;
        }
        money += price + tip;

        //update terminal ui
        resultTerminal.ShowResult(formChecker.govermentPrecentage, formChecker.desiredPrecentage, price, tip);
    }

    public bool EditMode
    {
        get { return editMode; }
        set { editMode = value;
            if (currentCustomer != null)
            {
                currentCustomer.neckBone.enabled = !value;
            }
        }
    }

    /// <summary>
    /// Finishes the haircut and compares the haircut with what the customer wants.
    /// </summary>
    public void HairCutFinished()
    {
        //if out of edit mode/haircut mode or the customer is walking, you can't finish.
        if (!EditMode || currentCustomer.movement.IsMoving) { return; }
        EditMode = false;

        //you can save the hair
        //currentCustomer.SaveHair();

        StartCoroutine(HairCutFinishing());

    }

    IEnumerator HairCutFinishing()
    {
        //play cash register sound
        AudioManager.instance?.Play3DSound(AudioEffect.cashRegister, 1, chair.transform.position);
        //play cash register particle
        atm.ShowParticle();

        yield return new WaitForSeconds(.5f);

        //char is spinning
        yield return StartCoroutine(chair.Spinning(true));

        //compares the haircut
        yield return StartCoroutine(formChecker.getPrecentageFilled(currentCustomer.Head, currentCustomer.DesiredHead, govermentHair));

        //Did the customer get what they wanted.
        bool customerGotWhatTheyWanted = false;
        // Customer wants government haircut and got government haircut.
        if (currentCustomer.customerData.wantsGovernmentHair && formChecker.govermentPrecentage > governmentHairThreshold) customerGotWhatTheyWanted = true;
        // Customer didn't want government haircut and got something else.
        if (!currentCustomer.customerData.wantsGovernmentHair && formChecker.govermentPrecentage < governmentHairThreshold) customerGotWhatTheyWanted = true;

        //if recurring character
        if (customerDataQueue[customerCount % customerDataQueue.Length].recurringCharacter)
        {
            Data.RECURRING_CHARACTER_IS_GIVEN_GOVERMENT_HAIR_SINCE_LAST_VISIT = formChecker.govermentPrecentage > governmentHairThreshold;
            //save hair and update data
            currentCustomer.SaveHair(Data.HAIRCUTS_FOLDER_NAME, Data.RECURRING_CHARACTER_HAIRCUT_CURRENT_FILENAME);
            Data.RECURRING_CHARACTER_IS_POSITIVE_SINCE_LAST_VISIT = customerGotWhatTheyWanted;
            Data.RECURING_CHARACTER_VISITS++;
        }

        //save the data to playermade haircuts
        Settings.AmountOfCustomers = (Settings.AmountOfCustomers + 1) % Data.MAX_FILES_IN_PLAYER_FOLDER;
        currentCustomer.SaveHair(Data.PLAYER_HAIRCUTS_FOLDER_NAME, Settings.AmountOfCustomers + ".hair");

        yield return new WaitForSeconds(1f);

        //customer standsup and looks at player
        currentCustomer.Sit(false);
        yield return StartCoroutine(currentCustomer.movement.GoTo(greetingPos.position));
        yield return StartCoroutine(currentCustomer.movement.Orienting(
            currentCustomer.transform.position + 
            new Vector3(0, 0, 1) * 5f
            ));
        
        //reaction of customer on how much the haircut resembles the government haircut
        yield return currentCustomer.Reaction(customerGotWhatTheyWanted);
        yield return new WaitForSeconds(2f);

        //updates the money and gallery
        UpdateStore();

        //customer walks out
        currentCustomer.canvas.gameObject.SetActive(false);
        yield return StartCoroutine(currentCustomer.movement.GoTo(doorPos.position));
        yield return StartCoroutine(currentCustomer.movement.GoTo(spawnPos.position, true));

        yield return new WaitForSeconds(5f);
        customerCount++;

        //next customer walks in
        NextCustomerWalksIn();

    }
}
