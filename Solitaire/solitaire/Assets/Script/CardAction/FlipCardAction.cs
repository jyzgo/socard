using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FlipCardAction : CardAction
{

    List<CardAbstract> _flipCardList = new List<CardAbstract>();
    List<Vector3> _oldCardPos = new List<Vector3>();
    List<Vector3> _originalCardPosList = new List<Vector3>();
    List<Vector3> _originlaCardEulerList = new List<Vector3>();
    Dictionary<CardAbstract, Transform> _originDict = new Dictionary<CardAbstract, Transform>();
    public void Init(List<CardAbstract> flipList)
    {

        for(int i  = 0; i < flipList.Count;i ++)
        {
            var c = flipList[i];
            _flipCardList.Add(c);
            _originDict.Add(c, c.transform.parent);
            _oldCardPos.Add(c.transform.position);
            _originalCardPosList.Add(c.originalPos);
            _originlaCardEulerList.Add(c.transform.eulerAngles);
        }


    }
    int refreshTime = 0;
    public override void DoAction()
    {
        var pileList = LevelMgr.current._pileList;
        var pileReadyList = LevelMgr.current._pileReadyList;
        bool moveSuccess = false;
        for(int i = 0; i <_flipCardList.Count; i ++ )
        {
            var flipCard = _flipCardList[i];
            flipCard.BlockTouch(0.2f);
            pileList.Remove(flipCard);
            pileReadyList.Add(flipCard);
            moveSuccess = true;
        }
        SoundManager.Current.Play_flip_pile(0);
        LevelMgr.current.RefreshPileReady();
        refreshTime = LevelMgr.current._gameState.RemainRefreshTime();
        if (moveSuccess)
        {
            LevelMgr.current._gameState.Moves++;
           

            LevelMgr.current.UpdateUI();
        }


    }

    public override void ReverseAction()
    {
        var gameState = LevelMgr.current._gameState;
        var pileList = LevelMgr.current._pileList;
        var pileReadyList = LevelMgr.current._pileReadyList;
        bool moveSuccess = false;
        for(int i = _flipCardList.Count -1; i >= 0; i--)
        {
            var flipCard = _flipCardList[i];
            var trans = _originDict[flipCard];

            pileList.Add(flipCard);
            pileReadyList.Remove(flipCard);
            flipCard.transform.position = _oldCardPos[i];
            flipCard.transform.eulerAngles = _originlaCardEulerList[i];
            flipCard.originalPos = _originalCardPosList[i];
            flipCard.transform.parent = trans;
            moveSuccess = true;
        }

        SoundManager.Current.Play_flip_pile(0);
        LevelMgr.current.RefreshPileReady();
        gameState.SetRefreshTime(refreshTime);
        if (moveSuccess)
        {
            gameState.Moves--;
            

            gameState.AddScore(LevelMgr.current._gameState.Reverse());
           
        }

        LevelMgr.current.UpdateUI();


    }

    public override void ResetPos()
    {
        ResetListPos(_oldCardPos);
        ResetListPos(_originalCardPosList);

    }
}
