using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OntriggerEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            EventBus.Invoke(new FlagSwap(other.gameObject, null));
            gameObject.SetActive(false);
        }
    }
}
