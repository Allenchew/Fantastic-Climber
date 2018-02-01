using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    private string sentences;
    bool DisplayNext = false;
    int SentencesCount;

	// Use this for initialization
	void Start () {
        int SentencesCount = 0;
	}

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log(dialogue.name +" : "+ dialogue.sentences[SentencesCount]);
        if (DisplayNext)
        {
            Debug.Log(dialogue.name + " : " + dialogue.sentences[SentencesCount]);
        }
    }

    public void DisplayNextSentences()
    {
        DisplayNext = !DisplayNext;
        SentencesCount += 1;
        Debug.Log("Display Next" + DisplayNext);
    }
    void EndDialogue()
    {

    }

    // Update is called once per frame
    void Update () {
        Debug.Log(SentencesCount);
	}
}
