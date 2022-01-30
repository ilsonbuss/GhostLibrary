using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworksCallbacks : GlobalEventListener
{
    public GameObject cubePrefab;

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        var spawnPos = new Vector3(Random.Range(-8, 8), 0, Random.Range(-8, 8));
        BoltNetwork.Instantiate(cubePrefab, spawnPos, Quaternion.identity);
    }

    public void Update()
    {
        //if game ended show screen of winner
        if (GameState.Instance.state.GameFinished)
        {

        }
    }

    public override void OnEvent(PlayerEnter e)
    {
        var lightGhost = e.Player.gameObject.transform.Find("LightGhost").gameObject;
        var darkGhost = e.Player.gameObject.transform.Find("DarkGhost").gameObject;
        if(e.Dark)
        {
            Debug.Log("Destroy lite!" + lightGhost.tag + " _  local: " + e.Player.IsOwner);
            Destroy(lightGhost);
            darkGhost.SetActive(true);
        }
        else
        {
            Debug.Log("Destroy dark! " + darkGhost.tag + " _ " + " _  local: " + e.Player.IsOwner);
            Destroy(darkGhost);
            lightGhost.SetActive(true);
        }

        //GameState.Instance.ServerSpawnPlayer(e.Player, e.Dark);
        //Debug.LogWarning("PlayerEnter " + e.Nickname + " Dark: " + e.Dark);
    }

}
