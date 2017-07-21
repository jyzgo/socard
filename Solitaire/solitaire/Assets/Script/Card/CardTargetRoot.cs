using UnityEngine;
using System.Collections;

public class CardTargetRoot : CardAbstract
{

    void Awake()
    {
        cardState = CardState.InTarget;
    }

    public override void Start()
    {
        base.Start();
    }

    public override bool isCardPutable(CardAbstract card)
    {
        if (nextCard == null && card.CardNum == 1)
        {
            return true;
        }
        return false;
    }

    readonly Vector3 nexp = new Vector3(0, 0, -0.1f);
    public override Vector3 GetNextPos(int i = 1)
    {
        return transform.position + nexp * i;

    }
}
