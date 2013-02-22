using UnityEngine;
using System.Collections;
using System.IO;
using Thrift.Transport;
using Thrift.Protocol;
using com.funcom.lego.generated;
using System;

public class NetworkDataHandler : IThriftHandler
{
    private GameState GameState { get; set;  }

    public NetworkDataHandler(GameState gameState)
    {
        GameState = gameState;
    }

    public void Handle(byte[] raw)
    {
        //Debug.Log("Handling data from server" + BitConverter.ToString(raw).Replace("-", string.Empty));
        using (var stream = new MemoryStream(raw))
        {
            var transport = new TStreamTransport(stream, null);
            var protocol = new TBinaryProtocol(transport);
            int count = protocol.ReadI32();
            MessageId[] messageIds = new MessageId[count];
            for (int i = 0; i < count; ++i)
            {
                messageIds[i] = (MessageId)protocol.ReadByte();
            }

            for (int i = 0; i < count; ++i)
            {
                var messageId = messageIds[i];
                switch (messageId)
                {
                    case MessageId.PLAYER_CONNECTED:
                        var playerConnected = new PlayerConnected();
                        playerConnected.Read(protocol);
                        OnPlayerConnected(playerConnected);
                        break;

                    case MessageId.WORLD_INFO:
                        var worldInfo = new WorldInfo();
                        worldInfo.Read(protocol);
                        OnWorldInfo(worldInfo);
                        break;

                    case MessageId.BRICK_UPDATE:
                        var brickUpdate = new BrickUpdate();
                        brickUpdate.Read(protocol);
                        OnBrickUpdate(brickUpdate);
                        break;

                    default:
                        Debug.LogError("Unexpected type from server: " + messageId);
                        break;
                }
            }
        }
    }

    private void OnPlayerConnected(PlayerConnected playerConnected)
    {
        Debug.Log("Player Connected " + playerConnected);
        if (GameState.Name.Equals(playerConnected.Name))
        {
            GameState.Color = playerConnected.Color;
        }
        else
        {
            Debug.Log("Player connected: " + playerConnected.Name + " with Color: " + playerConnected.Color);
        }
    }

    private void OnWorldInfo(WorldInfo worldInfo)
    {
        Debug.Log("WorldInfo received " + worldInfo);
        GameState.WorldInfo = worldInfo;
    }

    private void OnBrickUpdate(BrickUpdate brickUpdate)
    {
        GameState.UpdateBrick(brickUpdate.Row, brickUpdate.Column, brickUpdate.Color);
    }
}
