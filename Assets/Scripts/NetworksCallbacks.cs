using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworksCallbacks : GlobalEventListener
{
    public GameObject cubePrefab;
    public GameObject gameStatePrefab;
    public GameObject lightPrefab;

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        var spawnPos = new Vector3(Random.Range(-8, 8), 0, Random.Range(-8, 8));


        if (BoltNetwork.IsServer)
        {
            BoltNetwork.Instantiate(gameStatePrefab, spawnPos, Quaternion.identity);
            SpawnLights();
        }

        BoltNetwork.Instantiate(cubePrefab, spawnPos, Quaternion.identity);
    }

    public override void OnEvent(PlayerEnter e)
    {
        //e.Player.gameObject

        if(BoltNetwork.IsServer)
        {
            GameState.Instance.ServerSpawnPlayer(e.Player, e.Dark);
        }

        Debug.LogWarning("PlayerEnter " + e.Nickname + " Dark: " + e.Dark);
    }

    public override void OnEvent(PlayerSwap e)
    {
        //Not implemented
    }


    public override void OnEvent(PlayerLeave e)
    {
        if (BoltNetwork.IsServer)
        {
            GameState.Instance.ServerRemovePlayer(e.player);
        }

        Debug.LogWarning("PlayerLeave " + e.player);
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
            BoltEntity.Instantiate(lightPrefab, position, Quaternion.identity);
        }
    }

}
