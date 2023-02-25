using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyHandler : MonoBehaviour
{
    public static PartyHandler instance;

    [SerializeField]
    private GameObject[] slimeObjects;
    [SerializeField]
    private GameObject[] bidetObjects;
    [SerializeField]
    private GameObject[] cakeObjects;

    [SerializeField]
    private GameObject[] slimeObjectivesToDisable;
    [SerializeField]
    private GameObject[] bidetObjectivesToDisable;
    [SerializeField]
    private GameObject[] cakeObjectivesToDisable;
    
    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
            return;
        }

        instance = this;
    }
    
    private void Start()
    {
        UpdateParty();
    }

    public void UpdateParty()
    {
        if (GameManager.instance.GetPartyObjective(0))
        {
            foreach (GameObject go in cakeObjects)
            {
                go.SetActive(true);
            }
            foreach (GameObject go in cakeObjectivesToDisable)
            {
                go.SetActive(false);
            }
        }
        if (GameManager.instance.GetPartyObjective(1))
        {
            foreach (GameObject go in slimeObjects)
            {
                go.SetActive(true);
            }
            foreach (GameObject go in slimeObjectivesToDisable)
            {
                go.SetActive(false);
            }
        }
        if (GameManager.instance.GetPartyObjective(2))
        {
            foreach (GameObject go in bidetObjects)
            {
                go.SetActive(true);
            }
            foreach (GameObject go in bidetObjectivesToDisable)
            {
                go.SetActive(false);
            }
        }
    }
}
