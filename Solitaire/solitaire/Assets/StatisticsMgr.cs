using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using MTUnity.Utils;
using UnityEngine.UI;
using MTXxtea;

public  enum DrawEnum{
    Draw1,
    Draw3
}


public class StatisticsMgr : MonoBehaviour {
    public static StatisticsMgr current;
    void Awake()
    {
        current = this;
        LoadFile();
    }

    public GameObject Window;
    public Text GameWonsText;
    public Text WinRateText;
    public Text GamesPlayedText;
    public Text FastWinText;
    public Text FewestMovesText;
    public Text HighScoreText;


    public void LoadFile()
    {
        var filePath = GetPath();
        if (File.Exists(filePath))
        {
            LoadData();
        }
        else
        {
            SaveToFile();
        }


    }

    string GetPath()
    {
        return Application.persistentDataPath + "/statics.dt";
    }
    
    void LoadData()
    {
        var bt = File.ReadAllBytes(GetPath());
       string content = MTXXTea.DecryptToString(bt, SKEY);
        MTJSONObject stJs = MTJSON.Deserialize(content);
        if (stJs == null)
        {
            SaveToFile();
        }
        else
        {

            GameWons[0] = stJs.GetInt(StatEnum.GameWons.ToString() + "0");
            GameWons[1] = stJs.GetInt(StatEnum.GameWons.ToString() + "1");
            GamePlayed[0] = stJs.GetInt(StatEnum.GamePlayed.ToString() + "0");
            GamePlayed[1] = stJs.GetInt(StatEnum.GamePlayed.ToString() + "1");
            FastestWin[0] = stJs.GetInt(StatEnum.FastestWin.ToString() + "0");
            FastestWin[1] = stJs.GetInt(StatEnum.FastestWin.ToString() + "1");
            FewestMoves[0] = stJs.GetInt(StatEnum.FewestMoves.ToString() + "0");
            FewestMoves[1] = stJs.GetInt(StatEnum.FewestMoves.ToString() + "1");
            HighScore[0] = stJs.GetInt(StatEnum.HighScore.ToString() + "0");
            HighScore[1] = stJs.GetInt(StatEnum.HighScore.ToString() + "1");


        }
    }

    public void SaveToFile()
    {
        MTJSONObject stJs = MTJSONObject.CreateDict();
        stJs.Set(StatEnum.GameWons.ToString() + "0", GameWons[0]);
        stJs.Set(StatEnum.GameWons.ToString() +"1", GameWons[1]);
        stJs.Set(StatEnum.GamePlayed.ToString() +"0", GamePlayed[0]);
        stJs.Set(StatEnum.GamePlayed.ToString() + "1", GamePlayed[1]);
        stJs.Set(StatEnum.FastestWin.ToString() +"0", FastestWin[0]);
        stJs.Set(StatEnum.FastestWin.ToString() + "1", FastestWin[1]);
        stJs.Set(StatEnum.FewestMoves.ToString() +"0",FewestMoves[0]);
        stJs.Set(StatEnum.FewestMoves.ToString() + "1", FewestMoves[1]);
        stJs.Set(StatEnum.HighScore.ToString() + "0", HighScore[0]);
        stJs.Set(StatEnum.HighScore.ToString() + "1", HighScore[1]);
        var bt = MTXXTea.Encrypt(stJs.ToString(), SKEY);
        File.WriteAllBytes(GetPath(), bt);


    }

    const string SKEY = "79710d4c4e7c664785";
    public enum StatEnum
    {
        GameWons,
        GamePlayed,
        FastestWin,
        FewestMoves,
        HighScore
    }


    


    int[] GameWons = new int[2] { 0, 0 };
    int[] GamePlayed = new int[2] { 0, 0 };

    int[] FastestWin = new int[2] { 0, 0 };
    int[] FewestMoves = new int[2] { 0, 0 };
    int[] HighScore = new int[2] { 0, 0 };

    

