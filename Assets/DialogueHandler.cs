using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueHandler : MonoBehaviour
{
    public static DialogueHandler instance;

    [SerializeField]
    private Canvas dialogueCanvas;

    [SerializeField]
    private TMP_Text textField;
    [SerializeField]
    private TMP_Text nameField;
    
    
    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(instance.gameObject);
            }
        }

        instance = this;
    }

    private bool dialogueShowing = false;
    
    private DialogueInfo currentDialogue;
    private int currentLine = 0;

    private string fullText;

    private bool textDisplaying = false;

    private float delay = 0.02f;

    private Coroutine currentRoutine;
    public void StartDialogue(DialogueInfo info)
    {
        EscapeMenu.instance.menuDisabled = true;
        PlayerManager.instance.DisableControls(true);
        PlayerManager.instance.MovePlayerDynamically(info.playerPosition);
        info.dialogueCam.SetActive(true);
        currentDialogue = info;
        currentLine = 0;
        dialogueShowing = true;
        dialogueCanvas.enabled = true;
        ReadLine();
    }

    private void Update()
    {
        if (!dialogueShowing) return;
        if (Input.anyKeyDown)
        {
            if (textDisplaying)
            {
                if (currentRoutine != null)
                {
                    StopCoroutine(currentRoutine);
                    currentRoutine = null;
                    textField.text = fullText;
                    textDisplaying = false;
                }
            }
            else
            {
                if (currentLine >= currentDialogue.dialogueLine.Length) DialogueEnd();
                else ReadLine();
            }
        }
    }

    private void ReadLine()
    {
        textDisplaying = true;
        nameField.text = currentDialogue.character[currentLine].name;
        currentDialogue.character[currentLine].Bounce();
        fullText = currentDialogue.dialogueLine[currentLine];

        currentRoutine = StartCoroutine(ShowText());
        currentLine++;
    }

    private void DialogueEnd()
    {
        EscapeMenu.instance.menuDisabled = false;
        textDisplaying = false;
        currentRoutine = null;
        dialogueCanvas.enabled = false;
        dialogueShowing = false;
        currentDialogue.actionToExecuteOnComplete?.Invoke();
        currentDialogue.dialogueCam.SetActive(false);
        PlayerManager.instance.DisableControls(false);
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length+1; i++)
        {
            textField.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(delay);
        }
        textDisplaying = false;
        currentRoutine = null;
    }
}
