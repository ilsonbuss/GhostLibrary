using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class NetworksCallbacks : GlobalEventListener
{
    public GameObject cubePrefab;
    public GameObject endGameCanvasLight;
    public GameObject endGameCanvasDark;
    private bool waitEndState;
    private VideoPlayer endGameVideoPlayer;

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        var spawnPos = new Vector3(Random.Range(-8, 8), 0, Random.Range(-8, 8));
        BoltNetwork.Instantiate(cubePrefab, spawnPos, Quaternion.identity);
    }

    public void Update()
    {
        //if game ended show screen of winner
        if (GameState.Instance != null && GameState.Instance.state.GameFinished && !waitEndState)
        {
            waitEndState = true;

            if (GameState.Instance.state.WinnerTeam == (int)GameState.Team.LIGHT)
            {
                endGameCanvasLight.SetActive(true);
                endGameVideoPlayer = endGameCanvasLight.GetComponentsInChildren<VideoPlayer>().FirstOrDefault();
            }
            else
            {
                endGameCanvasDark.SetActive(true);
                endGameVideoPlayer = endGameCanvasLight.GetComponentsInChildren<VideoPlayer>().FirstOrDefault();
            }

            StartCoroutine(nameof(WaitVideoFinish));
        }
    }


    private IEnumerator WaitVideoFinish()
    {
        endGameVideoPlayer.Play();

        while (endGameVideoPlayer.isPlaying)
        {
            yield return null;
        }

        //we need to wait a bit more than "usual" because the video flag is setted
        //1 or 2 seconds before the screen is actually updated
        yield return new WaitForSeconds(7);

        EndGameSwitchScene();
    }

    private void EndGameSwitchScene()
    {
        //if the game is over and the video has finished
        //then we should return to lobby
        if (waitEndState && GameState.Instance.state.GameFinished)
        {
            SceneManager.LoadScene("PhotonLobby", LoadSceneMode.Single);
        }
    }

    public override void OnEvent(PlayerEnter e)
    {
        var lightGhost = e.Player.gameObject.transform.Find("LightGhost").gameObject;
        var darkGhost = e.Player.gameObject.transform.Find("DarkGhost").gameObject;
        if (e.Dark)
        {
            //Debug.Log("Destroy lite!" + lightGhost.tag + " _  local: " + e.Player.IsOwner);
            darkGhost.SetActive(true);
            e.Player.gameObject.GetComponent<BasicCharacter>().ghost = darkGhost;
            //Destroy(lightGhost);
            
            
        }
        else
        {
            //Debug.Log("Destroy dark! " + darkGhost.tag + " _ " + " _  local: " + e.Player.IsOwner);
            lightGhost.SetActive(true);
            e.Player.gameObject.GetComponent<BasicCharacter>().ghost = lightGhost;
            //Destroy(darkGhost);
            
        }

        //GameState.Instance.ServerSpawnPlayer(e.Player, e.Dark);
        //Debug.LogWarning("PlayerEnter " + e.Nickname + " Dark: " + e.Dark);
    }


}
