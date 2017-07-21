using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MTUnity.Actions;
using System;

public class HintMgr : MonoBehaviour {

    public GameObject CardPrefab;
    HashSet<Card> _unusedCardSet = new HashSet<Card>();
    HashSet<Card> _usedCardSet = new HashSet<Card>();

    public static HintMgr current;
	// Use this for initialization
    void Awake()
    {
        current = this;
        for(int i = 0; i  < 13; i ++)
        {
            var gb = Instantiate<GameObject>(CardPrefab);
            var cardSc = gb.GetComponent<Card>();
            gb.transform.parent = transform;
            gb.transform.localPosition = Vector3.zero;
            var box = gb.GetComponent<BoxCollider2D>();
            box.enabled = false;
            _unusedCardSet.Add(cardSc);
              
        }

    }

    public void StopAllHint()
    {
        var en = _usedCardSet.GetEnumerator();
        while(en.MoveNext())
        {
            var card = en.Current;
            card.transform.parent = transform;
            card.transform.localPosition = Vector3.zero;
            card.transform.eulerAngles = Vector3.zero;
            card.preCard = null;
            card.nextCard = null;
            card.StopAllActions();
            _unusedCardSet.Add(card);
        }
        _usedCardSet.Clear();
    }

    public void ShowFlipHint(CardAbstract moveCard,Vector3 tarPos)
    {
        if(_usedCardSet.Count > 0 )
        {
            return;
        }

        if(LevelMgr.current.GetLevelState() != LevelMgr.LevelState.Playing)
        {
            return;
        }

        var card = GetUnusedHintCard();
        card.transform.eulerAngles = new Vector3(0, 180, 0);
        card.transform.position = moveCard.transform.position;
        var repAction = RepeatHintAct(card, tarPos + Vector3.back * 2f);
        card.RunAction(repAction);

    }
    
    public void ShowMoveHint(CardAbstract moveCard,CardAbstract endCard)
    {
        if(_usedCardSet.Count > 0)
        {
            return;
        }

        var card = GetUnusedHintCard();
        card.cardColor = moveCard.cardColor;
        card.CardNum = moveCard.CardNum;
        card.cardState = moveCard.cardState;
        card.UpdateCardView();
        card.transform.position = moveCard.transform.position + Vector3.back ;

        var nextCard = moveCard.nextCard;
        var curHintCard = card;
        while(nextCard != null)
        {
            var nCard = GetUnusedHintCard();
            nCard.cardColor = nextCard.cardColor;
            nCard.CardNum = nextCard.CardNum;
            nCard.UpdateCardView();
            nCard.transform.parent = curHintCard.transform;
            nCard.cardState = nextCard.cardState;
            nCard.transform.position = curHintCard.GetNextPos();
            
            curHintCard.nextCard = nCard;
            curHintCard = nCard;

            nextCard = nextCard.nextCard;
        }



        var repAction = RepeatHintAct(card, endCard.GetNextPos());
        card.RunAction(repAction);


    }

    public HintText hintText;
    internal void ShowNoMoveText()
    {
        hintText.gameObject.SetActive(true);
        hintText.ShowText("No Moves.");
    }

    Card GetUnusedHintCard()
    {
        Card unusedCard = null;
        if (_unusedCardSet.Count > 0) {
            var en = _unusedCardSet.GetEnumerator();
            while (en.MoveNext())
            {
                unusedCard = en.Current;
                break;
            }
          
        } else
        {
            var gb = Instantiate<GameObject>(CardPrefab);
            unusedCard = gb.GetComponent<Card>();
            gb.transform.parent = transform;
            gb.transform.localPosition = Vector3.zero;

        }
        _unusedCardSet.Remove(unusedCard);
        _usedCardSet.Add(unusedCard);
        unusedCard.BackColor();
        return unusedCard;
    }
    

    public MTAction RepeatHintAct(Card movecard, Vector3 tarPos)
    {
        Vector3 originPos = movecard.transform.position;
        var seq = new MTSequence(new MTMoveToWorld(0.4f, tarPos), 
            new CardFadeAction(0.3f,movecard), 
            new MTCallFunc(() => movecard.transform.position = originPos), 
            new MTDelayTime(1f),
            new MTCallFunc(()=> movecard.BackColor())
            
            );
        var repeat = new MTRepeatForever(seq);
        return repeat;
    }
}
