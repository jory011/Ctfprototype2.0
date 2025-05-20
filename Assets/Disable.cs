using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : MonoBehaviour
{
    [SerializeField] GameObject oeoei;
    private void OnDisable()
    {
        if (oeoei != null)
        {
            oeoei?.SetActive(false);
        }
    }
}
