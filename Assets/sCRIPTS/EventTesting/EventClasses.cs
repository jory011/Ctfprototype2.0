using UnityEngine;

public class PlayerFellEvent
{
    public string playerName;
    public Vector3 lastValidPosition;

    public PlayerFellEvent(string playerName, Vector3 lastValidPosition)
    {
        this.playerName = playerName;
        this.lastValidPosition = lastValidPosition;
    }
}

public class FlagSwap
{
    public GameObject newFlagHolder;
    public GameObject oldFlagHolder;

    public FlagSwap(GameObject newFlagHolder, GameObject oldFlagHolder)
    {
        this.newFlagHolder = newFlagHolder;
        this.oldFlagHolder = oldFlagHolder;
        
    }
}

public class EnemySpawnedEvent
{
    public Vector3 spawnPosition;
    public string enemyType;

    public EnemySpawnedEvent(Vector3 spawnPosition, string enemyType)
    {
        this.spawnPosition = spawnPosition;
        this.enemyType = enemyType;
    }
}
public class ReadyEvent
{
    public GameObject player;
    public string zoneID;
    public bool ready;
    public Vector3 telportpos;

    public ReadyEvent(GameObject player, string zoneID, bool ready, Vector3 telportpos)
    {
        this.player = player;
        this.zoneID = zoneID;
        this.ready = ready;
        this.telportpos = telportpos;
    }

}
public class CountdownEvent
{
    public int secondsRemaining;
    public CountdownEvent(int secondsRemaining)
    {
        this.secondsRemaining = secondsRemaining;
    }
}
public class PlayersReadyChangedEvent
{
    public int readyCount;
    public int totalCount;

    public PlayersReadyChangedEvent(int readyCount, int totalCount)
    {
        this.readyCount = readyCount;
        this.totalCount = totalCount;
    }

}

public class  InFlagRange
{
    public GameObject player;
 

    public InFlagRange(GameObject player)
    {
        this.player = player;
        
    }

}
public class  MinimapUpdate
{
    public GameObject player;
 

    public MinimapUpdate(GameObject player)
    {
        this.player = player;
        
    }

}

