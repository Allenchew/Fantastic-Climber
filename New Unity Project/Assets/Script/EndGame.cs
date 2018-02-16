using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour {

    public GameObject UI;
    public Text DialogueStart;
    // Use this for initialization
    void Start () {
        UI.SetActive(false);
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            UI.SetActive(true);
            DialogueStart.text = "謎の人と話しませんか？";
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
