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

    public void BeginDialogue(Dialogue dialogue, string name) {
        //set dialogue text
        BeginLine(dialogue.text, name);

        //destroy all buttons
        foreach (Transform child in buttonParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //make all buttons
        if (dialogue.responses.Length > 0)
        {
            for (int i = 0; i < dialogue.responses.Length; i++)
            {
                GameObject tempButton = Instantiate(buttonPrefab, buttonParent);
                tempButton.SetActive(true);
                //set pos
                tempButton.GetComponent<RectTransform>().localPosition = new Vector3(-25 + 50 * i, 0, 0);
                //set text
                tempButton.GetComponentInChildren<Text>().text = dialogue.responses[i].text;
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
        AudioManager.instance?.Play3DSound(AudioEffect.dialogueTalk, 1, transform.position);

        float interval = dialogueDuration / line.Length;
        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            yield return new WaitForSeconds(interval);
        }
    }
}