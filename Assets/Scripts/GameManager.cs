using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class GameManager : GlobalEventListener
{

    /*
    #region SINGLETON
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
    #endregion
    */

    #region EDITOR_PARAMS

    public int TotalCrystals = 20;
    public float GameDuration = 60.0f;
    public float AttackCooldown = 0.8f;

    #endregion

    #region DADOS

    [HideInInspector]
    public int LightsOn { get; set; }
    [HideInInspector]
    public int LightsOff { get; set; }
    [HideInInspector]
    public bool GameStarted { get; set; }
    [HideInInspector]
    public bool GameFinished { get; set; }

    [HideInInspector]
    public float CurrentTime { get; set; }

    [HideInInspector]
    public List<GameManagerPlayer> LightPlayers = new List<GameManagerPlayer>();
    [HideInInspector]
    public List<GameManagerPlayer> DarkPlayers = new List<GameManagerPlayer>();

    [HideInInspector]
    public int NextPlayerId { get; set; }

    public enum Team { LIGHT, DARK }
    public enum WinnerTeam { NONE, LIGHT, DARK }

    #endregion

    private void Reset()
    {
        LightsOn = TotalCrystals / 2;
        LightsOff = TotalCrystals / 2;
        CurrentTime = 0.0f;
        LightPlayers.Clear();
        DarkPlayers.Clear();
        NextPlayerId = 1;
        Debug.Log("GameManager resetado");
    }

    public GameManagerPlayer LocalPlayer { get; set; }
    public GameManagerPlayer OwnerPlayer { get; set; }

    #region LOBBY_INTEGRATION

    public void StartRoom(string nickname)
    {
        var player = SpawnPlayer(nickname);
        OwnerPlayer = player;
        LocalPlayer = player;
        OnPlayerLobbyChange();
    }

    public void JoinRoom(string nickname)
    {
        var player = SpawnPlayer(nickname);
        LocalPlayer = player;
        OnPlayerLobbyChange();
    }

    public GameManagerPlayer SpawnPlayer(string nickname)
    {
        var player = new GameManagerPlayer();
        player.Nickname = nickname;
        player.Id = NextPlayerId++;
        if (LightPlayers.Count > DarkPlayers.Count)
        {
            player.Dark = true;
            DarkPlayers.Add(player);
        }
        else
        {
            player.Dark = false;
            LightPlayers.Add(player);
        }
        return player;
    }

    public GameManagerPlayer FindPlayer(int id)
    {
        var player = LightPlayers.Find(x => x.Id == id);
        if (player == null)
        {
            player = DarkPlayers.Find(x => x.Id == id);
        }
        return player;
    }

    public void RemovePlayer(int id)
    {
        var count = LightPlayers.RemoveAll(x => x.Id == id);
        if (count == 0)
        {
            DarkPlayers.RemoveAll(x => x.Id == id);
        }
        OnPlayerLobbyChange();
    }

    public void SwapPlayerTeam(int id)
    {
        var player = FindPlayer(id);
        RemovePlayer(id);
        player.Dark = !player.Dark;
        if (player.Dark)
        {
            DarkPlayers.Add(player);
        }
        else
        {
            LightPlayers.Add(player);
        }
        OnPlayerLobbyChange();
    }


    //Botão disabled/enabled
    public bool CanStartGame()
    {
        Assert.IsNotNull(LocalPlayer, "Deve setar o LocalPlayer!!");
        return LightPlayers.Count == DarkPlayers.Count && LightPlayers.Count >= 1;
    }

    public void StartGame()
    {
        //Assert.IsTrue(CanStartGame(), "Não é possível começar o jogo!");
        GameStarted = true;
        OnGameStart();
    }

    #endregion

    #region INGAME_INTEGRATION

    public void UpdateInGame(float deltaTime)
    {
        if(GameStarted && !GameFinished)
        {
            CurrentTime += deltaTime;
            if(CurrentTime > GameDuration && getWinnerTeam() != WinnerTeam.NONE)
            {
                GameFinished = true;
                OnGameOver();
            }
        }
        
    }

    public void TurnLight(bool on)
    {
        if(on)
        {
            LightsOn++;
            LightsOff--;
        } 
        else
        {
            LightsOn--;
            LightsOff++;
        }
        OnChangeScore();
    }

    public bool PlayerAttack(int id)
    {
        var player = FindPlayer(id);
        if(player == null)
        {
            return false;
        }
        var result = player.Attack(this);
        OnPlayerAttack(player);
        return result;
    }

    #endregion

    #region ENDGAME_INTEGRATION

    public WinnerTeam getWinnerTeam()
    {
        if(LightsOn > LightsOff)
        {
            return WinnerTeam.LIGHT;
        } 
        else if(LightsOff > LightsOn)
        {
            return WinnerTeam.DARK;
        }
        else
        {
            return WinnerTeam.NONE;
        }
    }

    #endregion

    #region EVENTS

    public event OnEventHandler OnPlayerLobbyChange;
    public event OnEventHandler OnGameStart;
    public event OnEventHandler OnChangeScore;
    public event OnEventHandler OnGameOver;
    public event OnPlayerEventHandler OnPlayerAttack;

    #endregion
}

public delegate void OnEventHandler();
public delegate void OnPlayerEventHandler(GameManagerPlayer player);
//public delegate void OnPlayerLobbyHandler();
//public delegate void OnGameOverHandler();
