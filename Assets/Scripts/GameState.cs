using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class GameState : EntityBehaviour<IGameState>
{

    #region SINGLETON
    public static GameState Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public override void Attached()
    {
        if(entity.IsOwner)
        {
            state.GameDuration = 60.0f;
            state.TotalCrystals = 20;
            state.AttackCooldown = 0.8f;
            Ready1 = true; Ready2 = true;
        }

        state.AddCallback("NextPlayerId", OnUpdateNextPlayerId);
        state.AddCallback("TeamBalanceCount", OnUpdateTeamBalanceCount);

        //var e = PlayerEnter.Create();
        //e.Dark = dark;
        //e.Player = playerEntity;
        //e.Nickname = nickname;
        //e.Send();

        //Debug.LogWarning("GameState criado! IsServer " + BoltNetwork.IsServer + " IsOwner " + entity.IsOwner + " TotalCrystal " + state.TotalCrystals);
    }

    private bool Ready1;
    private bool Ready2;
    public bool IsReady()
    {
        return Ready1 && Ready2;
    }

    public void OnUpdateNextPlayerId()
    {
        Ready1 = true;
        //Debug.LogWarning("Update next player id!!!" + state.NextPlayerId);
        if (BasicCharacter.Local != null && BasicCharacter.Local.Entered == false && IsReady())
        {
            BasicCharacter.Local.Enter();
        }
    }

    public void OnUpdateTeamBalanceCount()
    {
        Ready2 = true;
        //Debug.LogWarning("Update TeamBalanceCount!!!" + state.TeamBalanceCount);
        if (BasicCharacter.Local != null && BasicCharacter.Local.Entered == false && IsReady())
        {
            BasicCharacter.Local.Enter();
        }
    }

    private void Start()
    {

        //Debug.LogWarning("GameState Start! IsServer " + BoltNetwork.IsServer + " IsOwner " + entity.IsOwner + " TotalCrystal " + state.TotalCrystals);
    }

  


    //private int NextPlayerId = 1;
    //private Dictionary<BoltEntity, ICustomStatePlayer> lookupTable = new Dictionary<string, Action>();



    public int CountDarkPlayers()
    {
        int count = 0;
        foreach (var inst in state.DarkPlayers)
        {
            if (inst != null) count++;
        }
        return count;
    }
    public int CountLightPlayers()
    {
        int count = 0;
        foreach (var inst in state.LightPlayers)
        {
            if (inst != null) count++;
        }
        return count;
    }

    public bool IsNextPlayerDark()
    {
        return state.TeamBalanceCount > 0;
    }
    public void ServerSpawnPlayer(BoltEntity playerEntity, bool dark)
    {

        state.NextPlayerId += 1;
        if (dark)
        {
            state.TeamBalanceCount -= 1;
            for (var i = 0; i < state.DarkPlayers.Length; i++)
            {
                if(state.DarkPlayers[i] == null)
                {
                    state.DarkPlayers[i] = playerEntity;
                }
            }
        }
        else
        {
            state.TeamBalanceCount += 1;
            for (var i = 0; i < state.LightPlayers.Length; i++)
            {
                if (state.LightPlayers[i] == null)
                {
                    state.LightPlayers[i] = playerEntity;
                }
            }
        }
    }

    public void ServerRemovePlayer(BoltEntity player)
    {
        for (var i = 0; i < state.DarkPlayers.Length; i++)
        {
            if (state.DarkPlayers[i] == player)
            {
                state.DarkPlayers[i] = player;
            }
        }
        for (var i = 0; i < state.LightPlayers.Length; i++)
        {
            if (state.LightPlayers[i] == player)
            {
                state.LightPlayers[i] = player;
            }
        }
    }


}
