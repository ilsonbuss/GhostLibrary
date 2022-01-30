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
        Debug.LogWarning($"Server received event CrystalHit - State:" + (e.HitState ? "Light":"Dark"));
        GameState.Instance.ServerComputeLight(e.HitState);
    }


    public void SpawnLights()
    {
        Debug.Log("Spawning Lights --------------------------------------------------------------");

        var LightSpawners = GameObject.FindGameObjectsWithTag("LightSpawnerMarker");
        var positions = LightSpawners.Select(l => new
        {
            randomOrder = Random.Range(0, LightSpawners.Length),
            position = l.transform.position
        })
            .OrderBy(a => a.randomOrder)
            .Take(4)
            .Select(a => a.position);

        foreach (var position in positions)
        {
            BoltNetwork.Instantiate(BoltPrefabs.Crystal, position, Quaternion.identity);
        }
    }
}
