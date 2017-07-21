using UnityEngine;
using System.Collections;

public class ToolbarMgr : MonoBehaviour {

    public static ToolbarMgr current;
    void Awake()
    {
        current = this;
        originalPos = toolbar.localPosition;
        originalLayoutPos = toolLayout.localPosition;
    }

    Vector3 originalPos;
    Vector3 originalLayoutPos;

    public GameObject PlayCanvas;
    public GameObject ChallengeCanvas;

    public void ShowPlayMenu()
    {
        if (!IsAllowPress())
        {
            return;
        }
        SoundManager.Current.Play_ui_open(0);
        //Debug.Log("show play menu");

        if (ChallengeMgr.current.ChallengeActive)
        {
            ChallengeCanvas.SetActive(true);
        }
        else
        {
            PlayCanvas.SetActive(true);
        }
    }

    public void HidePlayMenu()
    {


        if (PlayCanvas == null)
        {
            return;
        }

        if (PlayCanvas.activeSelf)
        {
            SoundManager.Current.Play_ui_close(0);
        }
       // PlayCanvas.SetActive(false);
       PlayCanvas.SetActive(false);
        ChallengeCanvas.SetActive(false);

    }

    [Header("ChallengeBtn")]
    public RectTransform UndoBtn;
    public RectTransform ChallengeBtn;
    public RectTransform SettingBtn;
    public RectTransform PlayBtn;
    public RectTransform HintBtn;
    public void HideChallengeBtn()
    {

        ChallengeBtn.gameObject.SetActive(false);
        float len = UndoBtn.localPosition.x - SettingBtn.localPosition.x;
        var playPos = PlayBtn.localPosition;

        PlayBtn.localPosition = new Vector3(SettingBtn.localPosition.x + len / 3, playPos.y, playPos.z);
        HintBtn.localPosition = new Vector3(SettingBtn.localPosition.x + len / 3 * 2, playPos.y, playPos.z);


    }

    public void ShowChallengeBtn()
    {
        ChallengeBtn.gameObject.SetActive(true);
        float len = UndoBtn.localPosition.x - SettingBtn.localPosition.x;
        var playPos = PlayBtn.localPosition;

        PlayBtn.localPosition = new Vector3(SettingBtn.localPosition.x + len / 2, playPos.y, playPos.z);
        HintBtn.localPosition = new Vector3(SettingBtn.localPosition.x + len / 4 * 3, playPos.y, playPos.z);

    }

    bool IsAllowPress()
    {
        if (LevelMgr.current.IsBlockUIBtn())
        {
            return false;
        }
        if (LevelMgr.current.isCardPressing())
        {
            return false;
        }
        return true;
    }

    public void ShowSettingMenu()
    {
        if (!IsAllowPress())
        {
            return;
        }
        SoundManager.Current.Play_ui_open(0);
        SettingMenu.SetActive(true);
        //Debug.Log("showSettingMenu");
    }

    public void HideSettingMenu()
    {
        if (SettingMenu.activeSelf)
        {
            SoundManager.Current.Play_ui_close(0);
        }
        SettingMenu.SetActive(false);

        //  Debug.Log("hidesettingmenu");
    }

    public void ShowDailyMenu()
    {
        if (!IsAllowPress())
        {
            return;
        }
        ChallengeMgr.current.ShowChallenge(); 

    }

    public void HideDailyMenu()
    {
        DailyMenu.SetActive(false);
    }

   // public GameObject PlayCanvas;
    public GameObject SettingMenu;
    public GameObject DailyMenu;

    public RectTransform toolbar;
    public RectTransform toolLayout;

    public void MoveUp(float height = 90f)
    {
        toolbar.localPosition = new Vector3(originalPos.x, originalPos.y + height, originalPos.z);
        toolLayout.localPosition = new Vector3(originalLayoutPos.x, originalLayoutPos.y + height, originalLayoutPos.z);
    }
}
