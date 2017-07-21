using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum LangEnum
{
    Score,
    Time,
    Moves,
    Setting,
    Undo,
    Hint,
    Draw3,
    VegasCumulative,
    WinningDeals,
    Orientation,
    Portrait,
    Landscape,
    Auto,
    CongratulationsScreen,
    TapMove,
    On,
    Off,
    Time_Moves,
    RightHanded,
    Rules,
    Done,
    Win,
    Lose,
    NewGame,
    Cancel,
    ReplayThisGame

}

public class Lang  {

    Dictionary<LangEnum, string> langDict = new Dictionary<LangEnum, string>();
    public void Init()
    {
        foreach (LangEnum _landKey in Enum.GetValues(typeof(CardColor)))
        {
            langDict.Add(_landKey, _landKey.ToString());
        }

    }

    public string GetLang(LangEnum en)
    {
        string name = "";
       if (langDict.TryGetValue(en,out name))
        {
            return name;
        }
        return "";
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
