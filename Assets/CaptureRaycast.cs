using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaptureRaycast : MonoBehaviour
{
    [SerializeField] private bool useGamepad;
    [SerializeField] private PlayerInputActions inputActions;
    [SerializeField] private float shootRange = 10f;
    [SerializeField] private bool uiactive = false;
    [SerializeField] private LayerMask detectionMask; // Optional for performance
    private Coroutine uiPingCoroutine;


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
    private void OnShoot()
    {
        // Use this GameObject's transform as origin
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        // Perform the raycast
        if (Physics.Raycast(origin, direction, out RaycastHit hit, shootRange))
        {
            Debug.DrawLine(origin, hit.point, Color.red, 1f);  // Optional: visualiz

            if (hit.collider.CompareTag("Player"))
            {
                EventBus.Invoke(new FlagSwap(gameObject, hit.transform.gameObject));
            }
        }
        else
        {
            Debug.DrawRay(origin, direction * shootRange, Color.green, 1f);  // Optional: visualize miss
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
        bool previousUIActive = uiactive;
        if (uiactive && !previousUIActive)
        {
            if (uiPingCoroutine != null) StopCoroutine(uiPingCoroutine);
            uiPingCoroutine = StartCoroutine(UIPingRoutine());
        }
        else if (!uiactive && previousUIActive)
        {
            if (uiPingCoroutine != null)
            {
                StopCoroutine(uiPingCoroutine);
                uiPingCoroutine = null;
            }
        }
    }
    private void UpdateUIRaycastCheck()
    {

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, shootRange))
        {
            Debug.DrawLine(origin, hit.point, Color.red, 1f);
            Debug.Log($"Raycast hit: {hit.collider.name}");

            if (hit.collider.CompareTag("Player"))
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
