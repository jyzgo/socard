using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MTUnity.Actions;
using MonsterLove.StateMachine;
using UnityEngine.UI;
using System;
using MTUnity.Utils;
using MTUnity;
using System.IO;
using MTXxtea;


[Serializable]
public struct CardData
{
    public CardColor cardColor;

    [Range(1, 13)]
    public int CardNum;
}

public class LevelMgr : MonoBehaviour
{

    public enum LevelState
    {
        Load,
        Playing,
        Win

    }

    public GameObject MenuCanvas;
    public WinWindow WinCanvas;

    public GameObject UnableRefreshIcon;

    public void SetUnableRefreshIconVis(bool b)
    {
        UnableRefreshIcon.SetActive(b);
    }


    Lang _langSet;
    [HideInInspector]
   public List<CardAction> _CardActions = new List<CardAction>();

    public GameObject CardPrefab;

    public Transform start;

    public Text Score;
    public Text UseTime;
    public Text UseTime2;
    public Text Moves;

    public GameState _gameState;


    public void UpdateUI()
    {
        
        Score.text = _langSet.GetLang(LangEnum.Score) + ":" + _gameState.GetScore().ToString();
        UpdateMoves();
        UpdateTime();

    }

    public bool isCardPressing()
    {
        return _pressingCard.Count > 0;
    }

    public void CleanPressingCard()
    {
        var en = _pressingCard.GetEnumerator();
        while(en.MoveNext())
        {

            var card = en.Current;
            card.BackToOriginalPos();
        }
        _pressingCard.Clear();
    }

    public bool IsBlockUIBtn()
    {
        return _BlockUIBtnTime > Time.time;
    }

    public void ShowHint()
    {
        if (IsBlockUIBtn())
        {
            return;
        }


        if (!undoAble || isAutoCollecting)
        {
            return;
        }
        if (_pressingCard.Count > 0)
        {
            return;
        }

        CardAbstract moveCard = null;
        CardAbstract tarCard = null;

        //find whether the pile card is able to put to tar
        if (FindPileCardToTargetAndPlatform(ref moveCard, ref tarCard))
        {
            HintMgr.current.ShowMoveHint(moveCard, tarCard);
            return;
        }


        //iterate the platform top card and try put it to the target area
        if (FindPlatformCardToTarget(ref moveCard, ref tarCard))
        {
            HintMgr.current.ShowMoveHint(moveCard, tarCard);
            return;
        }


        //find platform to platform card
        if (FindPlatformToPlatform(ref moveCard, ref tarCard))
        {
            HintMgr.current.ShowMoveHint(moveCard, tarCard);
            return;
        }

        // flip  pile card
        if (FindFlipableCard(ref moveCard))
        {
            var tarPos = PileReadyTrans[PileReadyTrans.Length - 1].position;
            HintMgr.current.ShowFlipHint(moveCard, tarPos);
            return;
        }

        // target to platform
        if (FindTargetToPlatform(ref moveCard, ref tarCard))
        {
            HintMgr.current.ShowMoveHint(moveCard, tarCard);
            return;
        }

        HintMgr.current.ShowNoMoveText();


    }


    public GameObject Toolbar;


    public bool FindTargetToPlatform(ref CardAbstract moveCard, ref CardAbstract tarCard)
    {
        for (int i = 0; i < _targetList.Count; i++)
        {
            var curCard = _targetList[i].GetTopCard();
            for (int j = 0; j < _platformList.Count; j++)
            {
                var pCard = _platformList[i].GetTopCard();
                if (pCard.isCardPutable(curCard))
                {
                    moveCard = curCard;
                    tarCard = pCard;
                    return true;

                }

            }

        }
        return false;
    }

    public bool FindFlipableCard(ref CardAbstract moveCard)
    {
        if (_pileList.Count > 0)
        {
            moveCard = _pileList[_pileList.Count - 1];
            return true;
        }
        return false;
    }

    public bool FindPlatformCardToTarget(ref CardAbstract moveCard, ref CardAbstract targetCard)
    {
        if (_platformList.Count > 0)
        {
            for (int i = 0; i < _platformList.Count; i++)
            {
                var lastCard = _platformList[i].GetTopCard();
                for (int j = 0; j < _targetList.Count; j++)
                {
                    var tarCard = _targetList[j].GetTopCard();
                    if (tarCard.isCardPutable(lastCard))
                    {
                        moveCard = lastCard;
                        targetCard = tarCard;
                        return true;
                    }

                }
            }
        }

        return false;

    }


    public bool FindPileCardToTargetAndPlatform(ref CardAbstract moveCard, ref CardAbstract targetCard)
    {
        if (_pileReadyList.Count > 0)
        {
            var lastCard = _pileReadyList[_pileReadyList.Count - 1];

            for (int i = 0; i < _targetList.Count; i++)
            {
                var tarCard = _targetList[i].GetTopCard();
                if (tarCard.isCardPutable(lastCard))
                {
                    moveCard = lastCard;
                    targetCard = tarCard;
                    return true;
                }

            }

            for (int i = 0; i < _platformList.Count; i++)
            {
                var tarCard = _platformList[i].GetTopCard();
                if (tarCard.isCardPutable(lastCard))
                {
                    moveCard = lastCard;
                    targetCard = tarCard;
                    return true;
                }

            }
        }


        return false;
    }

    public bool FindPlatformToPlatform(ref CardAbstract moveCard, ref CardAbstract targetCard)
    {
        if (_platformList.Count > 0)
        {
            for (int i = 0; i < _platformList.Count; i++)
            {
                var lastCard = _platformList[i].GetTopCard();
                for (int j = 0; j < _platformList.Count; j++)
                {
                    if (i != j)
                    {
                        var curCard = _platformList[j];
                        var nextCard = curCard.nextCard;
                        while (nextCard != null)
                        {
                            if (nextCard.isUp())
                            {
                                if (lastCard.isCardPutable(nextCard))
                                {
                                    //if (nextCard.preCard != null)
                                    //{
                                    //    var preCard = nextCard.preCard;
                                    //    if ((int)preCard.cardColor % 2 == (int)lastCard.cardColor % 2 && preCard.isUp())
                                    //    {
                                    //        nextCard = nextCard.nextCard;
                                    //        continue;
                                    //    }
                                    //}
                                    moveCard = nextCard;
                                    targetCard = lastCard;
                                    return true;
                                }
                            }
                            nextCard = nextCard.nextCard;
                        }
                    }
                }
            }
        }
        return false;
    }



    public void UpdateMoves()
    {
        Moves.text = _langSet.GetLang(LangEnum.Moves) + ":" + _gameState.Moves.ToString();
    }

    public void UpdateTime()
    {
        UseTime.text = _langSet.GetLang(LangEnum.Time) + ":" + _gameState.GetTime();
        UseTime2.text = _langSet.GetLang(LangEnum.Time) + ":" + _gameState.GetTime();
    }


