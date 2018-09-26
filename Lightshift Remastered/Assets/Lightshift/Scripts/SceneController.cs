using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public Scenes scene;

    private void Start()
    {
        ChangeScene(scene);
    }

    public static void ChangeScene(Scenes scene)
    {
        switch (scene)
        {
            case Scenes.Authentication:
                SceneManager.LoadScene("_AUTHENTICATION_");
                break;
            case Scenes.ServerConnector:
                SceneManager.LoadScene("_SERVERCONNECTOR_");
                break;
            case Scenes.Game:
                SceneManager.LoadScene("_GAME_");
                break;
            case Scenes.GameServer:
                SceneManager.LoadScene("_GAMESERVER_");
                break;
            case Scenes.Spawner:
                SceneManager.LoadScene("_SPAWNER_");
                break;
        }

    }
}

public enum Scenes
{
    Authentication,
    ServerConnector,
    Game,
    GameServer,
    Spawner,
}