using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalHandler : MonoBehaviour
{
    [SerializeField]
    private Portal[] portals;

    public static PortalHandler instance;

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

    public void EnablePortal(int portalToEnable)
    {
        portals[portalToEnable].EnablePortal();
        if(SceneManager.GetActiveScene().name.Equals("Main Scene"))
            GameManager.instance.SetPortalEnabled(portalToEnable);
    }

    public void EnablePortalNoSave(int portalToEnable)
    {
        portals[portalToEnable].EnablePortal();
    }
}
