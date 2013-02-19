using UnityEngine;
using System.Collections;
using System;
using System.Text;
using Funcom.Lego.Thrift.Generated;

public class GameState 
{
    public delegate void WorldInitialized();
    public event WorldInitialized WorldInit;

    public delegate void BrickUpdated(int x, int y, BrickColor color);
    public event BrickUpdated BrickUpdate;

    private WorldInfo worldInfo;
    public bool Dirty { get; set; }

    public WorldInfo WorldInfo 
    {
        get 
        { 
            return worldInfo; 
        } 
        set
        {
            worldInfo = value;
            Dirty = true;
        }
    }

    public int Rows { get { return WorldInfo.Rows; } }
    public int Columns { get { return WorldInfo.Columns; } }

    public GameState()
    {
        Dirty = false;
    }

    public BrickColor GetBrick(int column, int row)
    {
        return WorldInfo.Bricks[column + row * Rows];
    }

    public void SetBrick(int column, int row, BrickColor color)
    {
        if (column.Between(0, Columns) && row.Between(0, Rows))
        {
            WorldInfo.Bricks[column + row * Rows] = color;
            var sb = new StringBuilder();
            WorldInfo.Bricks.ForEach(b => sb.Append((int)b));
            Debug.Log(sb.ToString());
        }
    }

    public void UpdateBrick(int column, int row, BrickColor color)
    {
        WorldInfo.Bricks[column + row * Rows] = color;
        Dirty = true;
        Debug.Log(WorldInfo.Bricks);
    }

}
