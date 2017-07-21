using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinWindow : MonoBehaviour {

    public Text scoreText;
    public Text timeText;
    public Text moveText;


    public void ShowScores()
    {
        int score=  LevelMgr.current._gameState.GetScore();
        int moves = LevelMgr.current._gameState.Moves;
        string time = LevelMgr.current._gameState.GetTime();

        scoreText.text = score.ToString();
        moveText.text = moves.ToString();
        timeText.text = time;
    }


    public GameObject NewGameBtn;
    public GameObject ContinueBtn;

    public void SetChallangeWin(bool b)
    {
        ContinueBtn.SetActive(b);
        NewGameBtn.SetActive(!b);
    }

    public void Close()
    {
        if (ChallengeMgr.current.ChallengeActive)
        {
            ChallengeMgr.current.ShowChallenge();
        }
        gameObject.SetActive(false);
    }
}
