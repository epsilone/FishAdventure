using UnityEngine;
using System.Collections;

public interface IThriftHandler
{
    void Handle(byte[] raw);
}
