using UnityEngine;
using System.Collections;
using MTUnity.Utils;
using System;

public class VegasState : GameState {



    public override int FromPileToTarget()
    {

        return 5;
    }

    public override int FromPlatToTarget()
    {
        return 5;
    }


    public override int FromPileToPlat()
    {
        return 0;
    }

    bool isCumulitive = false;
    public override void Init()
    {
        base.Init();
        _totalScore = -52;
        isCumulitive = SettingMgr.current.VegasCumulative == 1;
        if(isCumulitive)
        {
            _totalScore += SettingMgr.current.CumulitiveNum;
            SettingMgr.current.CumulitiveNum = _totalScore;
        }

        remainRefreshTime = 0;
        if(SettingMgr.current.Draw3 == 1)
        {
            remainRefreshTime = 2;
            draw3 = true;
        }else
        {
            draw3 = false;
        }



        UpdateUnableIcon();
    }


    void UpdateUnableIcon()
    {
        LevelMgr.current.SetUnableRefreshIconVis(remainRefreshTime <= 0);
    }

    


    public override int FromTarToPlat()
    {
        return -5;
    }

    public override int Reverse()
    {
        return 0;
    }

    public override int FlipPlatCard()
    {
        return 0;
    }

    public override int RefreshPile()
    {
        return 0;
    }

    public override int RemainRefreshTime()
    {
        return remainRefreshTime;
    }

    public override void SetRefreshTime(int n)
    {
        remainRefreshTime = n;
        UpdateUnableIcon();
    }

    public override void ReduceRefreshTime()
    {
        remainRefreshTime--;
        UpdateUnableIcon();
    }


    public override int AddScore(int score)
    {
        _totalScore += score;
        if (isCumulitive)
        {
            SettingMgr.current.CumulitiveNum = _totalScore;
        }

        return score;
    }
    public override int GetScore()
    {
        return _totalScore;
    }

    public override MTJSONObject ToJson()
    {
        MTJSONObject js = MTJSONObject.CreateDict();
        js.Add("type", 1);
        js.Add("isCumulitive",isCumulitive);

        AddToJs(js);
        

        return js;
    }

    public override void Init(MTJSONObject js)
    {
        isCumulitive = js["isCumulitive"].b;

        InitByJs(js);
    }

    public override string GetStateName()
    {
        return "Vegas";
    }

}
