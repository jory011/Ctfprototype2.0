using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaptureRaycast : MonoBehaviour
{
    [SerializeField] private bool useGamepad;
    [SerializeField] private PlayerInputActions inputActions;
    [SerializeField] private float shootRange = 10f;
    [SerializeField] private bool uiactive = false;
    [SerializeField] private Transform camtransform;
    private bool previousUIActive = false;
    private Coroutine uiPingCoroutine;
    private string PlayerTag;
    private GameObject NullCheckValue;
    
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
     private void Tagsetter(TagIntializeEvent e)
    {
        PlayerTag = e.playertag;

    }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        if (useGamepad)
        {
            inputActions.Gamepad.Enable();
            inputActions.Keyboard.Disable();
        }
        else
        {
            inputActions.Keyboard.Enable();
            inputActions.Gamepad.Disable();
        }
    }

    private void OnFlagGrabbed(FlagSwap e)
    {
        NullCheckValue = e.newFlagHolder;
    }
    private void OnShoot()
    {
        // Use this GameObject's transform as origin
        Vector3 origin = camtransform.position;
        Vector3 direction = camtransform.forward;

        // Perform the raycast
        if (Physics.Raycast(origin, direction, out RaycastHit hit, shootRange))
        {
            Debug.DrawLine(origin, hit.point, Color.red, 1f);  // Optional: visualiz

            if (hit.collider.CompareTag(PlayerTag) && hit.collider.gameObject != gameObject)
            {
                if (NullCheckValue != null)
                {
                    EventBus.Invoke(new FlagSwap(gameObject, hit.transform.gameObject));
                    Debug.Log(gameObject.name + hit.transform.gameObject + "Flagswapped to,from"); 
                }

            }

        }
        
    }
    private void Update()
{
    UpdateUIRaycastCheck();

    bool jumpPressed = useGamepad
           ? inputActions.Gamepad.Shoot.triggered
           : inputActions.Keyboard.Shoot.triggered;
    if (jumpPressed)
    {
        OnShoot();
    }

    // Start coroutine if uiactive became true this frame
    if (uiactive && !previousUIActive)
    {
        if (uiPingCoroutine != null) StopCoroutine(uiPingCoroutine);
        uiPingCoroutine = StartCoroutine(UIPingRoutine());
    }
    // Stop coroutine if uiactive became false this frame
    else if (!uiactive && previousUIActive)
    {
        if (uiPingCoroutine != null)
        {
            StopCoroutine(uiPingCoroutine);
            uiPingCoroutine = null;
        }
    }

    // Store current state for next frame comparison
    previousUIActive = uiactive;
}

    private void UpdateUIRaycastCheck()
    {

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, shootRange))
        {
            Debug.DrawLine(origin, hit.point, Color.red, 1f);
            Debug.Log($"Raycast hit: {hit.collider.name}");

            if (hit.collider.CompareTag(PlayerTag))
            {
                uiactive = true;
                return;
            }


        }
        uiactive = false;

    }
    private IEnumerator UIPingRoutine()
    {
        while (uiactive)
        {
            Debug.Log("UI Active: Target in sight");
            EventBus.Invoke(new InFlagRange(gameObject));
            yield return new WaitForSeconds(0.2f);
        }
    }

}
