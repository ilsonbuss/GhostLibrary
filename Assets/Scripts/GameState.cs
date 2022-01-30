using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using System.Linq;

public class GameState : EntityBehaviour<IGameState>
{

    #region SINGLETON
    public static GameState Instance;

    private void Awake()
    {
        Instance = this;
        GameStartTime = BoltNetwork.ServerTime; //guarda o tempo da sessão quando o jogo iniciou

    }
    #endregion

    public float GameStartTime;
    public float MaxGameTime;
    public enum Team { NONE, LIGHT, DARK }

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            MaxGameTime = 60f;
            state.GameStarted = true;
            state.TotalCrystals = 8;
            state.AttackCooldown = 0.8f;
            state.LightsOn = state.TotalCrystals / 2;
            Ready1 = true; Ready2 = true;
            SpawnLights();
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

    public void SpawnLights()
    {
        //Debug.Log("Spawning Lights --------------------------------------------------------------");

        var LightSpawners = GameObject.FindGameObjectsWithTag("LightSpawnerMarker");

        var positionsUnique = LightSpawners.Select(l => l.transform.position).ToList();
        var positionsFinal = new List<Vector3>();
        while (positionsFinal.Count < state.TotalCrystals)
        {
            var randIndex = Random.Range(0, positionsUnique.Count);
            if (LightSpawners[randIndex] != null)
            {
                if (!positionsFinal.Contains(positionsUnique[randIndex]))
                {
                    positionsFinal.Add(positionsUnique[randIndex]);
                }
            }
        }

        //spawn active crystals
        foreach (var position in positionsFinal.Take(positionsFinal.Count / 2))
        {
            var entity = BoltNetwork.Instantiate(BoltPrefabs.Crystal, position, Quaternion.identity);
            var lightManager = entity.gameObject.GetComponent<LightManager>();
            if (lightManager != null)
            {
                lightManager.SetInitState(true);
            }
        }

        //spawn black crystals
        foreach (var position in positionsFinal.Skip(positionsFinal.Count / 2))
        {
            var entity = BoltNetwork.Instantiate(BoltPrefabs.Crystal, position, Quaternion.identity);
            var lightManager = entity.gameObject.GetComponent<LightManager>();
            if (lightManager != null)
            {
                lightManager.SetInitState(false);
            }
        }

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
                if (state.DarkPlayers[i] == null)
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

    public void ServerRemovePlayer(bool dark)
    {
        if (dark)
        {
            state.TeamBalanceCount += 1;
        }
        else
        {
            state.TeamBalanceCount -= 1;
        }
        //for (var i = 0; i < state.DarkPlayers.Length; i++)
        //{
        //    if (state.DarkPlayers[i] == player)
        //    {
        //        state.DarkPlayers[i] = player;
        //    }
        //}
        //for (var i = 0; i < state.LightPlayers.Length; i++)
        //{
        //    if (state.LightPlayers[i] == player)
        //    {
        //        state.LightPlayers[i] = player;
        //    }
        //}
    }

    /// <summary>
    /// Finaliza o jogo pois o tempo acabou
    /// </summary>
    public void EndGame()
    {
        state.GameFinished = true;

        //verifica vencedor
        if ((state.TotalCrystals - state.LightsOn) > state.LightsOn)
        {
            state.WinnerTeam = (int)Team.DARK;
        }
        else
        {
            state.WinnerTeam = (int)Team.LIGHT;
        }
    }


    public void ServerComputeLight(bool lightStateOn)
    {
        if (lightStateOn)
        {
            state.LightsOn++;
        }
        else
        {
            state.LightsOn--;
        }
    }

}