    IEnumerator TimeTick()
    {
        
        while (true)
        {
            yield return new WaitForSeconds(1f);
            _gameState.Tick();
            UpdateTime();

        }
    }


    StateMachine<LevelState> fsm;

    public static LevelMgr current;



    public Transform[] TargetPlace;
    public Transform[] PlatformPlace;
    public Transform Pile;

    public Transform[] PileReadyTrans;
    public List<CardAbstract> _pileReadyList = new List<CardAbstract>();


   

    public SettingMgr _settingMgr;
    public SoundManager _soundMgr;
    IEnumerator _timeTick;
    void Awake()
    {
       
        AdMgr.RegisterAllAd(this);
        current = this;



    }

    //void onAppLovinEventReceived(string ev)
    //{
    //    if (ev.Contains("DISPLAYEDINTER"))
    //    {
    //        // An ad was shown.  Pause the game.
    //        //MTTracker.Instance.Track(SoliTrack.ads, StatisticsMgr.current.WinsCount(), "0", "applovin");
    //        AdMgr.TrackApplovin("0");
    //    }
    //    else if (ev.Contains("HIDDENINTER"))
    //    {
    //        // Ad ad was closed.  Resume the game.
    //        // If you're using PreloadInterstitial/HasPreloadedInterstitial, make a preload call here.
    //        AdMgr.ApplovinPreloadInterstitial();
    //        AdMgr.TrackApplovin("1");
    //        //MTTracker.Instance.Track(SoliTrack.ads, StatisticsMgr.current.WinsCount(), "1","applovin");
    //    }
    //    else if (ev.Contains("LOADEDINTER"))
    //    {
    //        // An interstitial ad was successfully loaded.
    //        AdMgr.TrackApplovin("3");
    //     //   MTTracker.Instance.Track(SoliTrack.ads, StatisticsMgr.current.WinsCount(), "3", "applovin");
    //    }
    //    else if (string.Equals(ev, "LOADINTERFAILED"))
    //    {
    //        // An interstitial ad failed to load.
    //        AdMgr.TrackApplovin("2");
    //   //     MTTracker.Instance.Track(SoliTrack.ads, StatisticsMgr.current.WinsCount(), "2", "applovin");
        
    //    }
    //}

    void Start()
    {
        _langSet = new Lang();
        _langSet.Init();
        //_settingMgr.LoadFile();
        Application.targetFrameRate = 60;
        GenCard();
        CleanCard();
        InitCardPlatform();
        if (File.Exists(GetArchivePath()))
        {
            
            fsm = StateMachine<LevelState>.Initialize(this, LevelState.Load);

        }
        else
        {
            fsm = StateMachine<LevelState>.Initialize(this, LevelState.Playing);
        }
       
        if (SettingMgr.current.Installed == 0)
        {
            //MTTracker.Instance.TrackInstall();
            //MTTracker.Instance.TrackDeviceInfo();
            SettingMgr.current.Installed = 1;
            SettingMgr.current.SaveToFile();
        }

        //if (Application.internetReachability != NetworkReachability.NotReachable)
        //{
        //    MTTracker.Instance.Track(SoliTrack.login, StatisticsMgr.current.WinsCount(), Application.internetReachability.ToString());
        //}
    }

    void OnApplicationQuit()
    {
        SaveGame();
        SettingMgr.current.SaveToFile();
      //  MTTracker.Instance.Track(SoliTrack.background, StatisticsMgr.current.WinsCount(), Time.time.ToString());
    }

    void OnApplicationPause(bool paused)
    {
        isStartTick = false;
        StopTick();

        if (!paused)
        {
            // Application came back to the fore; double-check authentication
          //  MTTracker.Instance.Track(SoliTrack.background, StatisticsMgr.current.WinsCount(), Time.time.ToString());
        }
        else
        {
            SaveGame();
        }


    }

    

    void SaveGame()
    {
        if (isAutoCollecting || _gameState == null || (_gameState != null && _gameState.GameTime < 1f))
        {
            File.Delete(GetArchivePath());
            return;
        }

        if (GetLevelState() == LevelState.Playing)
        {
            SaveArchive();
        }
        else
        {

            File.Delete(GetArchivePath());
        }
    }


    public void TestSave() {
        MTJSONObject saveJs = MTJSONObject.CreateDict();
        saveJs.Add("gameState", _gameState.ToJson());
        MTJSONObject targetJs = MTJSONObject.CreateList();
        for (int i = 0; i < _targetList.Count; i++)
        {
            MTJSONObject js = MTJSONObject.CreateList();
            var curTar = _targetList[i];
            var nCard = curTar.nextCard;
            while (nCard != null)
            {

                js.Add(nCard.CardIndex);
                nCard = nCard.nextCard;

            }

            targetJs.Add(js);

        }

        saveJs.Add(CardState.InTarget.ToString(), targetJs);

        MTJSONObject platFormJs = MTJSONObject.CreateList();
        for (int i = 0; i < _platformList.Count; i++)
        {
            MTJSONObject js = MTJSONObject.CreateList();
            var curTar = _platformList[i];
            var nCard = curTar.nextCard;
            while (nCard != null)
            {
                int index = nCard.CardIndex;
                if (nCard.isUp())
                {
                    index += 100;
                }
                js.Add(index);
                nCard = nCard.nextCard;

            }

            platFormJs.Add(js);

        }
        saveJs.Add(CardState.InPlatform.ToString(), platFormJs);


        MTJSONObject pileReady = MTJSONObject.CreateList();
        for (int i = 0; i < _pileReadyList.Count; i++)
        {
            var curCard = _pileReadyList[i];
            pileReady.Add(curCard.CardIndex);
        }

        saveJs.Add(CardState.InPileReady.ToString(), pileReady);

        MTJSONObject pileJs = MTJSONObject.CreateList();
        for (int i = 0; i < _pileList.Count; i++)
        {
            pileJs.Add(_pileList[i].CardIndex);
        }
        saveJs.Add(CardState.InPile.ToString(), pileJs);
        //  var bt = MTXXTea.Encrypt(saveJs.ToString(), SettingMgr.SKEY);
        //File.WriteAllBytes(GetArchivePath(), bt);
        File.WriteAllText(GetArchivePath() + "1", saveJs.ToString());
    }

