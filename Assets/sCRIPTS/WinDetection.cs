using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WinDetection : MonoBehaviour
{

    [SerializeField] private GameObject CurrentFlagholder;
    [SerializeField] private GameObject ValidPlayer;
    [SerializeField] private string PlayerTag;
    private void OnEnable()
    {
        EventBus.Subscribe<FlagSwap>(OnFlagGrabbed);
        EventBus.Subscribe<TagIntializeEvent>(Tagsetter);


    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<FlagSwap>(OnFlagGrabbed);
        EventBus.UnSubscribe<TagIntializeEvent>(Tagsetter);
    }
    public void OnFlagGrabbed(FlagSwap e)
    {
        CurrentFlagholder = e.newFlagHolder;
        Debug.Log("tag intlized for script" + name + "on object" + gameObject);

    }
    private void Tagsetter(TagIntializeEvent e)
    {
        PlayerTag = e.playertag;
       
    }
    void OnTriggerEnter(Collider other)
    {
        if (CurrentFlagholder != null || ValidPlayer != null)
        {
            if (CurrentFlagholder == other.gameObject && other.gameObject == ValidPlayer)
            {
                if (other.gameObject.tag == PlayerTag)
                {
                    EventBus.Invoke(new WinEvent(CurrentFlagholder));
                }

            }
        }
        if (ValidPlayer == null)
        {
            if (other.gameObject.tag == PlayerTag)
            {
                ValidPlayer = other.gameObject;
            }
        }
    }

}
