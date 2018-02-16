using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    private Queue<string> sentences;
    bool DisplayNext = false;
    int SentencesCount;
    public GameObject startDialoguebtn;
    public GameObject EndingText;

    public Text nameText;
    public Text dialogueText;

	// Use this for initialization
	void Start () {
        int SentencesCount = 0;
        sentences = new Queue<string>();
	}

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log(dialogue.name +" : "+ dialogue.sentences[SentencesCount]);
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        startDialoguebtn.SetActive(false);
        DisplayNextSentences();

    }

    public void DisplayNextSentences()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        Debug.Log("end dialogue");
        EndingText.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log(SentencesCount);
	}
}
