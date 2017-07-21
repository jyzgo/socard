using UnityEngine;
using System.Collections;
using MTUnity.Utils;

public abstract class GameState
{

    protected int _totalScore = 0;
    protected int _refreshPileTimes = 0;
    public virtual int FromPileToTarget()
    {
        return 0;
    }

    public virtual int FromPlatToTarget()
    {
        return 0;
    }


    public virtual int FromPileToPlat()
    {
        return 0;
    }

     protected const int ReduceInterval = 10;



    public virtual int FromTarToPlat()
    {
        return 0;
    }


    public virtual int FlipPlatCard()
    {
        return 0;
    }

    public virtual int Reverse()
    {
        return 0;
    }

    public virtual int RefreshPile()
    {
        return 0;
    }

    public virtual int AddScore(int score)
    {
        return score;
    }

    protected int remainRefreshTime = 1;

    public virtual int RemainRefreshTime()
    {
        return 1;
    }

    public virtual MTJSONObject ToJson()
    {
        return null;
    }

    public void AddToJs(MTJSONObject js)
    {
        js.Add("_totalScore", _totalScore);
        js.Add("draw3", draw3);
        js.Add("_refreshPileTimes", _refreshPileTimes);
        js.Add("remainRefreshTime", remainRefreshTime);
        js.Add("refreshPileTime", refreshPileTime);
        js.Add("UndoTimes", UndoTimes);
        js.Add("GameTime", GameTime);
        js.Add("Moves", Moves);
    }

    public void InitByJs(MTJSONObject js)
    {
        _totalScore = js["_totalScore"].i;
        draw3 = js.Get("draw3").b;
        _refreshPileTimes = js["_refreshPileTimes"].i;
        remainRefreshTime = js["remainRefreshTime"].i;
        refreshPileTime = js["refreshPileTime"].i;
        UndoTimes = js["UndoTimes"].i;
        GameTime = js["GameTime"].f;
        Moves = js["Moves"].i;
    }


    public virtual void Init(MTJSONObject js)
    {

    }
    public bool draw3 = false;

    public virtual void SetRefreshTime(int n)
    {
    }

    public int refreshPileTime = 0;
    public virtual void ReduceRefreshTime()
    { }

    public virtual void Init()
    {
        GameTime = 0;
        Moves = 0;
        UndoTimes = 0;
    }

    public virtual int GetScore()
    {
        return _totalScore;
    }
    public int UndoTimes = 0;

    public float GameTime = 0;
    public string GetTime()
    {
        if ((int)(GameTime / 60) > 0)
        {
            return (int)(GameTime / 60) + ":" + ((int)(GameTime % 60)).ToString("00");
        }else
        {
            return ((int)GameTime).ToString();
        }
    }

    public void Tick()
    {
        GameTime += 1;
        ReduceScoreByTime();
    }


    public virtual void ReduceScoreByTime()
    { }

    public int Moves = 0;

    public virtual string GetStateName()
    {
        return "G";
    }




}