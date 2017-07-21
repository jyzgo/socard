using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.IO;
using MTUnity;
using MTUnity.Utils;


public enum CSaveEnum
{
    StartDay,
    MonthDict,
    Days,
    LifeTimeEarned,
    MonthProcess,
    DayState,
    LastMonth

}

public enum DayState
{
    UNSOLVED ,
    SOVLED_IN_TIME ,
    SOVLED_AFTER 
}

public class ChallengeMgr : MonoBehaviour {
    public GameObject challenge;

    public Image ProgressBar;

    public GameObject ChallengeHelp;

    public Text PlayBoardText;
    public void ShowChallengeHelp()
    {
        ChallengeHelp.SetActive(true);
    }
    public void HideChallengeHelp()
    {
        ChallengeHelp.SetActive(false);
    }

    public GameObject ChallengeDayPrefab;

    public GameObject InnerBoard;
    public Transform Sun;
    // Use this for initialization

    public Text DateText;

    public static ChallengeMgr current;
    List<DayBtn> _btnList = new List<DayBtn>();
    void Awake()
    {
        current = this;

        int k = 0;
        for (int i = 0; i < 42; i++)
        {
            if (i % 7 == 0)
            {
                k = 0;
            }
            var day = Instantiate<GameObject>(ChallengeDayPrefab);
            var daySc = day.GetComponent<DayBtn>();
            _btnList.Add(daySc);
            var rTrans = (RectTransform)day.transform;
            rTrans.SetParent(InnerBoard.transform);
            rTrans.localScale = new Vector3(1, 1, 1);
            rTrans.localPosition = new Vector3(Sun.localPosition.x + 156 * k, 227 - i / 7 * 112, 0);
            k++;

        }
        _currentDate = DateTime.Now.Date;


       
    }


    void Start()
    {
        LoadChallengeData();

        _monthOffset = 0;
        UpdateUI();
    }
    public Sprite PlayBtnSp;
    public Sprite ReplayBtnSp;

    public Image PlayBtnImage;
    public Text PlayBtnText;


    void LoadChallengeData()
    {
        _selectedDate = _currentDate;
        if(!SoFileMgr.Exists(challenFile))
        {
            _loadJson = MTJSONObject.CreateDict();
            _startDate = DateTime.Now.Date;
            _loadJson.Add(CSaveEnum.StartDay.ToString(), _startDate);
            _loadJson.Add(CSaveEnum.LifeTimeEarned.ToString(), _lifeEarn);
            _loadJson.Add(CSaveEnum.LastMonth.ToString(), DateTime.Now);

            AddCurMonthAsNew();
             

        }else
        {


              var str = SoFileMgr.Load(challenFile);
            _loadJson = MTJSON.Deserialize(str);

            var lastMonthO = _loadJson.Get(CSaveEnum.LastMonth.ToString());
            if (lastMonthO != null)
            {
                var lastDate = Convert.ToDateTime(_loadJson.Get(CSaveEnum.LastMonth.ToString()).o);
                int offset = DateTime.Now.Month - lastDate.Month + (DateTime.Now.Year - lastDate.Year) * 12;

                if (offset > 0)
                {
                    var dtarr = new DateTime[offset];
                    for (int i = 0; i < offset; i++)
                    {
                        var now = lastDate.AddMonths(i + 1);
                        AddMonth(now);
                    }


                }
            }else
            {
                AddMonth(DateTime.Now);
                _loadJson.Add(CSaveEnum.LastMonth.ToString(), DateTime.Now);

            }


            _startDate = Convert.ToDateTime(_loadJson.Get(CSaveEnum.StartDay.ToString()).o);


        }
        _loadJson.Set(CSaveEnum.LastMonth.ToString(), DateTime.Now);
        SaveChallengeData();

    }

    bool isSameMonth(DateTime date)
    {
        var now = DateTime.Now;

        if(date.Year == now.Year && date.Month == now.Month)
        {
            return true;
        }
        return false;
    }
    
    void AddMonth(DateTime date)
    {

        string curMonth = ToYearMonth(date);
        if (_loadJson[curMonth] == null)
        {
            var month = MTJSONObject.CreateDict();
            var days = MTJSONObject.CreateDict();
            int dayCount = DateTime.DaysInMonth(date.Year, date.Month);
            for (int i = 0; i < dayCount; i++)
            {
                var n = new DateTime(date.Year, date.Month, i + 1).Date;
                days.Add(ToYMD(n), CreateEmptyDay());
            }
            month.Add(CSaveEnum.Days.ToString(), days);

            int monthProcess = 0;
            month.Add(CSaveEnum.MonthProcess.ToString(), monthProcess);
                       

            _loadJson.Add(curMonth, month);

        }

    }

    void AddCurMonthAsNew()
    {
        AddMonth(DateTime.Now);
    }


