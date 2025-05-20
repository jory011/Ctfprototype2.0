using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Main UI Refrences")]
    [SerializeField] private GameObject CountDownUIHolder;
    [SerializeField] private GameObject WinUIHolder;
    [SerializeField] private GameObject ReadyUIHolder;
    [Header("FlagUI Holder and Text Presets")]
    [SerializeField] private GameObject FlagHolderUIHolder;
    [SerializeField] private string presetText1 = "You have the flag bring it back to your spawn!";
    [SerializeField] private string presetText2 = "Your friend took the flag steal it form them!";

    [Header("Player-to-Text Mapping (Index Matched cant be botherd changing core events)")]
    [SerializeField] private List<GameObject> playerObjects = new();
    [SerializeField] private List<TextMeshProUGUI> playerTextFields = new();
    private Dictionary<GameObject, TextMeshProUGUI> playerTextMap = new();
    [Header("ik i hsould change the events and have a player text assignment but also what if i like dont??")]
    private bool flagHasBeenPickedUp = false;
    private void OnEnable()
    {
        EventBus.Subscribe<CountdownEvent>(CountdownUI);
        EventBus.Subscribe<WinEvent>(WinUI);
        EventBus.Subscribe<FlagSwap>(FlagSwapUI);
        EventBus.Subscribe<PlayersReadyChangedEvent>(PlayersReadyUI);
        EventBus.Subscribe<PosCalibration>(DisableCountdownUI);
        
    }
    private void OnDisable()
    {
        EventBus.UnSubscribe<CountdownEvent>(CountdownUI);
        EventBus.UnSubscribe<WinEvent>(WinUI);
        EventBus.UnSubscribe<FlagSwap>(FlagSwapUI);
        EventBus.UnSubscribe<PlayersReadyChangedEvent>(PlayersReadyUI);
        EventBus.UnSubscribe<PosCalibration>(DisableCountdownUI);
    
    }
    private void DisableCountdownUI(PosCalibration e)
    {
        CountDownUIHolder.SetActive(false);
        ReadyUIHolder.SetActive(false);
    }
    private void CountdownUI(CountdownEvent e)
    {
        Debug.Log($"Countdown triggered with {e.secondsRemaining} seconds remaining.");
        if (e.secondsRemaining <= 0)
        {
            CountDownUIHolder?.SetActive(false);
        }
        else if (e.secondsRemaining > 0)
        {
            CountDownUIHolder?.SetActive(true);
            CountDownUIHolder.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Countdown " + e.secondsRemaining + ".......";

        }
        
    }

    private void WinUI(WinEvent e)
    {
        FlagHolderUIHolder.SetActive(false);
        WinUIHolder?.SetActive(true);
        WinUIHolder.GetComponentInChildren<TextMeshProUGUI>().text = e.player.name + " won the game good job here is a picture of tung tung tung sahur";

    }
    private void InitializePlayerTextMap()
    {
        playerTextMap.Clear();

        if (playerObjects.Count != playerTextFields.Count)
        {
            Debug.LogError($"Mismatch: {playerObjects.Count} players and {playerTextFields.Count} text fields.");
            return;
        }

        for (int i = 0; i < playerObjects.Count; i++)
        {
            GameObject player = playerObjects[i];
            TextMeshProUGUI text = playerTextFields[i];

            if (player == null || text == null)
            {
                Debug.LogWarning($"Null at index {i}: Player or Text is missing.");
                continue;
            }

            if (!playerTextMap.ContainsKey(player))
            {
                playerTextMap.Add(player, text);
                Debug.Log($"[Init] Linked {player.name} to {text.name}");
            }
            else
            {
                Debug.LogWarning($"Duplicate player entry: {player.name}");
            }
        }
    }

    public void FlagSwapUI(FlagSwap e)
    {
        // Lazy init in case not called in Awake
        if (playerTextMap.Count == 0)
        {
            Debug.Log("Initializing player-text map at runtime.");
            InitializePlayerTextMap();
        }

        if (playerTextMap.Count < 2)
        {
            Debug.LogError("Player-to-text mapping incomplete. Must have at least 2 valid entries.");
            return;
        }

        if (!flagHasBeenPickedUp)
        {
            flagHasBeenPickedUp = true;
            FlagHolderUIHolder?.SetActive(true);
            Debug.Log($"Flag picked up for the first time by {e.newFlagHolder?.name ?? "null"}.");
        }

        foreach (var kvp in playerTextMap)
        {
            if (kvp.Key == null || kvp.Value == null)
            {
                Debug.LogWarning("Skipping null player or text entry in map.");
                continue;
            }

            if (kvp.Key == e.newFlagHolder)
            {
                kvp.Value.text = presetText1;
                Debug.Log($"Set text for {kvp.Key.name}: '{presetText1}'");
            }
            else
            {
                kvp.Value.text = presetText2;
                Debug.Log($"Set text for {kvp.Key.name}: '{presetText2}'");
            }
        }
    }



    private void PlayersReadyUI(PlayersReadyChangedEvent e)
    {
        ReadyUIHolder.GetComponentInChildren<TextMeshProUGUI>().text = e.readyCount.ToString() + "/2 Players Ready";
    }
}