    public void SaveArchive()
    {
        MTJSONObject saveJs = MTJSONObject.CreateDict();
        saveJs.Add("gameState",_gameState.ToJson());
        MTJSONObject targetJs = MTJSONObject.CreateList();
        for(int i = 0; i < _targetList.Count; i ++)
        {
            MTJSONObject js = MTJSONObject.CreateList();
            var curTar = _targetList[i];
            var nCard = curTar.nextCard;
            while (nCard != null)
            {

                js.Add(nCard.CardIndex);
                nCard = nCard.nextCard;
                
            }

            targetJs.Add(js);

        }

        saveJs.Add(CardState.InTarget.ToString(), targetJs);

        MTJSONObject platFormJs = MTJSONObject.CreateList();
        for(int i = 0; i < _platformList.Count; i ++)
        {
            MTJSONObject js = MTJSONObject.CreateList();
            var curTar = _platformList[i];
            var nCard = curTar.nextCard;
            while (nCard != null)
            {
                int index = nCard.CardIndex;
                if (nCard.isUp())
                {
                    index += 100;
                }
                js.Add(index);
                nCard = nCard.nextCard;

            }

            platFormJs.Add(js);

        }
        saveJs.Add(CardState.InPlatform.ToString(), platFormJs);


        MTJSONObject pileReady = MTJSONObject.CreateList();
        for (int i = 0; i < _pileReadyList.Count; i++)
        {
            var curCard = _pileReadyList[i];
            pileReady.Add(curCard.CardIndex);
        }

        saveJs.Add(CardState.InPileReady.ToString(), pileReady);

        MTJSONObject pileJs = MTJSONObject.CreateList();
        for(int i = 0; i < _pileList.Count;i ++)
        {
            pileJs.Add(_pileList[i].CardIndex);
        }
        saveJs.Add(CardState.InPile.ToString(), pileJs);
        //  var bt = MTXXTea.Encrypt(saveJs.ToString(), SettingMgr.SKEY);
        //File.WriteAllBytes(GetArchivePath(), bt);
        File.WriteAllText(GetArchivePath(), saveJs.ToString());
        
    }

    public void LoadArchive()
    {
        // var bt = File.ReadAllBytes(GetArchivePath());
        //string content = MTXXTea.DecryptToString(bt,SettingMgr.SKEY); 
        var content = File.ReadAllText(GetArchivePath());
        MTJSONObject loadJs = MTJSON.Deserialize(content);

        var stateJson = loadJs.Get("gameState");

        if(stateJson["type"].i == 1)
        {
            _gameState = new VegasState();
            _gameState.Init(stateJson);
        }else if(stateJson["type"].i == 0)
        {
            _gameState = new NormalState();
            _gameState.Init(stateJson);
        }else
        {

            _gameState = new ChallengeState();
            _gameState.Init(stateJson);
            ChallengeMgr.current.ChallengeActive = true;
        }
        flipNum = _gameState.draw3 == true ? 3 : 1;
    

        var tarListJs = loadJs.Get(CardState.InTarget.ToString());
        int m = 0;
        
        for (int i = 0; i < tarListJs.count; i++)
        {
            var curTarList = tarListJs[i];

            var curTar = _targetList[i];

            for(int j = 0; j <curTarList.count;j++)
            {
               
                int index = curTarList[j].i;
                var curCard = CardList[m];
               // Debug.Log("t " + m + " " + curTarList.count);
                m++;
                curCard.UpdateCardByNum(index);
                curCard.transform.eulerAngles = Vector3.zero;
                curCard.transform.position = curTar.GetNextPos();
                curCard.originalPos = curCard.transform.position;
                curCard.transform.parent = curTar.transform;
                curCard.cardState = CardState.InTarget;
                curTar.nextCard = curCard;
                curCard.preCard = curTar;
                curTar = curTar.nextCard;

            }
        }


        var platList = loadJs.Get(CardState.InPlatform.ToString());
        for(int i = 0; i < platList.count;i++)
        {
            var curPlatList = platList[i];
            var plat = _platformList[i];
            for(int j = 0; j <curPlatList.count;j++)
            {
                int index = curPlatList[j].i;
                var curCard = CardList[m];
               // Debug.Log("p " + m +" d " + curPlatList.count);
                m++;
              
                if (index >= 100)
                {
                    curCard.transform.eulerAngles = Vector3.zero;
                    index -= 100;
                }
                else
                {
                    curCard.transform.eulerAngles = new Vector3(0, 180, 0);
                }
                curCard.cardState = CardState.InPlatform;
                curCard.transform.position = plat.GetNextPos();
                curCard.UpdateCardByNum(index);
                curCard.originalPos = plat.GetNextPos();
                curCard.preCard = plat;
                curCard.transform.parent = Root;

                if (curCard.preCard != null && curCard.preCard.isUp())
                {
                   curCard.transform.parent = plat.transform;
                }
                plat.nextCard = curCard;

                curCard.originalPos = curCard.transform.position;
                plat = plat.nextCard;

            }
             
        }

        var readyList = loadJs.Get(CardState.InPileReady.ToString());
        _pileReadyList.Clear();
        for(int i = 0; i < readyList.count; i ++)
        {
            var curCard = CardList[m];
            //Debug.Log("r " + m + "  " + readyList.count);
            m++;
            int index =  readyList[i].i;
            curCard.cardState = CardState.InPile;
            curCard.UpdateCardByNum(index);
            curCard.originalPos = curCard.transform.position;
            _pileReadyList.Add(curCard);

        }

        var pileList = loadJs.Get(CardState.InPile.ToString());
        for(int i = 0; i <pileList.count;i++)
        {
            //Debug.Log("m " + m + " c" + pileList.count);
            var curCard = CardList[m];
            m++;
            curCard.cardState = CardState.InPile;
            int index = pileList[i].i;
            curCard.UpdateCardByNum(index);
            curCard.transform.parent = Pile;
            curCard.transform.localPosition = Vector3.zero;
            curCard.originalPos = curCard.transform.position;
            _pileList.Add(curCard);
        }

        RefreshPileReady(true);
        UpdateUI();
        _currentPlayState = StartState.Load;


    }

    public string GetArchivePath()
    {
        //Debug.Log("pp " + Application.persistentDataPath);
        return Application.persistentDataPath + "/archive.dt";
    }

    public LevelState GetLevelState()
    {
        return fsm.State;
    }
    public List<CardAbstract> CardList = new List<CardAbstract>();
    List<CardAbstract> _platformList = new List<CardAbstract>();
    List<CardAbstract> _targetList = new List<CardAbstract>();

    public void LoadTestLv()
    {
        LoadLvFromFile(SettingMgr.current.testLv);

    }

    int _challengeLv;
    public void CallChallengeLv(int lv)
    {
        _challengeLv = lv;
        _currentPlayState = StartState.Challenge; 
        StartGame();
        //LoadLvFromFile(lv, "challenge/solitaire");
    }


    public Text testLv;
    public Text testPath;
    int lvid = 0;
    void LoadLvFromFile(int n,string path = "winablegames/solitaire")
    {

        lvid = n;

        testPath.text = path + n;

        TextAsset lvTex = Resources.Load<TextAsset>(path + n.ToString());
        if (lvTex != null)
        {
            var levelStr = lvTex.text;



            string[] lines = levelStr.Split(new string[] { "\n" }, StringSplitOptions.None);


            for (int i = 0; i < 52; i++)
            {


                int s = Convert.ToInt32(lines[i]);

                CardColor c = (CardColor)(s / 13);
                int num = s % 13;
                var curCard = CardList[i];
                curCard.cardColor = c;
                curCard.CardNum = num + 1;

                curCard.UpdateCardView();


            }
        }
        else
        {
          //  Debug.Assert(true, "Not here");
            lvid = lvid * -1;
            CardList.RandomShuffle<CardAbstract>();
        }
        testLv.text = lvid.ToString(); 
        ResetGame();
    }

