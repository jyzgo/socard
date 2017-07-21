using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ThemeMgr : MonoBehaviour {
    public static ThemeMgr current;

    void Awake()
    {
        current = this;
    }


    public BGResize bg;

    public Sprite[] CardBgs;


    public GameObject CardBgPrefab;
    public GameObject CardRoot;

    const int themeMaxId = 16;
    const int cardBgMaxIndex = 21;

    int _curThemeIndex = 1;
    public void TestBg()
    {
        _curThemeIndex++;

        SetBg(_curThemeIndex);
    }


    int _curCardBack = 0;
    public void TestCardBack()
    {
        _curCardBack++;
        SetBack(_curCardBack);

    }

    void SetBack(int index)
    {
        if (index >= CardBgs.Length)
        {
            index = 0;
        }

        _curCardBack = index;
        SettingMgr.current.CardBackIndex = index;
        var sp = CardBgs[index]; //Resources.Load<Sprite>("artRes/cardback/" + _curCardBack);
        var cardList = LevelMgr.current.CardList;
        SettingMgr.current.CardBack.sprite = sp;
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].UpdateBack(sp);

        }
        Resources.UnloadUnusedAssets();
    }

    void SetBg(int index)
    {
        if (index > themeMaxId)
        {
            index = 1;
        }
        _curThemeIndex = index;

        var sp = Resources.Load<Sprite>("artRes/theme/" + _curThemeIndex);
        bg.SetBg(sp);
        SettingMgr.current.BgIndex = _curThemeIndex;
        SettingMgr.current.BgTheme.sprite = sp;
        Resources.UnloadUnusedAssets();
    }
    List<CardBgSelector> _cardBgList = new List<CardBgSelector>();
    List<ThemeSelector> _themeList = new List<ThemeSelector>();
    public void SettingLoadDone()
    {
        _cardBgList.Clear();

        SetBack(SettingMgr.current.CardBackIndex);
        SetBg(SettingMgr.current.BgIndex);
        _selectCardIndex = _curCardBack;

        for (int i = 0; i <CardBgs.Length;i++)
        {
            int curIndex = i ;
            var sp = CardBgs[i];// Resources.Load<Sprite>("artRes/cardback/" + curIndex);
            var inst = Instantiate<GameObject>(CardBgPrefab);
            var cardSe = inst.GetComponent<CardBgSelector>();
            cardSe.Init(curIndex, sp, curIndex == _curCardBack);
            _cardBgList.Add(cardSe);
            inst.transform.SetParent(CardRoot.transform);
            inst.transform.localScale = new Vector3(1, 1, 1);
        }

        _themeList.Clear();
        for (int i = 0; i < themeMaxId;i++)
        {
            int curIndex = i + 1;
            var sp = Resources.Load<Sprite>("artRes/themeSmall/" + curIndex);
            var inst = Instantiate<GameObject>(ThemePrefab);
            var cardSe = inst.GetComponent<ThemeSelector>();
            cardSe.Init(curIndex, sp, curIndex == _curCardBack);
            _themeList.Add(cardSe);
            inst.transform.SetParent(ThemeRoot.transform);
            inst.transform.localScale = new Vector3(1, 1, 1);
        }


    }

    [Header("CardSelectMenu")]
    public GameObject CardSelectMenu;
    public void ShowCardSelectMenu()
    {
        _selectCardIndex = _curCardBack;

        UpdateCardSelectGlow(_selectCardIndex);
        CardSelectMenu.SetActive(true);
    }

    public void UpdateCardSelectGlow(int index)
    {
        _selectCardIndex = index;

        for (int i = 0; i < _cardBgList.Count; i++)
        {
            var curCardBgSel = _cardBgList[i];
            curCardBgSel.ShowGlow(curCardBgSel._index == _selectCardIndex);
        }
     

    }

    public void HideCardSelectMenu()
    {
        CardSelectMenu.SetActive(false);
    }


    public int _selectCardIndex = 0;
    public void ConfirmSelectBg()
    {
        SetBack(_selectCardIndex);
        HideCardSelectMenu();
    }





    [Header("ThemeSelectMenu")]
    public int _selectThemeIndex = 0;
    public GameObject ThemeSelectMenu;
    public void ShowThemeSelectMenu()
    {
        _selectThemeIndex = _curThemeIndex;

        UpdateThemeSelectGlow(_selectThemeIndex);
        ThemeSelectMenu.SetActive(true);
    }

    public void UpdateThemeSelectGlow(int index)
    {
        _selectThemeIndex = index;

        for (int i = 0; i < _themeList.Count; i++)
        {
            var curCardBgSel = _themeList[i];
            curCardBgSel.ShowGlow(curCardBgSel._index == _selectThemeIndex);
        }


    }

    public void HideThemeSelectMenu()
    {
        ThemeSelectMenu.SetActive(false);
    }



    public void ConfirmSelectTheme()
    {
        SetBg(_selectThemeIndex);
        HideThemeSelectMenu();
    }


    public GameObject ThemeRoot;
    public GameObject ThemePrefab;
}
