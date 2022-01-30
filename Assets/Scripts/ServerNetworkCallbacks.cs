using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Game")]
public class ServerNetworkCallbacks : GlobalEventListener
{
    [HideInInspector]
    public bool hasInitialized;

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (hasInitialized)
            return;

        hasInitialized = true;
        var spawnPos = new Vector3(Random.Range(-8, 8), 0, Random.Range(-8, 8));

        BoltNetwork.Instantiate(BoltPrefabs.GameState, spawnPos, Quaternion.identity);
        SpawnLights();
    }

    public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
    {
        //Debug.LogWarning("Remote Aqui mano");

        SpawnLights();
    }

    //update routine on server
    public void Update()
    {
        //Debug.Log($"Time:{BoltNetwork.ServerTime - GameState.Instance.GameStartTime}");

        if (GameState.Instance != null &&
            (BoltNetwork.ServerTime - GameState.Instance.GameStartTime >= GameState.Instance.MaxGameTime) &&
            GameState.Instance.state.GameFinished == false)
        {
            GameState.Instance.EndGame();
        }
    }




    public override void OnEvent(PlayerEnter e)
    {
        GameState.Instance.ServerSpawnPlayer(e.Player, e.Dark);
        Debug.LogWarning("PlayerEnter " + e.Nickname + " Dark: " + e.Dark);
    }

    public override void OnEvent(PlayerLeave e)
    {
        GameState.Instance.ServerRemovePlayer(e.Dark);

        Debug.LogWarning("PlayerLeave Dark: " + e.Dark);
    }

    public override void OnEvent(CrystalHit e)
    {
        Debug.LogWarning($"Server received event CrystalHit - State:" + (e.HitState ? "Light" : "Dark"));
        GameState.Instance.ServerComputeLight(e.HitState);
    }


    public void SpawnLights()
    {
        //Debug.Log("Spawning Lights --------------------------------------------------------------");

        var LightSpawners = GameObject.FindGameObjectsWithTag("LightSpawnerMarker");
        var positions = LightSpawners.Select(l => new
        {
            randomOrder = Random.Range(0, LightSpawners.Length),
            position = l.transform.position
        }).OrderBy(a => a.randomOrder)
          .Take(6)
          .Select(a => a.position);


        //spawn active crystals
        foreach (var position in positions.Take(3))
        {
            var entity = BoltNetwork.Instantiate(BoltPrefabs.Crystal, position, Quaternion.identity);
            var lightManager = entity.gameObject.GetComponent<LightManager>();
            if (lightManager != null)
            {
                lightManager.initState = true;
            }
        }

        //spawn black crystals
        foreach (var position in positions.Skip(3))
        {
            var entity = BoltNetwork.Instantiate(BoltPrefabs.Crystal, position, Quaternion.identity);
        }

    }
}
