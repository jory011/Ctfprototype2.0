using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    [SerializeField] private GameObject previousHolder;
    [SerializeField] private GameObject currentHolder;

    private void OnEnable()
    {
        EventBus.Subscribe<FlagSwap>(OnFlagGrabbed);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<FlagSwap>(OnFlagGrabbed);
    }

    private void OnFlagGrabbed(FlagSwap evt)
    {
    
        if (previousHolder == null || currentHolder != evt.newFlagHolder)
        {
            currentHolder = evt.newFlagHolder;
            evt.newFlagHolder = currentHolder; // optional: ensure event reflects final assignment
            previousHolder = currentHolder;
            EventBus.Invoke(new MinimapUpdate(evt.newFlagHolder));

            if (evt.oldFlagHolder != null && evt.oldFlagHolder.TryGetComponent<Movement>(out var movement))
            {
                movement.ApplySpeedDebuff();
            }
        }
         
        
    }
    public bool HasFlag(GameObject player)
    {
        return currentHolder == player;
    }

}
