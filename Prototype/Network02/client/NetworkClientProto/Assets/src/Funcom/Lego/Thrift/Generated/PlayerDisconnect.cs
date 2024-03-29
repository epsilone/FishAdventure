/**
 * Autogenerated by Thrift Compiler (0.9.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace Funcom.Lego.Thrift.Generated
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class PlayerDisconnect : TBase
  {
    private string _playerId;

    public string PlayerId
    {
      get
      {
        return _playerId;
      }
      set
      {
        __isset.playerId = true;
        this._playerId = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool playerId;
    }

    public PlayerDisconnect() {
    }

    public void Read (TProtocol iprot)
    {
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.String) {
              PlayerId = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          default: 
            TProtocolUtil.Skip(iprot, field.Type);
            break;
        }
        iprot.ReadFieldEnd();
      }
      iprot.ReadStructEnd();
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("PlayerDisconnect");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (PlayerId != null && __isset.playerId) {
        field.Name = "playerId";
        field.Type = TType.String;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(PlayerId);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("PlayerDisconnect(");
      sb.Append("PlayerId: ");
      sb.Append(PlayerId);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