    const int DRAW_1 = 0;
    const int DRAW_3 = 1;
    const int OVERALL = 2;

    public int GetGameWons(int index)
    {
        if (index < 2)
        {
            return GameWons[index];
        }
        return GameWons.Max();
    }

    public float WinRate(int index)
    {
        if(index < 2)
        {
            return (float)GameWons[index] / (float)GamePlayed[index];
        }

        float rate = 0f;
        for(int i = 0; i < 1; i++)
        {
            var curRate = (float)GameWons[index] / (float)GamePlayed[index];
            if(curRate > rate)
            {
                rate = curRate;
            }
        }
        return rate;
    }

    public int GetGamePlayed(int index)
    {
        if (index < 2)
        {
            return GamePlayed[index];
        }
        return GamePlayed.Max();
    }
    public int GetFastestWin(int index)
    {
        if (index < 2)
        {
            return FastestWin[index];
        }
        return FastestWin.Min();
    }
    public int GetFewestMoves(int index)
    {
        if (index < 2)
        {
            return FewestMoves[index];
        }
        return FewestMoves.Min();
    }
    public int GetHighScore(int index)
    {
        if (index < 2)
        {
            return HighScore[index];
        }
        return HighScore.Max();
    }

    void OnApplicationQuit()
    {
        SaveToFile();
    }



    public void AddGameWons(DrawEnum drawNum)
    {
        GameWons[(int)drawNum] += 1;
        
    }

    public int WinsCount()
    {
        return GameWons[0] + GameWons[1];
    }
    public void AddGamePlayed(DrawEnum drawNum)
    {
        GamePlayed[(int)drawNum] += 1;
        SaveToFile();
    }

    public bool AddFastestWin(DrawEnum drawNum, int costTime)
    {
        int currentFast = FastestWin[(int)drawNum];
        if(costTime < currentFast)
        {
            FastestWin[(int)drawNum] = costTime;
            return true;
        }
        return false;
    }
    public bool AddFewestMoves(DrawEnum drawNum, int costMoves)
    {
        int currentMoves = FewestMoves[(int)drawNum];
        if (costMoves < currentMoves)
        {
            FewestMoves[(int)drawNum] = costMoves;
            return true;
        }
        return false;
    }
    public bool AddHighScore(DrawEnum drawNum, int curScore)
    {
        int curMax = HighScore[(int)drawNum];
        if (curScore  >  curMax)
        {
            HighScore[(int)drawNum] = curScore;
            return true;
        }
        return false;
    }

    public void ShowWindow()
    {
        int draw = SettingMgr.current.Draw3;
        GameWonsText.text = GameWons[draw].ToString();
        if(GamePlayed[draw] == 0)
        {
            WinRateText.text = "0%";
        }else
        {
            float rate = (float)GameWons[draw] / (float)GamePlayed[draw] * 100;
            WinRateText.text =  rate.ToString("0.00") + "%";
        }

        GamesPlayedText.text = GamePlayed[draw].ToString();
        FastWinText.text = FastestWin[draw].ToString();
        FewestMovesText.text = FewestMoves[draw].ToString();
        HighScoreText.text = HighScore[draw].ToString();
        Window.SetActive(true);
    }

    public void HideWindow()
    {
        Window.SetActive(false);
    }

    public GameObject ResetWindow;
    public void ShowResetStatisticWindow()
    {
        ResetWindow.SetActive(true);
    }

    public void HideResetStatisticWindow()
    {
        ResetWindow.SetActive(false);
    }

    public void ResetData()
    {
        ResetWindow.SetActive(false);
        Window.SetActive(false);
        GameWons = new int[2] { 0, 0 };
        GamePlayed = new int[2] { 0, 0 };

        FastestWin = new int[2] { 0, 0 };
        FewestMoves = new int[2] { 0, 0 };
        HighScore = new int[2] { 0, 0 };
        SaveToFile();
    }

}
