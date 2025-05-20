using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ReadyPlayerEntry
{
    public GameObject player;
    public string zoneID;
    public bool ready;
    public Vector3 teleportpos;
}

public class ReadyManager : MonoBehaviour
{
    [SerializeField] private List<ReadyPlayerEntry> readyPlayerEntries = new List<ReadyPlayerEntry>();
    [SerializeField] private List<Movement> Players = new List<Movement>();
    private Coroutine countdownCoroutine;

    void OnEnable()
    {
        EventBus.Subscribe<ReadyEvent>(OnReadyEvent);

    }

    void OnDisable()
    {
        EventBus.UnSubscribe<ReadyEvent>(OnReadyEvent);
    }

    void Start()
    {
        PlayertagGetter();//intilization bus also error preventing using eventbus to feed scripts player.tag will give error if they are misassigned
    }
    private void PlayertagGetter()
    {
        Players = FindObjectsOfType<Movement>().ToList();

        if (Players.Count == 0)
        {
            Debug.LogWarning("No players found.");
            return;
        }

        string commonTag = Players[0].gameObject.tag;
        bool allSame = Players.All(player => player.gameObject.tag == commonTag);

        if (allSame)
        {
            EventBus.Invoke(new TagIntializeEvent(commonTag));

        }
        else
        {
            Debug.LogError("Not all players share the same tag. pls make sure they all share a logically named tag");
        }
    }


    private void OnReadyEvent(ReadyEvent evt)
    {
        if (evt == null)
        {
            Debug.LogWarning("Received null ReadyEvent. Check event bus setup.");
            return;
        }

        bool playerFound = Players.Any(p => p.gameObject == evt.player);
        if (!playerFound) return;

        bool stateChanged = false;
        var existingEntry = readyPlayerEntries.FirstOrDefault(e => e.player == evt.player);

        if (evt.ready)
        {
            if (existingEntry == null)
            {
                readyPlayerEntries.Add(new ReadyPlayerEntry
                {
                    player = evt.player,
                    zoneID = evt.zoneID,
                    ready = true,
                    teleportpos = evt.telportpos // this must be unique per player!
                });
                Debug.Log($"Player {evt.player.name} is now ready in zone {evt.zoneID}");
            }

            else
            {
                existingEntry.ready = true;
                existingEntry.zoneID = evt.zoneID;
                existingEntry.teleportpos = evt.telportpos;
                Debug.Log($"Player {evt.player.name} updated and marked ready in zone {evt.zoneID}");
            }
            stateChanged = true;
        }

        else
        {
            if (existingEntry != null && existingEntry.ready)
            {
                existingEntry.ready = false;
                Debug.Log($"Player {evt.player.name} is no longer ready.");
                stateChanged = true;
            }
        }

        if (stateChanged)
        {
            int readyCount = readyPlayerEntries.Count(e => e.ready);
            
        }

        var readyEntries = readyPlayerEntries.Where(e => e.ready && e.player != null).ToList();

        bool allPlayersReady = readyEntries.Count == Players.Count && readyEntries.Select(e => e.zoneID).Distinct().Count() == readyEntries.Count;

        EventBus.Invoke(new PlayersReadyChangedEvent(readyEntries.Select(e => e.zoneID).Distinct().Count(), Players.Count));

        if (allPlayersReady)
        {

            if (countdownCoroutine == null)
            {
                
                countdownCoroutine = StartCoroutine(StartCountdown(3));
            }
        }
        else
        {
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
                EventBus.Invoke(new CountdownEvent(0)); 
                Debug.Log("Countdown canceled: not all players ready.");
            }
        }
    }

    private IEnumerator StartCountdown(int seconds)
    {
        int remaining = seconds;

        // Snapshot of players that triggered the countdown
        List<ReadyPlayerEntry> countdownSnapshot = readyPlayerEntries
            .Where(e => e.ready && e.player != null)
            .Select(e => new ReadyPlayerEntry
            {
                player = e.player,
                zoneID = e.zoneID,
                teleportpos = e.teleportpos,
                ready = true
            }).ToList();

        Debug.Log(" Snapshot taken for countdown teleportation.");

        while (remaining > 0)
        {
            // Re-check current ready status to possibly reset the countdown timer
            var currentReadyEntries = readyPlayerEntries.Where(e => e.ready && e.player != null).ToList();

            bool allPlayersStillReady = currentReadyEntries.Count == Players.Count && currentReadyEntries.Select(e => e.zoneID).Distinct().Count() == currentReadyEntries.Count;

            if (!allPlayersStillReady)
            {
                if (remaining != seconds)
                {
                    Debug.Log("Countdown interrupted — resetting to full duration.");
                    EventBus.Invoke(new CountdownEvent(0)); // optional UI feedback
                }

                remaining = seconds;
                yield return null;
                continue;
            }

            EventBus.Invoke(new CountdownEvent(remaining));
            Debug.Log($"⏳ Countdown: {remaining}");

            yield return new WaitForSeconds(1f);
            remaining--;
        }

        Debug.Log("✅ Countdown complete. Teleporting all players in snapshot regardless of current state.");

        foreach (var entry in countdownSnapshot)
        {
            if (entry.player != null)
            {
                Debug.Log($"Teleporting {entry.player.name} to {entry.teleportpos} (Zone {entry.zoneID})");
                entry.player.transform.position = entry.teleportpos;
                EventBus.Invoke(new CountdownEvent(0));
                EventBus.Invoke(new PosCalibration(entry.player));
            }
            else
            {
                Debug.LogWarning("Skipped teleporting null player in snapshot.");
            }
        }

        countdownCoroutine = null;
    }

}