    MTJSONObject CreateEmptyDay()
    {
        MTJSONObject js = MTJSONObject.CreateDict();
        js.Add(CSaveEnum.DayState.ToString(), (int)DayState.UNSOLVED);
        return js;
    }

    string ToYearMonth(DateTime dt)
    {
        return dt.Year + "_" + dt.Month;
    }

    string ToYMD(DateTime dt)
    {
        return dt.Year + "_" + dt.Month + "_" + dt.Day;
    }

    MTJSONObject _loadJson;

    void SaveChallengeData()
    {
        SoFileMgr.Save(challenFile, _loadJson.ToString());


    }

    const string challenFile = "challenge.dm";

    DateTime _startDate = new DateTime(2016, 11, 1);
    DateTime _currentDate;
    DateTime _selectedDate;
    int _monthOffset = 0;
    MTJSONObject _curDayJs;
    MTJSONObject _curMonthJs;


    void UpdateUI()
    {
        _currentDate = DateTime.Now.Date.AddMonths(_monthOffset);
        int days = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);


        var firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);

        int firstIndex = (int)firstDayOfMonth.DayOfWeek;
        int lastIndex = firstIndex + days -1;

        _curMonthJs = _loadJson[ToYearMonth(_currentDate)];
       
        _curDayJs = _curMonthJs[CSaveEnum.Days.ToString()];
        for (int i = 0; i < _btnList.Count;i++)
        {
            if(i > lastIndex || i < firstIndex)
            {
                _btnList[i].gameObject.SetActive(false);
            }else
            {
                var curDateBtn = _btnList[i];
                curDateBtn.gameObject.SetActive(true);
                var curDate = new DateTime(_currentDate.Year, _currentDate.Month, i - firstIndex + 1);
                curDateBtn.SetDay(curDate);

                if (curDate <= DateTime.Now.Date)
                {
                    curDateBtn.SetInDate(true);
                    curDateBtn.UpdateDate(_selectedDate, _curDayJs);
                }
                else
                {
                    curDateBtn.SetInDate(false);
                }

            }
        }

        DayState selState =  (DayState)_curDayJs[ToYMD(_selectedDate)].GetInt (CSaveEnum.DayState.ToString());
        if (selState == DayState.UNSOLVED)
        {
            CurrentCrown.sprite = ResMgr.current.CrownShadow;
            CurrentCrownText.text = "Unsolved";

            PlayBtnImage.sprite = PlayBtnSp;
            PlayBtnText.text = "PLAY";
        }
        else if (selState == DayState.SOVLED_IN_TIME)
        {
            CurrentCrown.sprite = ResMgr.current.CrownInTime;
            CurrentCrownText.text = "Solved";
            PlayBtnImage.sprite = ReplayBtnSp;
            PlayBtnText.text = "REPLAY";
        }
        else
        {
            CurrentCrown.sprite = ResMgr.current.CrownAfter;
            CurrentCrownText.text = "Solved";
            PlayBtnImage.sprite = ReplayBtnSp;
            PlayBtnText.text = "REPLAY";
        }

        MaxDay.text = days.ToString();
        DateText.text = _selectedDate.ToString("MMMM");
        PlayBoardText.text = _selectedDate.ToString("MMMM") + " " + _selectedDate.Day;
        _lifeEarn = _loadJson.GetInt(CSaveEnum.LifeTimeEarned.ToString());
        LifeEarnText.text = "Lifetime Earned: " + _lifeEarn;


        int process = _curMonthJs.GetInt(CSaveEnum.MonthProcess.ToString());
        float p = ((float)process / (float)days);
        ProgressBar.fillAmount = p;

        if (process < 10)
        {
            Badge.sprite = ResMgr.current.BadgeShadow;
        }
        else if (process >= 10 && process < 20)
        {
            Badge.sprite = ResMgr.current.Badge3;
        }

        else if (process >= 20 && process < days)
        {
            Badge.sprite = ResMgr.current.Badge2;
        }
        else
        {
            Badge.sprite = ResMgr.current.Badge1;
        }



    }



    int _lifeEarn = 0;
    public Text LifeEarnText;

    public Text MaxDay;

    public void CloseChallenge()
    {
        HideChallenge();
        if(ChallengeActive)
        {
            ChallengeActive = false;
            LevelMgr.current.CallNewGame();
        }
     
    }

    public void HideChallenge()
    {
        challenge.SetActive(false);
    }

    public void ShowChallenge()
    {
        challenge.SetActive(true);

            DayState selState = (DayState)_curDayJs[ToYMD(_selectedDate)].GetInt(CSaveEnum.DayState.ToString());

            if (selState == DayState.UNSOLVED)
            {
                if (!(LevelMgr.current._gameState is ChallengeState))
                {

                    ShowNewChallenge();
                }

            }
        

    }


    public void BtnPress(DateTime date)
    {

        _selectedDate = date;
        UpdateUI();

    }

    public void NextMonth()
    {
        if (DateTime.Now.Date.AddMonths(_monthOffset + 1).Year > DateTime.Now.Date.Year || (DateTime.Now.Date.AddMonths(_monthOffset + 1).Year == DateTime.Now.Year && DateTime.Now.Date.AddMonths(_monthOffset + 1).Month > DateTime.Now.Date.Month)) 
        {
            ShowWrongDate("You cannot view a month in the future.");

        }
        else
        {
            _monthOffset++;
            _selectedDate = DateTime.Now.Date.AddMonths(_monthOffset);
            _selectedDate = new DateTime(_selectedDate.Year, _selectedDate.Month, 1);

            UpdateUI();
        }


    }

    public void PreMonth()
    {
        if (_startDate <= DateTime.Now.AddMonths(_monthOffset - 1) || _startDate.Month <= DateTime.Now.AddMonths(_monthOffset - 1).Month)
        {
            _monthOffset--;
            _selectedDate = DateTime.Now.Date.AddMonths(_monthOffset);
            _selectedDate = new DateTime(_selectedDate.Year, _selectedDate.Month, DateTime.DaysInMonth(_selectedDate.Year,_selectedDate.Month));
            UpdateUI();
        }
        else
        {
            
            ShowWrongDate("There is no previous month to view.");
        }
    }


    public GameObject WrongCanvas;
    public Text wrongDesc;
    public void ShowWrongDate(string str)
    {
        WrongCanvas.SetActive(true);
        wrongDesc.text = str;
    }

    public void HideWrongDate()
    {
        WrongCanvas.SetActive(false);
    }

    [Header("Crown in Playboard")]
    
    public Image CurrentCrown;
    public Text CurrentCrownText;


    [Header("New Challenge")]
    public GameObject NewChallenge;
    public void HideNewChallenge()
    {
        NewChallenge.SetActive(false);
    }

    public void ShowNewChallenge()
    {
        NewChallenge.SetActive(true);
    }

    public void PlayCurrentChallenge()
    {

        HideChallenge();
        HideNewChallenge();
        ChallengeActive = true;

        ToolbarMgr.current.HideChallengeBtn();

        var newD = _selectedDate - new DateTime();
        var lv = (int)newD.TotalDays % 300 + 1;

        LevelMgr.current.CallChallengeLv(lv);
    }

    bool _challengeActive;
    public bool ChallengeActive {
        set {
            _challengeActive = value;
            if(_challengeActive)
            {
                ToolbarMgr.current.HideChallengeBtn();
            } else
            {
                ToolbarMgr.current.ShowChallengeBtn();
            }
        }
        get {
            return _challengeActive;
        }
    } 

    [Header("Badge at top")]
    public Image Badge;


    [Header("Exit Challenge Confirm")]
    public GameObject ExitChaConfirm;
    public void ShowExitChaConfirm()
    {
        ToolbarMgr.current.HidePlayMenu();
        ExitChaConfirm.SetActive(true);
    }

    public void HideExitChaConfirm()
    {
        ExitChaConfirm.SetActive(false);
    }

    public void ExitChallengeMode()
    {
        HideExitChaConfirm();
        ShowChallenge();
        //ChallengeActive = false;
    }



    public void SetWin()
    {
        if (ChallengeActive)
        {
            if (_selectedDate.Date == DateTime.Now.Date)
            {
                
               
                var dateState = (DayState)_loadJson[ToYearMonth(_selectedDate)].Get(CSaveEnum.Days.ToString()).Get(ToYMD(_selectedDate)).GetInt(CSaveEnum.DayState.ToString());

                if (dateState == DayState.UNSOLVED)
                {
                    int current = _loadJson.GetInt(CSaveEnum.LifeTimeEarned.ToString());
                    _lifeEarn = current +1;
                    _loadJson.Set(CSaveEnum.LifeTimeEarned.ToString(), _lifeEarn);
                }
                _loadJson[ToYearMonth(_selectedDate)].Get(CSaveEnum.Days.ToString()).Get(ToYMD(_selectedDate)).Set(CSaveEnum.DayState.ToString(), (int)(DayState.SOVLED_IN_TIME));
            }
            else
            {
                var dateState = (DayState)_loadJson[ToYearMonth(_selectedDate)].Get(CSaveEnum.Days.ToString()).Get(ToYMD(_selectedDate)).GetInt(CSaveEnum.DayState.ToString());

                if (dateState == DayState.UNSOLVED)
                {
                    int current = _loadJson.GetInt(CSaveEnum.LifeTimeEarned.ToString());
                    _loadJson[ToYearMonth(_selectedDate)].Get(CSaveEnum.Days.ToString()).Get(ToYMD(_selectedDate)).Set(CSaveEnum.DayState.ToString(), (int)DayState.SOVLED_AFTER);
                    _lifeEarn = current + 1;
                    _loadJson.Set(CSaveEnum.LifeTimeEarned.ToString(), _lifeEarn);
                }
            }
            UpdateUI();
            SaveChallengeData();
        }
    }

}