    public int resetCount = 0;
   // int lastPlayedLv = 0;
     void DoReplayGame()
    {
        //int lastWinState = lastPlayedLv > 0 ? 0 : 1;

       // MTTracker.Instance.Track(SoliTrack.play,StatisticsMgr.current.GetGameWons(2) ,SettingMgr.current.PlayingLv.ToString(), "2", lastWinState.ToString());
       // ResetGame();

        if (lastNewGameTime + 2 > Time.time)
        {
            return;
        }
        lastNewGameTime = Time.time;

        int oldLv = playingLv;
        LoadLvFromFile(oldLv);


    }

    int playingLv = 1;

    public Transform Root;

    void GenCard()
    {
        if (CardList.Count > 0)
        {
            return;
        }

        foreach (CardColor _cardColr in Enum.GetValues(typeof(CardColor)))
        {
            for (int i = 0; i < 13; i++)
            {
                var gb = Instantiate<GameObject>(LevelMgr.current.CardPrefab);
                var cardSc = gb.GetComponent<Card>();
                cardSc.cardColor = _cardColr;
                cardSc.CardNum = i + 1;
                gb.name = "card " + _cardColr.ToString() + (i + 1).ToString();
                cardSc.UpdateCardView();
                gb.transform.position = new Vector3(i * 0.5f, (int)_cardColr, i * -0.5f);
                CardList.Add(cardSc);
            }
        }

        for (int i = 0; i < PlatformPlace.Length; i++)
        {
            var cardAb = PlatformPlace[i].GetComponent<CardAbstract>();
            _platformList.Add(cardAb);
        }

        for (int i = 0; i < TargetPlace.Length; i++)
        {
            var cardAb = TargetPlace[i].GetComponent<CardAbstract>();
            _targetList.Add(cardAb);
        }


    }
    void CleanCard()
    {
        var en = CardList.GetEnumerator();
        while (en.MoveNext())
        {
            var curCard = en.Current;
            curCard.transform.position = new Vector3(0, -10f, 0);
            var curRotation = curCard.transform.eulerAngles;
            curCard.transform.eulerAngles = new Vector3(curRotation.x, 180, curRotation.y);

        }
    }





    public float _BlockUIBtnTime = 0;
    public void BlockBtn(float t)
    {

        float newBlockTime = Time.time + t;
        if(_BlockUIBtnTime < newBlockTime)
        {
            _BlockUIBtnTime = newBlockTime;
        }
    }



    List<GameObject>[] CardPlatform = new List<GameObject>[7];
    List<CardAbstract> PlatformCardList = new List<CardAbstract>();
    void InitCardPlatform()
    {
        for (int i = 0; i < CardPlatform.Length; i++)
        {
            CardPlatform[i] = new List<GameObject>();
        }

        for (int i = 0; i < PlatformPlace.Length; i++)
        {
            var card = PlatformPlace[i].GetComponent<CardAbstract>();
            PlatformCardList.Add(card);
        }
    }

    void CleanCardPlatform()
    {
        for (int i = 0; i < CardPlatform.Length; i++)
        {
            CardPlatform[i].Clear();
        }

    }



    public void FindTheBestCard(CardAbstract card,ref CardAbstract tarCard, ref CardAbstract platCard)
    {
        for (int i = 0; i < _targetList.Count; i++)
        {
            var curCard = _targetList[i].GetTopCard();
            if (curCard.isCardPutable(card))
            {
                tarCard = curCard;
                break;
            }

        }

        for (int i = 0; i < _platformList.Count; i++)
        {
            var curCard = _platformList[i].GetTopCard();
            if (curCard.isCardPutable(card))
            {
                platCard = curCard;
                break;
            }

        }


    }

    float lastNewGameTime = -10f;
    void DoNewGame()
    {
        if (lastNewGameTime + 2 > Time.time)
        {
            return;
        }
        lastNewGameTime = Time.time;

        //int lastWinState = lastPlayedLv > 0 ? 0 : 1;
        //MTTracker.Instance.Track(SoliTrack.play, StatisticsMgr.current.GetGameWons(2), oldLv.ToString(), "1",lastWinState.ToString());
        playingLv = SettingMgr.current.GetPlayingLv();

        LoadLvFromFile(playingLv);



    }

    void DoChallenge()
    {
        if (lastNewGameTime + 2 > Time.time)
        {
            return;

        }
        lastNewGameTime = Time.time;
        LoadLvFromFile(_challengeLv);
       
        
    }

    public void CallWinDealGame()
    {
        _currentPlayState = StartState.Windeal;
        StartGame();
    }

    public void CallNewGame()
    {
        _currentPlayState = StartState.NewGame;
       
        StartGame(); ;
    }

    public void CallReplayState()
    {
        _currentPlayState = StartState.Replay;
        StartGame();
    }

    public GameObject ConfirmWindealWindow;
    void DoWinDealGame()
    {
        if (lastNewGameTime + 2 > Time.time)
        {
            return;
        }
        lastNewGameTime = Time.time;

        if(SettingMgr.current._state == PlayState.Vegas)
        {
            SettingMgr.current.ShowVegasWindeal();
            return;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //Do sth.
            ConfirmWindealWindow.SetActive(true);
        }
        else
        {
            playingLv = SettingMgr.current.GetPlayingLv();

            LoadLvFromFile(playingLv);
            //int lastWinState = lastPlayedLv > 0 ? 0 : 1;
           // MTTracker.Instance.Track(SoliTrack.play,StatisticsMgr.current.GetGameWons(2), oldLv.ToString(), "3",lastWinState.ToString());
        }
    }

    public void CloseWinDealGame()
    {
        ConfirmWindealWindow.SetActive(false);
    }
    void CleanBoard()
    {
        _pileReadyList.Clear();
        _pileList.Clear();
        _CardActions.Clear();

        foreach (var card in _targetList)
        {
            card.nextCard = null;
        }

        foreach (var card in _platformList)
        {
            card.nextCard = null;
        }


        CleanCardPlatform();
        foreach (var curCard in CardList)
        {
            var curCardSc = curCard;
            curCardSc.cardState = CardState.None;
            curCardSc.preCard = null;
            curCardSc.nextCard = null;
            curCard.StopAllActions();
            curCard.transform.position = start.position;
            curCard.transform.parent = Root;
        }

        foreach (var platform in PlatformCardList)
        {
            platform.nextCard = null;
        }

        for (int i = 0; i < CardList.Count; i++)
        {
            var curCard = CardList[i];

            curCard.transform.eulerAngles = new Vector3(0, 180f, 0);
            curCard.transform.position = start.position;
        }
    }

