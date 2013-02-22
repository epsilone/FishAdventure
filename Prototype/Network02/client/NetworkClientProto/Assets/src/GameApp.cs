using UnityEngine;
using System.Collections;
using Funcom.Lego.Thrift.Generated;
using Thrift.Protocol;

public class GameApp : MonoBehaviour
{
    #region Constants

    public enum Scene
    {
        None,
        GameSelect,
        GameCreate,
        GameJoin,
        GamePlay,
    }

    #endregion

    #region Singleton

    private static GameApp instance = null;

    public static GameApp Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("GameApp");
                instance = go.AddComponent<GameApp>();
            }

            return instance;
        }
    }

    #endregion

    public Scene PreviousScene { get; set; }
    public Scene CurrentScene { get; set; }

    public MessageId MessageId { get; set; }
    public TBase Message { get; set; }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () 
    {
        Debug.Log("GameApp Started");
        PreviousScene = Scene.None;
        CurrentScene = Scene.GameSelect;
    }
    
    // Update is called once per frame
    void Update () 
    {
    
    }

    public void doTransition(Scene scene)
    {
        switch (scene)
        {
            case Scene.GameSelect:
                break;
            case Scene.GameCreate:
                Application.LoadLevel("CreateNetworkGameScene");
                break;
            case Scene.GameJoin:
                Application.LoadLevel("NetworkClientProto");
                break;
            case Scene.GamePlay:
                Application.LoadLevel("NetworkClientProto");
                break;
        }

        PreviousScene = CurrentScene;
        CurrentScene = scene;
    }
}
