using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 0.1f;
    [SerializeField] private float rotateSpeed = 2f;

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
    [SerializeField] public Canvas canvas;

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
    public bool IsMoving { get; private set; } = false;

    private void Update() {
        AimCanvasToCamera();
    }
    public void Start()
    {
        canvas.worldCamera = VRInputModule.instance.currentCamera;
        dialogueHandeler.customer = this;
    }
    public CustomerData CustomerData
    {
        get { return customerData; }
        set {
            customerData = value;
            if (customerData.dialogues.Length > 0)
            {
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
    public IEnumerator Greeting()
    {
        yield return StartCoroutine( dialogueHandeler.Greetings(customerData.greetingDialogue, customerData.name));
        dialogueHandeler.BeginDialogue(customerData.dialogues[0], customerData.name);
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

    public IEnumerator Orienting(Vector3 destination)
    {
        Vector3 newDirection;

        while (Mathf.Abs(Vector3.Angle(transform.forward, destination - transform.position)) > 0.1f)
        {
            newDirection = Vector3.RotateTowards(transform.forward, destination - transform.position, Time.deltaTime * rotateSpeed, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            yield return new WaitForFixedUpdate();
        }
        newDirection = Vector3.RotateTowards(transform.forward, destination - transform.position, 1f, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

    }
    public IEnumerator Walking(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.Translate(transform.InverseTransformDirection(transform.forward) * walkSpeed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = destination;
    }

    public IEnumerator GoTo(Vector3 pos, bool destoryWhenFinish = false)
    {
        if (!IsMoving)
        {
            IsMoving = true;
            yield return StartCoroutine(Orienting(pos));
            yield return StartCoroutine(Walking(pos));
            IsMoving = false;
        }
        if (destoryWhenFinish)
        {
            Destroy(this.gameObject);
        }

    }

    public void LoadHair(string directory = "/saves", string fileName = "testHairSave.hair")
    {
        foreach (HairObject child in GetComponentsInChildren<HairObject>())
        {
            Destroy(child.gameObject);
        }
        HeadData.current = (HeadData)Data.LoadHair(directory, fileName);

        List<Transform> tempTransforms = new List<Transform>();
        for (int i = 0; i < HeadData.current.hairObjects.Count; i++)
        {
            HairData data = HeadData.current.hairObjects[i]; 
            HairObject obj = GameObject.CreatePrimitive(data.meshType).AddComponent<HairObject>();
            obj.GetComponent<MeshRenderer>().material.color = data.color;
            obj.gameObject.name = "hair object #" + i;

            if (data.parentIndex == -1)
            {
                obj.transform.parent = head.transform;
            } else
            {
                obj.transform.parent = tempTransforms[data.parentIndex];
            }
            obj.gameObject.tag = Tags.GRABABLE;
            obj.hairData = data;

            obj.transform.localScale = data.scale;
            obj.transform.localPosition = data.localposition;
            obj.transform.localRotation = data.rotation;

            tempTransforms.Add(obj.transform);
        }
        tempTransforms.Clear();
    }

    public void Reaction(float desiredMatch)
    {
        dialogueHandeler.BeginLine(
            desiredMatch > customerData.minimumPrecentageForPositiveReaction ? 
            customerData.positiveReaction : customerData.negativeReaction, customerData.name);
    }

    public bool SaveHair(string directory = "/saves", string fileName = "testHairSave.hair")
    {
        HeadData.current.test = 6;
        HeadData.current.hairObjects = new List<HairData>();
        foreach (HairObject child in GetComponentsInChildren<HairObject>())
        {
            AddHairObjectToSave(child);
        }
        Data.SaveHair(directory, fileName, HeadData.current);
        return true;
    }
    public void AddHairObjectToSave(HairObject obj)
    {
        if (obj.hairData == null)
        {
            return;
        }
        if (obj.transform.parent != head.transform)
        {
            obj.hairData.parentIndex = HeadData.current.hairObjects.IndexOf(obj.transform.parent.GetComponent<HairObject>().hairData);
        } else
        {
            obj.hairData.parentIndex = -1;
        }
        obj.hairData.localposition = obj.transform.localPosition;
        obj.hairData.scale = obj.transform.localScale;
        obj.hairData.rotation = obj.transform.localRotation;

        HeadData.current.hairObjects.Add(obj.hairData);
    }
}

[System.Serializable]
public class CustomerData
{
    public string name = "name";
    public bool moodForNewHaircut = false;
    public int basePrice = 20;
    public int maxTip = 30;
    public int desiredHaircutID = -1;
    public int appearanceSeed = 0;
    public float minimumPrecentageForPositiveReaction = 0.7f;
    public string greetingDialogue;
    public string positiveReaction;
    public string negativeReaction;
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