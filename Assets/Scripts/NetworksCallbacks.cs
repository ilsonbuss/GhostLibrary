using Bolt.Samples.Photon.Lobby.Utilities;
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
    [SerializeField] private SceneField lobbyScene;

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
        yield return new WaitForSeconds(8);

        EndGameSwitchScene();
    }

    private void EndGameSwitchScene()
    {
        //if the game is over and the video has finished
        //then we should return to lobby
        //Debug.LogWarning("EndGame");
        if (waitEndState && GameState.Instance.state.GameFinished)
        {
            //Debug.LogWarning("Changed");
            //Application.LoadLevel(0);
            //SceneManager.LoadScene(0);
            System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
            Application.Quit(); //kill current process
            //SceneManager.LoadScene(lobbyScene.SimpleSceneName);
        }
    }

    public override void OnEvent(PlayerEnter e)
    {
        if (e == null) return;

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

        Debug.LogWarning("PlayerEnter " + e.Nickname + " Dark: " + e.Dark + " IsOwner: " + e.Player.IsOwner);

        //GameState.Instance.ServerSpawnPlayer(e.Player, e.Dark);
        //Debug.LogWarning("PlayerEnter " + e.Nickname + " Dark: " + e.Dark);
    }

    public override void OnEvent(PlayerAtackEvent e)
    {
        if (e.Victim == null)
        {
            return;
        }

        if (e.Victim.IsOwner)
        {
            var victmy = e.Victim.gameObject.TryGetComponent<BasicCharacter>(out var entity) ? entity : null;
            victmy?.Respawn();
        }
    }

    //public override void OnEvent(CrystalHit e)
    //{
    //    if (e != null &&
    //        e.CrystalInstance != null && 
    //        e.CrystalInstance.gameObject.TryGetComponent(out LightManager lightManager) &&
    //        e.FromSelf == false)
    //    {
    //        lightManager.ActivateCallBack(e.HitState);
    //    }
    //}
//    public override void OnEvent(CrystalHit e)
//    {
//        if (e != null &&
//            e.CrystalInstance != null && 
//            e.CrystalInstance.gameObject.TryGetComponent(out LightManager lightManager))
////            e.FromSelf == false)
//        {
//            lightManager.ActivateCallBack(e.HitState);
//        }
//    }


}
