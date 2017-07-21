using UnityEngine;
using System.Collections;
using MTUnity.Utils;
using System;

public class ChallengeState : GameState
{


    public override int FromPileToTarget()
    {

        return 15;
    }

    public override int FromPlatToTarget()
    {
        return 5;
    }


    public override int FromPileToPlat()
    {
        return 5;
    }


    public override void Init()
    {
        base.Init();
        _totalScore = 0;

        draw3 = SettingMgr.current.Draw3 == 1;


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
        return 5;
    }

    public override int RefreshPile()
    {
        return 0;
    }


    public override void SetRefreshTime(int n)
    {
        remainRefreshTime = n;
    }

    public override void ReduceRefreshTime()
    {
        remainRefreshTime--;
    }


    public override int AddScore(int score)
    {
        _totalScore += score;

        return score;
    }
    public override int GetScore()
    {
        return _totalScore;
    }

    public override MTJSONObject ToJson()
    {
        MTJSONObject js = MTJSONObject.CreateDict();
        js.Add("type", 2);
        js.Add("current", current);
        AddToJs(js);

        return js;
    }

    int current = 0;

    public override void Init(MTJSONObject js)
    {
        current = js.Get("current").i;
        InitByJs(js);
    }

    public override string GetStateName()
    {
        return "Challenge";
    }
    public override void ReduceScoreByTime()
    {
        current += 1;
        if (current == ReduceInterval)
        {
            current = 0;
            AddScore(-2);
            LevelMgr.current.UpdateUI();
        }

    }
}
