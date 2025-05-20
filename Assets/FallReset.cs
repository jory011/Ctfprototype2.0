using UnityEngine;
using System.Collections;
using System;
using UnityEditor.UIElements;
using TMPro;


public class PositionSaver : MonoBehaviour
{
    public float saveInterval = 5f;
    public float retryInterval = 1f;
    public float groundCheckDistance = 10f;
    public LayerMask groundLayer;

    private Vector3 savedPosition;
    private bool isTryingToSave = false;
    

    private void OnEnable()
    {
        EventBus.Subscribe<PosCalibration>(TeleportSetter);
    }
     private void OnDisable()
    {
        EventBus.UnSubscribe<PosCalibration>(TeleportSetter);
    }
    private void Start()
    {
        StartCoroutine(SavePositionRoutine());
    }

    private IEnumerator SavePositionRoutine()
    {
        while (true)
        {
            yield return TrySavePositionWithCheck();
            yield return new WaitForSeconds(saveInterval);
        }
    }

    private void TeleportSetter(PosCalibration e)
    {
        if (e.player == gameObject)
        {
            TrySavePositionWithCheck();
        }
        
    }

    private IEnumerator TrySavePositionWithCheck()
    {
        isTryingToSave = true;

        while (isTryingToSave)
        {
            if (IsGroundBelow())
            {
                SavePosition();
                isTryingToSave = false;
            }
            else
            {
                Debug.Log("No ground found. Retrying in " + retryInterval + "s...");
                yield return new WaitForSeconds(retryInterval);
            }
        }
    }

    private bool IsGroundBelow()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        return Physics.Raycast(ray, groundCheckDistance, groundLayer);
    }

    private void SavePosition()
    {
        savedPosition = transform.position;
        Debug.Log("Position saved: " + savedPosition);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "FallOff")
        {
            gameObject.transform.position = savedPosition;
            Debug.Log("savedposition = " + savedPosition);
        }
    }
    
        
    

}
