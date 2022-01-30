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
        //SpawnLights();
    }

    public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
    {
        //SpawnLights();
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
    }

    public override void OnEvent(PlayerLeave e)
    {
        GameState.Instance.ServerRemovePlayer(e.Dark);

        Debug.LogWarning("PlayerLeave Dark: " + e.Dark);
    }

    public override void OnEvent(CrystalHit e)
    {
        var lm = e.CrystalInstance.gameObject.GetComponent<LightManager>();
        if(lm.isActiveLocal == e.HitState)
        {
            return;
        }
        lm.SetState(e.HitState);
        //lm.state.IsActive = e.HitState;

        Debug.LogWarning($"Server received event CrystalHit - State:" + (e.HitState ? "Light" : "Dark"));
        GameState.Instance.ServerComputeLight(e.HitState);
    }

}
