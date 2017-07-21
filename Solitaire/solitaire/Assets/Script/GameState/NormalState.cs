using UnityEngine;
using System.Collections;
using MTUnity.Utils;

public class NormalState : GameState {



    public override int FromPileToTarget()
    {
        return 15;
    }

    public override int FromPlatToTarget()
    {
        return 10;
    }

    public int flipTime = 0;

    
    int current = 0;

    public override void Init()
    {
        base.Init();
        
        LevelMgr.current.SetUnableRefreshIconVis(false);
       draw3 = SettingMgr.current.Draw3 == 1;
    }


    public override int FromPileToPlat()
    {

        return 5;
    }


    public override int FromTarToPlat()
    {
        return -15;
    }

    public override int Reverse()
    {
        return -2;
    }

    public override int FlipPlatCard()
    {
        return 5;
    }

    public override int RefreshPile()
    {
        if (draw3)
        {
            if (refreshPileTime >= 3)
            {
                return -20;
            }
            return 0;
        }
        
        return -100;
    }

    public override void ReduceRefreshTime()
    {

        refreshPileTime++;
    }
    public override int AddScore(int score)
    {

        if(_totalScore + score < 0)
        {
            int t = _totalScore;
            _totalScore = 0;

            if (score > 0)
            {
                return t;
            }
            else
            {
                return -1 * t;
            }
           
        }

        _totalScore += score;
        return score;

    }


    public override MTJSONObject ToJson()
    {
        MTJSONObject js = MTJSONObject.CreateDict();
        js.Add("type", 0);
        js.Add("flipTime", flipTime);
        js.Add("current", current);

        AddToJs(js);
        return js;

    }

    public override void Init(MTJSONObject js)
    {
        flipTime = js.Get("flipTime").i;
        current = js.Get("current").i;

        InitByJs(js);

       
    }

    public override int GetScore()
    {

        return _totalScore;
    }



    public override void ReduceScoreByTime()
    {
        current += 1;
        if(current == ReduceInterval)
        {
            current = 0;
            AddScore(-2);
            LevelMgr.current.UpdateUI();
        }
    }
    public override string GetStateName()
    {

        return "Normal";
     }
}

