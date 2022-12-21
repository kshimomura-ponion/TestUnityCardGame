using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateChangeFlg
{
    bool IsStateChanged { get; }

    void SetStateChanged(bool changed = true);
}
