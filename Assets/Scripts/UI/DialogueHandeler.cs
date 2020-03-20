using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DialogueHandeler : MonoBehaviour
{
    [SerializeField] private Text dialogueText;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private float dialogueDuration = 0.5f;
    [SerializeField] private Text nameText;
    [SerializeField] public Customer customer;

    public bool greetings = false;

    public void BeginDialogue(Dialogue dialogue, string name) {
        //set dialogue text
        BeginLine(dialogue.text, name);

        RemoveButtons();
        for (int i = 0; i < dialogue.responses.Length; i++)
        {
            AddButton(i, dialogue);
        }
    }
    public void AddButton(int i, Dialogue dialogue = null)
    {
        GameObject tempButton = Instantiate(buttonPrefab, buttonParent);
        tempButton.SetActive(true);
        //set pos
        if (dialogue != null)
        {
            //set text
            tempButton.GetComponentInChildren<Text>().text = dialogue.responses[i].text;
            tempButton.GetComponent<RectTransform>().localPosition = new Vector3(0, 0 - 40 * i, 0);

            //set button event
            int ii = i; //WHY DOES THIS WORK
            tempButton.GetComponent<Button>().onClick.AddListener(delegate {
                int id = dialogue.responses[ii].dialogueID;
                if (id < customer.customerData.dialogues.Length)
                {
                    BeginDialogue(customer.customerData.dialogues[id], name);
                }
            });

        }
        else //it is greetings button
        {
            //set text
            tempButton.GetComponentInChildren<Text>().text = "Yes";
            tempButton.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            //set button event
            tempButton.GetComponent<Button>().onClick.AddListener(delegate {
                greetings = false;
            });


        }
    }
    private void RemoveButtons()
    {
        //destroy all buttons
        foreach (Transform child in buttonParent.transform)
        {
            GameObject.Destroy(child.gameObject);
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