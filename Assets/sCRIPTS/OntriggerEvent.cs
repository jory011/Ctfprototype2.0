using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OntriggerEvent : MonoBehaviour
{
    private string PlayerTag;
    private void OnEnable()
    {
        EventBus.Subscribe<TagIntializeEvent>(Tagsetter);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<TagIntializeEvent>(Tagsetter);
    }

    private void Tagsetter(TagIntializeEvent e)
    {
        PlayerTag = e.playertag;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == PlayerTag)
        {
            EventBus.Invoke(new FlagSwap(other.gameObject, null));
            gameObject.transform.root.gameObject.SetActive(false);
        }
    }
}
