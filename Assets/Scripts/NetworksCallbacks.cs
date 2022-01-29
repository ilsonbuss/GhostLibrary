using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworksCallbacks : GlobalEventListener
{
    public GameObject cubePrefab;

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        var spawnPos = new Vector3(Random.Range(-8, 8), 0, Random.Range(-8, 8));

        BoltNetwork.Instantiate(cubePrefab, spawnPos, Quaternion.identity);
    }
}
