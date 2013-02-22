using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Text;
using com.funcom.lego.generated;

public class PlayerScript : MonoBehaviour
{
    public GameState GameState { get; private set; }

    # region Unity Specifics

    public GameObject prefab;
    public string playerName;
    private Dictionary<Vector3, GameObject> bricks = new Dictionary<Vector3, GameObject>();

    void OnEnable()
    {
        GameState = new GameState();
        GameState.Name = playerName;
        GameState.WorldInit += new GameState.WorldInitialized(OnWorldInit);
        GameState.BrickUpdate += new GameState.BrickUpdated(OnBrickUpdated);
    }

    void OnDisable()
    {
        GameState.WorldInit -= OnWorldInit;
        GameState.BrickUpdate -= OnBrickUpdated;
    }

    // Update is called once per frame
    void Update () 
    {
        var leftMouseButtonUp = Input.GetMouseButtonUp(0);
        if (leftMouseButtonUp)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("clicked at (x=" + mousePosition.x + ",y=" + mousePosition.y + ",z=" + mousePosition.z + ")");
            //Debug.Log("" + GameState.GetBrick((int)mousePosition.x, (int)mousePosition.y));
            var position = new Vector3((int)mousePosition.x, (int)mousePosition.y, 0);
            GameState.SetBrick((int)position.x, (int)position.y);
            GameObject go;
            if (bricks.TryGetValue(position, out go))
            {
                go.renderer.material.color = Convert(GameState.Color);

                var message = new BrickUpdate();
                message.Row = (int)position.x;
                message.Column = (int)position.y;
                message.Color = GameState.Color;

                var networkManager = GameObject.Find("NetworkManager");
                networkManager.GetComponent<LegoNetwork>().Add(MessageId.BRICK_UPDATE, message);
            }
        }

        if (GameState.Dirty)
        {
            if (bricks.Count == 0)
            {
                for (int i = 0; i < GameState.Rows; ++i)
                {
                    for (int j = 0; j < GameState.Columns; ++j)
                    {
                        var position = new Vector3(i, j, 0);
                        var go = Instantiate(prefab, position, prefab.transform.rotation) as GameObject;
                        go.renderer.material.color = Convert(GameState.GetBrick(i, j));
                        bricks[position] = go;
                    }
                }
            }
            else
            {
                for (int i = 0; i < GameState.Rows; ++i)
                {
                    for (int j = 0; j < GameState.Columns; ++j)
                    {
                        var position = new Vector3(i, j, 0);
                        GameObject go;
                        if (bricks.TryGetValue(position, out go))
                        {
                            go.renderer.material.color = Convert(GameState.GetBrick(i, j));
                        }
                    }
                }
            }

            GameState.Dirty = false;
        }
    }

    private static Color Convert(BrickColor color)
    {
        switch (color)
        {
            case BrickColor.RED: return Color.red;
            case BrickColor.BLUE: return Color.blue;
            case BrickColor.GREEN: return Color.green;
            case BrickColor.YELLOW: return Color.yellow;
            default: return Color.white;
        }
    }

    # endregion

    public void OnWorldInit()
    {
        Debug.Log("OnWorldInit");
        for (int i = 0; i < GameState.Rows; ++i)
        {
            for (int j = 0; j < GameState.Columns; ++j)
            {
                var position = new Vector3(i, j, 0);
                var go = Instantiate(prefab, position, prefab.transform.rotation) as GameObject;
                bricks[position] = go;
            }
        }
    }

    public void OnBrickUpdated(int x, int y, BrickColor color)
    {
        var position = new Vector3(x, y, 0);
        GameObject go;
        if (bricks.TryGetValue(position, out go))
        {
            go.renderer.material.color = Convert(GameState.Color);
        }
    }
}
