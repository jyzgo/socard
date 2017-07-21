using UnityEngine;
using System.Collections;

public class CardPlatformRootTest : CardPlatformRoot {


    public override bool isCardPutable(CardAbstract card)
    {
        return true; 
    }
    readonly Vector3 nexp = new Vector3(0, 0, -0.1f);

    public override Vector3 GetNextPos(int i = 1)
    {
        //if (i == 1)
        //{
        //    return transform.position + nexp;
        //}
        return base.GetNextPos(i - 1) + nexp;

    }

    public override void Start()
    {
        base.Start();
    }

}
