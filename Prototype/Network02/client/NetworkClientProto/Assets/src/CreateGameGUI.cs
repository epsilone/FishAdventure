using UnityEngine;
using System.Collections;
using Funcom.Lego.Thrift.Generated;
using System;

public class CreateGameGUI : MonoBehaviour 
{
    private string gameName;
    private string playerCount;

    // Use this for initialization
    void Start () 
    {
        gameName = "";
        playerCount = "";
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Game name: ");
        gameName = GUILayout.TextField(gameName, 150, GUILayout.MinWidth(150));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Player count: ");
        playerCount = GUILayout.TextField(playerCount, 25, GUILayout.MinWidth(25));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Create"))
        {
            GameApp.Instance.MessageId = MessageId.CREATE_GAME;
            var createGame = new CreateGame();
            createGame.GameName = gameName;
            int maxPlayers = 0;
            if (!Int32.TryParse(playerCount, out maxPlayers))
            {
                maxPlayers = 4;
            }
            createGame.MaxPlayers = maxPlayers;
            createGame.PlayerName = PlayerPrefs.GetString(Constants.PLAYER_NAME);
            createGame.Visible = true;
            GameApp.Instance.MessageId = MessageId.CREATE_GAME;
            GameApp.Instance.Message = createGame;
            GameApp.Instance.doTransition(GameApp.Scene.GamePlay);
        }
    }
}
