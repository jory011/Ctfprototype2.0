using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderReady : MonoBehaviour
{
    [SerializeField] private string zoneID; // Assign in Inspector, readonly from code
    [SerializeField] private Transform teleportpos;
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

    public string ZoneID => zoneID; // Readonly accesso
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(PlayerTag) && teleportpos != null)
        {

            EventBus.Invoke(new ReadyEvent(other.gameObject, zoneID, true, teleportpos.position));

        }
    }

    private void OnTriggerExit(Collider other)
    {


        if (other.CompareTag(PlayerTag) && teleportpos != null)
        {
            EventBus.Invoke(new ReadyEvent(other.gameObject, zoneID, false, teleportpos.position));
        }

    }
}
