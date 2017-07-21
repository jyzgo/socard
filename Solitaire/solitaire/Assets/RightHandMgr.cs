using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RightHandMgr : MonoBehaviour {
    public static RightHandMgr current;

    public CardAbstract[] Platforms;

    public Transform PileReadyRoot;
    void Awake()
    {
        float x = Screen.width / 2;
        float y = Screen.height / 2;

        Vector3 wPos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0));
        transform.position = wPos;
        for(int i = 0; i <Platforms.Length;i++)
        {
            _cardSet.Add(Platforms[i]);
        }

        current = this;
    }



    HashSet<CardAbstract> _cardSet = new HashSet<CardAbstract>();
    public void AddCard(CardAbstract card)
    {
        _cardSet.Add(card);
    }
    public bool isLeft = false;
    public void ResetPos()
    {
        isLeft = !isLeft;
        var en = _cardSet.GetEnumerator();
        while(en.MoveNext())
        {
            var card = en.Current;
            
            var oldPos = card.transform.position;

            var mirrorPos = new Vector3(oldPos.x * -1, oldPos.y, oldPos.z); 
            card.originalPos = mirrorPos;
            if(card.transform.parent == null || card.transform.parent.CompareTag("ROOT") || (card.preCard == null && card.cardState != CardState.InPile))
            {
                card.transform.position = mirrorPos;
            }

            
        }

        float centerX = CanvasBtns[0].parent.position.x;

        var readyRootOldPos = PileReadyRoot.position;
        var pileMirrorPos = new Vector3(readyRootOldPos.x * -1, readyRootOldPos.y, readyRootOldPos.z);
        PileReadyRoot.position = pileMirrorPos;

        for (int i = 0; i < CanvasBtns.Length;i ++)
        {
            var curTrans = CanvasBtns[i];
            var oldPos = curTrans.position;
            float offset = oldPos.x - centerX;
            curTrans.position = new Vector3(centerX + offset * -1, oldPos.y, oldPos.z);


        }
        var undoActionList = LevelMgr.current._CardActions;
        for(int i = 0; i <undoActionList.Count;i ++)
        {
            var curAction = undoActionList[i];
            curAction.ResetPos();
        }

       // LevelMgr.current.RefreshPileReady();
    }


    public Transform[] CanvasBtns;
}
