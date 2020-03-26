using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DialogueHandeler : MonoBehaviour
{
    [SerializeField] private Text dialogueText;

    [SerializeField] private ReactionButton[] reactionButtons;

    [SerializeField] private float dialogueDuration = 0.5f;
    [SerializeField] private Text nameText;
    [SerializeField] public Customer customer;

    public bool greetings = false;

    public void Start()
    {
        HideButtons();
    }
    public void BeginDialogue(Dialogue dialogue, string _name) {
        //set dialogue text
        BeginLine(dialogue.text, _name);

        HideButtons();
        for (int i = 0; i < dialogue.responses.Length; i++)
        {
            AddButton(i, dialogue);
        }
    }
    public void AddButton(int i, Dialogue dialogue = null)
    {

        ReactionButton tempButton = reactionButtons[Mathf.Max(0, Mathf.Min(i, reactionButtons.Length - 1))];

        tempButton.model.SetActive(true);
        tempButton.button.gameObject.SetActive(true);
        //set pos
        if (dialogue != null)
        {
            //set text
            tempButton.button.GetComponentInChildren<Text>().text = dialogue.responses[i].text;

            //set button event
            int ii = i; //WHY DOES THIS WORK
            tempButton.button.onClick.RemoveAllListeners();

            tempButton.button.onClick.AddListener(delegate {
                int id = dialogue.responses[ii].dialogueID;
                if (id < customer.customerData.dialogues.Length)
                {
                    BeginDialogue(customer.customerData.dialogues[id], customer.customerData.name);
                }
            });

        }
        else //it is greetings button
        {
            //set text
            tempButton.button.GetComponentInChildren<Text>().text = "Yes";

            //set button event
            tempButton.button.onClick.RemoveAllListeners();
            tempButton.button.onClick.AddListener( delegate {
                greetings = false;
            });


        }
    }


    private void HideButtons()
    {
        foreach (ReactionButton child in reactionButtons)
        {
            child.model.SetActive(false);
            child.button.gameObject.SetActive(false);
        }
    }

    public IEnumerator Greetings(string line, string name)
    {
        greetings = true;
        BeginLine(line, name);
        AddButton(-1);
        while (greetings)
        {
            yield return new WaitForFixedUpdate();
        }
    }

    public void BeginLine(string line, string name)
    {
        dialogueText.text = "";
        nameText.text = name;

        StopAllCoroutines();
        StartCoroutine(Talking(line));
    }

    private IEnumerator Talking(string line)
    {
        //play talking sound
        AudioManager.instance?.Play3DSound(AudioEffect.dialogueTalk, .5f, transform.position);

        float interval = dialogueDuration / line.Length;
        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            yield return new WaitForSeconds(interval);
        }

        //play talking sound
        AudioManager.instance?.StopSound(AudioEffect.dialogueTalk);

    }
}

[System.Serializable]
public class ReactionButton
{
    public Button button;
    public GameObject model;
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