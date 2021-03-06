using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using System;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

public class Menu : GlobalEventListener
{
    public void StartServer()
    { 
        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        BoltMatchmaking.CreateSession("testeDev", sceneToLoad: "Game");
        //BoltMatchmaking.CreateSession("xxxxTestexxxx", sceneToLoad: "TestScene");
    }

    public void StartClient()
    {
        BoltLauncher.StartClient();
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            if (photonSession.Source == UdpSessionSource.Photon)
            {
                BoltMatchmaking.JoinSession(photonSession);
            }
        }
    }
}
