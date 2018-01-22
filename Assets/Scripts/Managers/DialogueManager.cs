using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	Queue<string> sentences;

	public Text nameText;
	public Text dialogueText;
	public Animator animator;

	public static DialogueManager Instance;


	// Use this for initialization
	void Start () {

		if(Instance){
			DestroyImmediate(gameObject);
		}
		else
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}

		sentences = new Queue<string>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartDialogue(Dialogue dialogue){

		animator.SetBool ("isOpen", true);
		//print ("here"); 
		sentences.Clear();

		nameText.text = dialogue.name;

		foreach (string sentence in dialogue.sentences) {
			sentences.Enqueue (sentence);

		}
		DisplayNextSentence ();
	}

	public void DisplayNextSentence(){
		if (sentences.Count == 0) {
			EndDialogue ();
			return;
		}
		string sentence = sentences.Dequeue ();
		StopAllCoroutines ();
		StartCoroutine (TypeSentence (sentence));

		//dialogueText.text = sentence; //This displays the sentence instantly
	}

	IEnumerator TypeSentence(string sentence){
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray()) {
			dialogueText.text += letter;
			yield return null;
		}
	}

	void EndDialogue(){
		animator.SetBool ("isOpen", false);
		print ("End of Conversation");
	}

}
