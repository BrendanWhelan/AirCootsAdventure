using System;
using UnityEngine;
using UnityEngine.Events;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField]
    private DialogueRequirement[] dialoguesToCheck;

    [SerializeField]
    private DialogueInfo[] dialogues;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            for(int i=0; i < dialoguesToCheck.Length;i++)
            {
                if (GameManager.instance.TryDialogue(dialoguesToCheck[i]))
                {
                    InitiateDialogue(i);
                    break;
                }
            }
        }
    }

    private void InitiateDialogue(int id)
    {
        DialogueHandler.instance.StartDialogue(dialogues[id]);
    }
}

[System.Serializable]
public class DialogueRequirement
{
    public int dialogueToCheck;
    public int objectiveToCheck=-1;
}

[System.Serializable]
public class DialogueInfo
{
    public Vector3 playerPosition;
    public GameObject dialogueCam;
    public NPCCharacter[] character;
    public string[] dialogueLine;
    public UnityEvent actionToExecuteOnComplete;
}