    float lastResetGame = -10f;
    public void ResetGame()
    {
        BlockBtn(3f);
        if (lastResetGame + 2 > Time.time)
        {
            return;
        }

        if (ChallengeMgr.current.ChallengeActive) {
            _gameState = new ChallengeState();
        } else
        {
            if (_settingMgr._state == PlayState.Normal)
            {
                _gameState = new NormalState();
            }
            else 
            {
                _gameState = new VegasState();
            }
        }
        testLv.text = testLv.text + _gameState.GetStateName();
     

        _gameState.Init();
        HintMgr.current.StopAllHint();
        if (SettingMgr.current.PlayNoAdNum > 0)
        {
            SettingMgr.current.PlayNoAdNum--;
            if (SettingMgr.current.PlayNoAdNum <= 0)
            {
                AdMgr.PreloadAdmobInterstitial();
            }
            SettingMgr.current.SaveToFile();
        }

        resetCount++;

        bool isGroupD = false;// SettingMgr.current.GroupId % 2 == 0;



        //if (SettingMgr.current.PlayNoAdNum > 0)
        //{
        //    AdMgr.TrackApplovin("-1");
        //    AdMgr.TrackAdMob("-1");
           
        //}
        //else
        //{


            if (AdMgr.IsAdmobInterstitialReady())
            {

                AdMgr.ShowAdmobInterstitial();
            }
            //else
            //{
            //    //AdMgr.TrackAdMob("-2");
            //}
        //}





        AdMgr.ShowAdmobBanner();

        // AdMgr.ShowNativeBanner();



        AutoCollectBtn.SetActive(false);
        isAutoCollecting = false;
        ToolbarMgr.current.HidePlayMenu();
        lastResetGame = Time.time;
        CleanScore();
        CleanBoard();
        UpdateUI();
        UpdateMoves();
        UpdateTime();
        int index = 0;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                CardPlatform[i].Add(CardList[index].gameObject);
                index++;
            }
        }
        if (_settingMgr.Draw3 == 1)
        {
            flipNum = 3;
            StatisticsMgr.current.AddGamePlayed(DrawEnum.Draw3);
        }
        else
        {
            flipNum = 1;
            StatisticsMgr.current.AddGamePlayed(DrawEnum.Draw1);
        }

        CleanScore();
        PlayDealCard();

        string trackMode = GetTrackMode();
        var curQueue = SettingMgr.current._trackQueue;
        if (curQueue.Count > 0)
        {
            var lastTrackData = curQueue.ToArray()[curQueue.Count - 1];
           // MTTracker.Instance.Track(SoliTrack.levelStart, StatisticsMgr.current.WinsCount(), lvid.ToString(), trackMode, lastTrackData.LvId, lastTrackData.trackMode, lastTrackData.Win, _currentPlayState.ToString());
        }
        _currentTrackData = new TrackData();
        SettingMgr.current._trackQueue.Enqueue(_currentTrackData);
        _currentTrackData.trackMode = GetTrackMode();
        _currentTrackData.StartStateName = _currentPlayState.ToString();
        _currentTrackData.LvId = lvid.ToString();

        _currentTrackData.Win = "0";

        StartCoroutine(SetPile(index));
    }

    TrackData _currentTrackData;

    string GetTrackMode()
    {
        PlayState trackState;
        if (ChallengeMgr.current.ChallengeActive)
        {

            trackState = PlayState.Challenge;
        }
        else
        {
            if (_settingMgr._state == PlayState.Normal)
            {

                trackState = PlayState.Normal;
            }
            else
            {

                trackState = PlayState.Vegas;
            }
        }

       return trackState + "-" + flipNum.ToString() + "-" + SettingMgr.current.VegasCumulative.ToString();
    }



    void CleanScore()
    {
        _gameState.GameTime = 0f;
        StopTick();
       
        isStartTick = false;
        
    }

    public void DoStartTimeTick()
    {
        if (!isStartTick)
        {
            _timeTick = TimeTick();
            StartCoroutine(_timeTick);
            isStartTick = true;
        }
    }

    bool isStartTick = false;

    public void LoadGame(MTJSONObject js)
    {

        int index = 0;
        for (int i = 0; i < 7; i++)
        {
            var curList = js.Get(i.ToString()).list;

            for (int j = 0; j < curList.Count; j++)
            {
                var curCardJs = curList[j];
                CardColor curColor = (CardColor)curCardJs.GetInt("CardColor");
                int curNum = curCardJs.GetInt("CardNum");
                var curCard = CardList[index].GetComponent<Card>();
                curCard.cardColor = curColor;
                curCard.CardNum = curNum;
                curCard.UpdateCardView();
                index++;
            }

        }

        var pileList = js.Get("7").list;
        for (int i = 0; i < pileList.Count; i++)
        {
            var curCardJs = pileList[i];
            CardColor curColor = (CardColor)curCardJs.GetInt("CardColor");
            int curNum = curCardJs.GetInt("CardNum");
            var curCard = CardList[index].GetComponent<Card>();

            curCard.cardColor = curColor;
            curCard.CardNum = curNum;
            curCard.UpdateCardView();
            index++;
        }


        ResetGame();
    }

    void DoPlayDeal()
    {

        _soundMgr.Play_new_game(0.8f);
        BlockBtn(3f);
        if (AdMgr.IsAdmobInterstitialReady())
        {
            AdMgr.ShowAdmobInterstitial();
        }
        
        for (int i = 0; i < CardPlatform.Length; i++)
        {
            var curCardList = CardPlatform[i];
            for (int j = 0; j < curCardList.Count; j++)
            {
                CardAbstract root = PlatformPlace[i].GetComponent<CardAbstract>();
                var curCard = curCardList[j];
                var tarPos = root.GetNextPos(j + 1);

                MTSequence seq = null;

                if (j == curCardList.Count - 1)
                {
                    seq = new MTSequence(
                        new MTDelayTime(1f),
                        new MTMoveToWorld(i * 0.05f + j * 0.04f, tarPos),
                        new MTRotateTo(0.2f, new Vector3(0, 0, 0)),
                        new MTCallFunc(() => curCard.transform.eulerAngles = Vector3.zero));
                }
                else
                {
                    seq = new MTSequence(new MTDelayTime(1f), new MTMoveToWorld(i * 0.05f + j * 0.04f, tarPos));
                }
                CardAbstract preCard = root;
                if (j != 0)
                {
                    preCard = curCardList[j - 1].GetComponent<CardAbstract>();

                }

                var cardSc = curCard.GetComponent<CardAbstract>();
                cardSc.cardState = CardState.InPlatform;
                cardSc.preCard = preCard;
                preCard.nextCard = cardSc;
                curCard.RunActions(seq);
            }
        }
    }

    void HideToolBar(float t)
    {
        StartCoroutine(DoHideToolBar (t));
    }
    IEnumerator DoHideToolBar(float t)
    {
        Toolbar.SetActive(false);
        yield return new WaitForSeconds(t);
        Toolbar.SetActive(true);
    }
    void PlayDealCard()
    {
        

        DoPlayDeal();
        BlockBtn(3);
        StartCoroutine(PreloadAD());
        //StartCoroutine(DoPlayDeal());
    }

    IEnumerator PreloadAD()
    {
        yield return new WaitForSeconds(2f);
        AdMgr.PreloadAdmobInterstitial();
    }

    public void AddAction(CardAction action)
    {
        _CardActions.Add(action);
    }

    HashSet<Card> _pressingCard = new HashSet<Card>();


    bool hideAd = false;
    public void HideAd()
    {
        hideAd = true;
    }


    public void AddCardPress(Card card)
    {
        _pressingCard.Add(card);
    }

    public void RemovePress(Card card)
    {
        _pressingCard.Remove(card);
    }
    float lastReverseActionTime = 0;
    const float REVERSE_INTERVAL = 0.2f;
    public void ReverseAction()
    {
        if (IsBlockUIBtn())
        {
            return;
        }

        BlockBtn(0.3f);

        if (lastReverseActionTime + REVERSE_INTERVAL > Time.time)
        {
            return;
        }
        lastReverseActionTime = Time.time;

        if (_pressingCard.Count> 0)
        {
            return;
        }
        if (!undoAble)
        {
            return;
        }
        if (isAutoCollecting)
        {
            return;
        }
        if (_CardActions.Count > 0)
        {
            var lastAction = _CardActions[_CardActions.Count - 1];
            _CardActions.Remove(lastAction);
            lastAction.ReverseAction();
            HintMgr.current.StopAllHint();
            _gameState.UndoTimes++;
        }

    }

    public void RefreshCardView()
    {
        foreach (var card in CardList)
        {
            var cardSc = card.GetComponent<Card>();
            if (cardSc != null)
            {
                cardSc.UpdateCardView();
            }
        }
    }

    IEnumerator SetPile(int index)
    {
        yield return new WaitForSeconds(2f);
        for (int i = index; i < CardList.Count; i++)
        {
            var curCard = CardList[i];
            _pileList.Add(curCard);

            curCard.transform.position = GetPileCardPos();
            curCard.transform.parent = Pile.transform;
            var cardSc = curCard;//.GetComponent<CardAbstract>();
            cardSc.cardState = CardState.InPile;
        }

    }
    public bool isAutoCollecting = false;
    public bool CheckSuccess()
    {
        if (isCardPressing())
        {
            return false;
        }
        if (_pileList.Count != 0 || _pileReadyList.Count != 0)
        {
            return false;
        }

        for (int i = 0; i < _platformList.Count; i++)
        {
            if (_platformList[i].GetTopCard() != _platformList[i])
            {
                return false;
            }
        }

        return true;

    }

    public bool isAutoFinish = true;

    public bool CheckMetAutoCollect()
    {

        //if (_pileList.Count != 0 || _pileReadyList.Count != 0)
        //{
        //    return false;
        //}

        for (int i = 0; i < _platformList.Count; i++)
        {
            var curPlat = _platformList[i];
            if (curPlat.nextCard != null)
            {
                if (!curPlat.nextCard.isUp())
                {
                    
                    return false;
                }
            }
        }

        return true;
    }

    public void TryCheckWin()
    {
        if (!LevelMgr.current.isAutoCollecting)
        {
            if (LevelMgr.current.CheckSuccess())
            {
                LevelMgr.current.ToWinState();
            }

            if (LevelMgr.current.CheckMetAutoCollect())
            {
                LevelMgr.current.ShowAutoCollectBtn();
            }

        }
    }


    public Vector3 GetPileCardPos()
    {

        return Pile.transform.position + Vector3.back * 0.1f;
    }

    public int flipNum = 3;
    public void FlipPile()
    {
        var flipCardList = new List<CardAbstract>();

        for (int i = _pileList.Count - 1, count = flipNum; i >= 0 && count > 0; i--, count--)
        {
            flipCardList.Add(_pileList[i]);
        }

        if (flipCardList.Count > 0)
        {

            var flipCardAction = new FlipCardAction();
            flipCardAction.Init(flipCardList);
            flipCardAction.DoAction();
            AddAction(flipCardAction);
        }


    }

    public void RefreshPileReady(bool isInstant = false)
    {
        //for(int i  = 0; i < _pileReadyList.Count;i ++)
        //{
        //    _pileReadyList[i].transform.parent = PileReadyTrans[2];
        //}

        for (int i = _pileReadyList.Count - 1, j = 2; i >= 0; i--, j--)
        {
            var curPileCard = _pileReadyList[i];
            if (j == 2)
            {
                curPileCard.transform.position += Vector3.back;
            }
            if (j >= 0)
            {
                var curPileTrans = PileReadyTrans[j];
               

                curPileCard.transform.parent = curPileTrans;
                //curPileCard.transform.eulerAngles = Vector3.zero;
                curPileCard.BlockTouch(FLIP_TIME);
                curPileCard.StopAllActions();
                if (isInstant) {
                    curPileCard.transform.localPosition = Vector3.back * 0.1f;
                    curPileCard.transform.eulerAngles = Vector3.zero;
                }
                else
                {
                   // curPileCard.BlockTouch(FLIP_TIME + 0.01f); 
                    curPileCard.RunAction(new MTSpawn( new MTRotateTo(FLIP_TIME, Quaternion.identity), new MTMoveTo(FLIP_TIME, Vector3.back * 0.1f)));
                } 
                
                
            }
            else
            {
                curPileCard.BlockTouch(FLIP_TIME);
                curPileCard.transform.parent = PileReadyTrans[0];
                var tarPos = PileReadyTrans[0].position + new Vector3(0, 0, 50 - i);
                if (isInstant)
                {
                    curPileCard.transform.position = tarPos;
                }
                else
                {
                    curPileCard.RunAction(new MTMoveToWorld(FLIP_TIME, tarPos));
                }
                //curPileCard.transform.eulerAngles = new Vector3(0, 180f, 0);
            }

        }
    }

    const float FLIP_TIME = 0.15f;
    public void RefreshPile()
    {
        HintMgr.current.StopAllHint();
        DoStartTimeTick();
        if (_pileList.Count == 0)
        {
            if (_gameState.RemainRefreshTime() > 0)
            {
               
                var refreshPile = new RefreshPileAction();
                refreshPile.DoAction();
                _gameState.ReduceRefreshTime();
                _CardActions.Add(refreshPile);
            }
        }
        else
        {
            FlipPile();
        }

    }


    public void RemoveFromPile(CardAbstract card)
    {

        _pileList.Remove(card);
        _pileReadyList.Remove(card);


    }

    public int pileIndex = 0;

    public List<CardAbstract> _pileList = new List<CardAbstract>();


    bool undoAble = true;
    public void DisableUndo(float t)
    {
        undoAble = false;
        StartCoroutine(UndoReset(t));

    }
    IEnumerator UndoReset(float t)
    {
        yield return new WaitForSeconds(t);
        undoAble = true;
    }

    public void ToPlayState()
    {
        fsm.ChangeState(LevelState.Playing);
    }

    public void ToWinState()
    {
        fsm.ChangeState(LevelState.Win);
    }

    
    public void WinContinuePress()
    {
        WinCanvas.gameObject.SetActive(false);
        for (int i = 0; i < CardList.Count; i++)
        {
            var curCard = CardList[i];
            curCard.transform.position = Vector3.down * 100;
        }

    }

    IEnumerator Load_Enter()
    {
        LoadArchive();

        yield return null;
        if (SettingMgr.current.RightHanded != 1)
        {
        RightHandMgr.current.ResetPos();
        }
        fsm.ChangeState(LevelState.Playing);
    }



    IEnumerator Playing_Enter()
    {
        MenuCanvas.SetActive(true);
        WinCanvas.gameObject.SetActive(false);

        if (_currentPlayState != StartState.Load)
        {
            BlockBtn(4);
        }
        
        yield return  new WaitForSeconds(1f);
        Toolbar.SetActive(true);
        StartGame();
        

    }
  
    void StartGame()
    {
        ToolbarMgr.current.HidePlayMenu();
     
        if (GetLevelState() != LevelState.Playing)
        {
            fsm.ChangeState(LevelState.Playing);
            return;
        }

        if (_currentPlayState == StartState.NewGame)
        {
            DoNewGame();
        }else if(_currentPlayState == StartState.Replay)
        {
            DoReplayGame();
        }else if(_currentPlayState == StartState.Windeal)
        {
            DoWinDealGame();
        }
        else if(_currentPlayState == StartState.Load)
        {
            //Do nothing
        }else if(_currentPlayState == StartState.Challenge)
        {
            DoChallenge();
        }
    }

    StartState _currentPlayState = StartState.NewGame;

    public enum StartState
    {
        NewGame,
        Replay,
        Windeal,
        Load,
        Challenge
    }



    const int MAX_NUM = 50;
    const float Delay_Time = 0.6f;




    const float ANIM_PLAYTIME = 5f;

    bool isPlayingSuccess = false;
    IEnumerator Win_Enter()
    {
        StopTick();
        Toolbar.SetActive(false);
        ChallengeMgr.current.SetWin();
        isPlayingSuccess = true;
        PlayWinAnim();
        AutoCollectBtn.SetActive(false);
        yield return new WaitForSeconds(ANIM_PLAYTIME);
        //Toolbar.SetActive(true);
        isPlayingSuccess = false;
        MenuCanvas.SetActive(false);
        WinCanvas.gameObject.SetActive(true);
        StopTick();
        WinCanvas.ShowScores();
        WinCanvas.SetChallangeWin(ChallengeMgr.current.ChallengeActive);


       // lastPlayedLv = -1;
        _soundMgr.PlayWinMusic();
        var drawEn = flipNum == 1 ? DrawEnum.Draw1 : DrawEnum.Draw3;

        StatisticsMgr.current.AddGameWons(drawEn);
        StatisticsMgr.current.AddFastestWin(drawEn, (int)_gameState.GameTime);
        StatisticsMgr.current.AddFewestMoves(drawEn, _gameState.Moves);
        StatisticsMgr.current.AddHighScore(drawEn, _gameState.GetScore());
        StatisticsMgr.current.SaveToFile();

        var trackMode = GetTrackMode();
        //MTTracker.Instance.Track(SoliTrack.levelEnd, StatisticsMgr.current.WinsCount(), lvid.ToString(), trackMode, _gameState.GameTime.ToString(), _gameState.Moves.ToString(), _gameState.UndoTimes.ToString(), _gameState.GetScore().ToString());

        var trackQueue = SettingMgr.current._trackQueue;
        if (trackQueue.Count > 0)
        {
            var lastTrack = trackQueue.ToArray()[trackQueue.Count - 1];
            lastTrack.Win = "1";
        }
        SettingMgr.current.SaveTrack();
        // yield return null;


    }

    public GameObject AutoCollectBtn;
    public void AutoCollect()
    {
        if (isCardPressing())
        {
            return;
        }



        HintMgr.current.StopAllHint();


        ToolbarMgr.current.HidePlayMenu();
        ToolbarMgr.current.HideSettingMenu();
        Toolbar.SetActive(false);
        AutoCollectBtn.SetActive(false);
        isAutoCollecting = true;
        StopTick();
        CleanPressingCard();
        StartCoroutine(AutoPut());

    }

    void StopTick()
    {
        if (_timeTick != null)
        {
            StopCoroutine(_timeTick);
        }
        isStartTick = false;

    }

    public void ShowAutoCollectBtn()
    {
        HintMgr.current.StopAllHint();
        if (!isPlayingSuccess)
        {
            AutoCollectBtn.SetActive(true);
        }
    }


    public void HideAutoCollectBtn()
    {
        AutoCollectBtn.SetActive(false);
    }


    IEnumerator AutoPut()
    {
        CardAbstract moveCard = null;
        CardAbstract tarCard = null;
        while (GetTargetsTopCard(ref moveCard, ref tarCard))
        {
            tarCard.PutCard(moveCard);
            yield return new WaitForSeconds(0.11f);

        }
        yield return new WaitForSeconds(0.3f);
        isAutoCollecting = false;


        LevelMgr.current.ToWinState();


    }

    bool GetTargetsTopCard(ref CardAbstract moveCard, ref CardAbstract tarCard)
    {
        for (int i = 0; i < _platformList.Count; i++)
        {
            var curCard = _platformList[i].GetTopCard();
            for (int j = 0; j < _targetList.Count; j++)
            {
                var curTar = _targetList[j].GetTopCard();

                if (curTar.isCardPutable(curCard))
                {
                    moveCard = curCard;
                    tarCard = curTar;
                    return true;
                }


            }


        }


        for(int i = 0; i < _pileReadyList.Count; i ++)
        {
            var curCard = _pileReadyList[i];
            for (int j = 0; j < _targetList.Count; j++)
            {
                var curTar = _targetList[j].GetTopCard();

                if (curTar.isCardPutable(curCard))
                {
                    moveCard = curCard;
                    tarCard = curTar;
                    _pileReadyList.Remove(curCard);
                    curCard.transform.rotation = Quaternion.identity;
                    RefreshPile();
                    return true;
                }


            }
        }

        for(int i = 0; i < _pileList.Count; i ++)
        {
            var curCard = _pileList[i];
            for (int j = 0; j < _targetList.Count; j++)
            {
                var curTar = _targetList[j].GetTopCard();

                if (curTar.isCardPutable(curCard))
                {
                    moveCard = curCard;
                    tarCard = curTar;
                    _pileList.Remove(curCard);
                    RefreshPile();
                    return true;
                }


            }
        }



            return false;
    }

    const float dis = 2f;
    public void PlayWinAnim()
    {
        Action[] animAction = new Action[2] { PlayAnim1, PlayAnim2 };
        var index = MTRandom.GetRandomInt(0, animAction.Length - 1);
       // Debug.Log("index " + index);
        animAction[index]();
        //PlayAnim2();
      
    }

    const int moveTimes = 4;
   int[] GetArr(int index)
    {
        var a = new int[moveTimes];
        int x = index;
        for(int i = 0;i < moveTimes; i++)
        {
            if(x >= 4)
            {
                x = 0;
            }

            a[i] = x;
            x++;
        }
        return a;
    }

    void ResetCardState()
    {
        for (int i = 0; i < CardList.Count; i++)
        {
            var curCard = CardList[i];
            curCard.BlockTouch(ANIM_PLAYTIME);
            curCard.transform.parent = Root;

            curCard.transform.localEulerAngles = Vector3.zero;
        }

        CardList.Sort((l, r) => l.CardIndex.CompareTo(r.CardIndex));
    }

    void PlayAnim1()
    {
        ResetCardState();

        Vector3 leftUp = new Vector3(-1 * dis, 1 * dis, 0);
        Vector3 leftDown = new Vector3(-1 * dis, -1 * dis, 0);
        Vector3 rightUp = new Vector3(1 * dis, 1 * dis, 0);
        Vector3 rightDown = new Vector3(1 * dis, -1 * dis, 0);
        Vector3[] arr = new Vector3[] { leftUp, leftDown, rightUp, rightDown };
        int indx = 0;
        for (int i = 0; i < CardList.Count; i++)
        {
            var curCard = CardList[i];

            if (indx >= 4)
            {
                indx = 0;
            }

            var arrTar = GetArr(indx);
            indx++;
            curCard.RunActions(
                new MTDelayTime(i * 0.02f),
                new MTMoveTo(0.3f, Vector3.zero + Vector3.back * 0.1f * i),
                new MTDelayTime(1.5f),
                new MTMoveTo(0.3f, arr[arrTar[0]] + Vector3.back * 0.1f * i),
                new MTMoveTo(0.3f, arr[arrTar[1]] + Vector3.back * 0.1f * i),
                new MTMoveTo(0.3f, arr[arrTar[2]] + Vector3.back * 0.1f * i),
                new MTMoveTo(0.3f, arr[arrTar[3]] + Vector3.back * 0.1f * i),
                new MTDelayTime(0.5f)
               // new MTMoveTo(0.2f,new Vector3(0,-10f,0))

                );

        }
    }

    public void MoveRootDown(float f = 90f)
    {
        //   Root
        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
 
        float h = worldScreenHeight * f / Screen.height;
        Root.position -= new Vector3(0, h, 0);
    }


    void PlayAnim2()
    {
        ResetCardState();
        Vector3 leftUp = new Vector3(-1 * dis, 1 * dis, 0);
        Vector3 leftDown = new Vector3(-1 * dis, -1 * dis, 0);
        Vector3 rightUp = new Vector3(1 * dis, 1 * dis, 0);
        Vector3 rightDown = new Vector3(1 * dis, -1 * dis, 0);

        Vector3 leftMid = new Vector3(-1.5f * dis, 0,0);
        Vector3 rightMid = new Vector3(1.5f * dis, 0, 0);

        Vector3 upMid = new Vector3(0, 1.5f * dis, 0);
        Vector3 downMid = new Vector3(0, -1.5f * dis, 0);

        Vector3[] arr = new Vector3[] { leftUp, leftDown, rightUp, rightDown };
        int indx = 0;
        for (int i = 0; i < CardList.Count; i++)
        {
            var curCard = CardList[i];

            if (indx >= 4)
            {
                indx = 0;
            }

            var arrTar = GetArr(indx);
            indx++;
            Vector3 mid1;
            Vector3 mid2;
            if(indx %2 == 0)
            {
                mid1 = leftMid;
                mid2 = rightMid;
            }else
            {
                mid1 = rightMid;
                mid2 = leftMid;
            }
            curCard.RunActions(
                new MTDelayTime(i * 0.02f),
                new MTMoveTo(0.3f, upMid),
                new MTMoveTo(0.3f, arr[arrTar[0]] + Vector3.back * 0.1f * i),
                new MTMoveTo(0.3f, arr[arrTar[1]] + Vector3.back * 0.1f * i),
                new MTMoveTo(0.3f, mid2),
                new MTDelayTime(0.5f),
                new MTMoveTo(0.3f, arr[arrTar[2]] + Vector3.back * 0.1f * i),
                new MTMoveTo(0.3f,downMid),
                new MTMoveTo(0.3f, arr[arrTar[3]] + Vector3.back * 0.1f * i),
                new MTMoveTo(0.3f,mid1),
               
                new MTMoveTo(0.3f, Vector3.zero + Vector3.back * 0.1f * i),
                new MTDelayTime(1f)
    
               // new MTMoveTo(0.2f, new Vector3(0, 10f, 0))

                );

        }
    }

    void PlayAnim3()
    {
        ResetCardState();
    }

    public void DebugWin()
    {
      //  CleanBoard();
 
        for(int i = 0; i < CardList.Count;i++)
        {
            var curCard = CardList[i];

            curCard.UpdateCardByNum(i);
            if (i != CardList.Count - 1)
            {
                _targetList[i / 13].GetTopCard().PutCard(curCard);
            }
        }
    }


    public void onInterstitialEvent(string eventName, string msg)
    {
        ////Debug.Log("handler onAdmobEvent---" + eventName + "   " + msg);
        //if (eventName == AdmobEvent.onAdLoaded)
        //{
        //    //AdMgr.ShowAdmobInterstitial();
        //    AdMgr.TrackAdMob("3");
        //}
        //else if (eventName == AdmobEvent.onAdClosed)
        //{


        //    //AdMgr.PreloadAdmobInterstitial();

        //    AdMgr.TrackAdMob("1");

        //}
        //else if (eventName == AdmobEvent.onAdOpened)
        //{
         
        //    AdMgr.TrackAdMob("0");
        //} else if (eventName == AdmobEvent.onAdFailedToLoad)
        //{
        //    //AdMgr.PreloadAdmobInterstitial();
        //    AdMgr.TrackAdMob("2");
        //}

    }
    public void onBannerEvent(string eventName, string msg)
    {
        Debug.Log("handler onAdmobBannerEvent---" + eventName + "   " + msg);
    }
    public void onRewardedVideoEvent(string eventName, string msg)
    {
        Debug.Log("handler onRewardedVideoEvent---" + eventName + "   " + msg);
    }
   public void onNativeBannerEvent(string eventName, string msg)
    {
        Debug.Log("handler onAdmobNativeBannerEvent---" + eventName + "   " + msg);
    }



}

