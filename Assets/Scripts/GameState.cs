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
        
        Debug.LogWarning("GameState criado! IsServer " + BoltNetwork.IsServer + " IsOwner " + entity.IsOwner + " TotalCrystal " + state.TotalCrystals);
    }

    private void Start()
    {
        Debug.LogWarning("GameState Start! IsServer " + BoltNetwork.IsServer + " IsOwner " + entity.IsOwner + " TotalCrystal " + state.TotalCrystals);
    }

    public void ServerSpawnPlayer(BoltEntity entity)
    {
        state.GM = entity;
        Debug.LogWarning("Ae " + state.GM);
    }



}
