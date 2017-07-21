using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MTUnity.Actions;

public class CardLoader : CardAbstract {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public List<CardEdit> _list = new List<CardEdit>();
    public void AddCard(CardEdit card)
    {
        _list.Add(card);
        RefreshList();
    }

    public void RemoveCard(CardEdit card)
    {
        _list.Remove(card);
        RefreshList();

    }

    void RefreshList()
    {
        for(int i = 0; i < _list.Count; i ++)
        {
            var card = _list[i];
            card.transform.position = transform.position + (i+1) * new Vector3(0, -0.4f, -0.1f);
        }
    }

    public override bool IsPutAble()
    {
        return true;
    }
    public override bool isCardPutable(CardAbstract card)
    {
        return nextCard == null ;
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

    public override void PutCard(CardAbstract otherCard)
    {
        nextCard = otherCard;

        otherCard.cardState = CardState.InPlatform;
        otherCard.transform.parent = transform;
        if(otherCard.preCard != null)
        {
            otherCard.preCard.transform.rotation = Quaternion.identity;
            otherCard.preCard.nextCard = null;
        }

        otherCard.preCard = this;
        otherCard.RunActions(new MTMoveToWorld(0.1f, GetNextPos()));
    }


}
