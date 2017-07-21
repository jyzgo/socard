using UnityEngine;
using System.Collections;

public class CardPlatformRoot : CardAbstract {

    public override bool isCardPutable(CardAbstract card)
    {
        return nextCard == null && card.CardNum == 13;
    }
    readonly Vector3 nexp = new Vector3(0, 0, -0.1f);

    public override Vector3 GetNextPos(int i = 1)
    {
        //if (i == 1)
        //{
        //    return transform.position + nexp;
        //}
        return base.GetNextPos(i-1) + nexp ;

    }

    public override void Start()
    {
        base.Start();
    }


}
