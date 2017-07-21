using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardBgSelector : MonoBehaviour {

    public int _index = 0;
    public Image cardBg;
    public GameObject glow;
    public void Init(int index,Sprite sp,bool b)
    {
        _index = index;
        cardBg.sprite = sp;
        ShowGlow(b);
    }

    public void ShowGlow(bool b)
    {
        glow.SetActive(b); 
    }


    public virtual void Select()
    {
        ThemeMgr.current.UpdateCardSelectGlow(_index);
    }


}
