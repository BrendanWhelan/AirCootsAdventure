using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToEnable;

    public void Enable()
    {
        objectToEnable.SetActive(true);
    }
}
