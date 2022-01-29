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
        }


        //var e = PlayerEnter.Create();
        //e.Dark = dark;
        //e.Player = playerEntity;
        //e.Nickname = nickname;
        //e.Send();

        //Debug.LogWarning("GameState criado! IsServer " + BoltNetwork.IsServer + " IsOwner " + entity.IsOwner + " TotalCrystal " + state.TotalCrystals);
    }

    private void Start()
    {
        if (BasicCharacter.Local != null && BasicCharacter.Local.Entered == false)
        {
            BasicCharacter.Local.Enter();
        }

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
        return CountDarkPlayers() < CountLightPlayers();
    }
    public void ServerSpawnPlayer(BoltEntity playerEntity, bool dark)
    {

        state.NextPlayerId += 1;
        if (dark)
        {
            for(var i = 0; i < state.DarkPlayers.Length; i++)
            {
                if(state.DarkPlayers[i] == null)
                {
                    state.DarkPlayers[i] = playerEntity;
                }
            }
        }
        else
        {
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
