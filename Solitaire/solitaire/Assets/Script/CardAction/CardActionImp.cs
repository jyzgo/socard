using UnityEngine;
using System.Collections;
using System;
using MTUnity.Actions;

public class CardActionImp : CardAction {

    CardAbstract _mainCard;
    CardAbstract _otherCard;
    Vector3 _originalPos;
    CardAbstract _otherOriginalPreCard;
    Transform _originalTrans;
    bool _isOriginalPrecardUp = false;

    CardState _otherOriginalCardState = CardState.None;
    public void Init(CardAbstract card,CardAbstract bePutCard)
    {
        _mainCard = card;
        _otherCard = bePutCard;
        _originalPos = _otherCard.originalPos;
        _originalTrans = _otherCard.transform.parent;
        _otherOriginalPreCard = _otherCard.preCard;
        if(_otherOriginalPreCard != null)
        {
            _isOriginalPrecardUp = _otherOriginalPreCard.isUp();


        }
        _otherOriginalCardState = _otherCard.cardState;

        //if(_otherCard.cardState == CardState.InPile)
        //{
        //    var pileReadyList = LevelMgr.current._pileReadyList;
        //    _cardIndexInReady = pileReadyList.IndexOf(_otherCard.gameObject);
            
        //}
    }
    public override void DoAction()
    {

        var gameState = LevelMgr.current._gameState;
        UpdateGameState(_mainCard, _otherCard);
        if (_otherCard.cardState == CardState.InPile)
        {
            LevelMgr.current.RemoveFromPile(_otherCard);
            LevelMgr.current.RefreshPileReady();

        }

        if (_mainCard.cardState == CardState.InTarget)
        {
            _otherCard.nextCard = null;
        }
        _otherCard.cardState = _mainCard.cardState;
        _mainCard.nextCard = _otherCard;

        _otherCard.transform.parent = _mainCard.transform;
        bool isPlat = false;
        if(_otherCard.preCard != null && _otherCard.preCard.cardState == CardState.InPlatform && _otherCard.preCard.preCard == null)
        {
            isPlat = true;
        }
        bool isBackUp = false;
        if( _otherCard.DetachCardFromOriginal(ref isBackUp))
        {
            if (!isPlat && isBackUp)
            {
                gameState.AddScore(gameState.FlipPlatCard());
                LevelMgr.current.UpdateUI();
            }
            
        }

        float moveTime = 0.2f;
        if(LevelMgr.current.isAutoCollecting)
        {
            moveTime = 0.1f;
        }

        _otherCard.preCard = _mainCard;

        _otherCard.transform.eulerAngles = Vector3.zero;
        _otherCard.gameObject.StopAllActions();
        var otherPos = _otherCard.transform.position;
        _otherCard.transform.position = new Vector3(otherPos.x, otherPos.y, -5f);

        var tarPos = _mainCard.GetNextPos();
        tarPos = new Vector3(tarPos.x, tarPos.y, -5f);

        _otherCard.RunActions(
            new MTMoveToWorld(moveTime, tarPos),
            new MTCallFunc(()=>_otherCard.transform.position = _mainCard.GetNextPos()),
            new MTCallFunc(()=> _otherCard.transform.eulerAngles = Vector3.zero));
        LevelMgr.current.DisableUndo(moveTime);

        LevelMgr.current.TryCheckWin();




        LevelMgr.current._gameState.Moves++;
        LevelMgr.current.UpdateMoves();
      

        
    }

    public override void ReverseAction()
    {

        var gameState = LevelMgr.current._gameState;
        _mainCard.nextCard = null;
        _otherCard.preCard = _otherOriginalPreCard;
        if (_otherOriginalPreCard != null)
        {
            _otherOriginalPreCard.nextCard = _otherCard;
        }
        _otherCard.cardState = _otherOriginalCardState;
        var otherCardPos = _otherCard.transform.position;
        _otherCard.transform.position = new Vector3(otherCardPos.x, otherCardPos.y, -5f);
        _otherCard.RunAction(new MTMoveToWorld(0.15f, _originalPos));    
        
        if(_otherOriginalCardState == CardState.InPlatform)
        {
            if(_otherOriginalPreCard.preCard != null)//not the root
            {
                _otherOriginalPreCard.preCard.nextCard = _otherOriginalPreCard;
                if (!_isOriginalPrecardUp)
                {
                    gameState.AddScore(gameState.FlipPlatCard() * -1);
                    _otherOriginalPreCard.gameObject.RunActions(
                        new MTRotateTo(0.3f, new Vector3(0, 180, 0)),
                        new MTCallFunc(() => _otherOriginalPreCard.transform.eulerAngles = new Vector3(0, 180, 0)));
                }
                
            }
        }

        LevelMgr.current.RunActions(new MTDelayTime(0.2f), new MTCallFunc(() => _otherCard.transform.parent = _originalTrans));

        if(_otherCard.cardState == CardState.InPile)
        {
            LevelMgr.current._pileReadyList.Add(_otherCard);
            LevelMgr.current.RefreshPileReady();
        }


        SoundManager.Current.Play_put_success(0);

        gameState.AddScore(addScoreFrom *-1);
        gameState.AddScore(gameState.Reverse());
        if (LevelMgr.current.CheckMetAutoCollect())
        {
            LevelMgr.current.ShowAutoCollectBtn();
        }else
        {
            LevelMgr.current.HideAutoCollectBtn();
        }

        LevelMgr.current._gameState.Moves--;
        LevelMgr.current.UpdateUI();
    }

    int addScoreFrom = 0;
    public void UpdateGameState(CardAbstract mainCard,CardAbstract othCard)
    {

        int addScore = 0;
        var gameState = LevelMgr.current._gameState;

        if (othCard.cardState == CardState.InPile && mainCard.cardState == CardState.InTarget)
        {

            addScore = gameState.FromPileToTarget();
        }
        else if (othCard.cardState == CardState.InPlatform && mainCard.cardState == CardState.InTarget)
        {
            addScore = gameState.FromPlatToTarget();
        }
        else if (othCard.cardState == CardState.InPile && mainCard.cardState == CardState.InPlatform)
        {
            addScore = gameState.FromPileToPlat();
        }
        else if (othCard.cardState == CardState.InTarget && mainCard.cardState == CardState.InPlatform)
        {
            addScore = gameState.FromTarToPlat();
        }


        addScoreFrom = gameState.AddScore(addScore);
        LevelMgr.current.UpdateUI();
    }
    
    public override void  ResetPos()
    {
        _originalPos = new Vector3(_originalPos.x * -1, _originalPos.y, _originalPos.z);
    }

}
