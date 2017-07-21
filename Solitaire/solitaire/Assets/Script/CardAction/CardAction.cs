using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CardAction  {
    public abstract void DoAction();
    public abstract void ReverseAction();
    public abstract void ResetPos();

    public void ResetListPos(List<Vector3> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var oldPos = list[i];
            list[i] = new Vector3(oldPos.x * -1, oldPos.y, oldPos.z);

        }
    }

}
