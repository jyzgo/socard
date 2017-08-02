using UnityEngine;
using System.Collections;

public class ResMgr : MonoBehaviour {

    public Sprite heart;
    public Sprite club;
    public Sprite diamond;
    public Sprite spade;
    public GameObject cardEdit;
    public static ResMgr current;
    // Use this for initialization



    public Sprite BtnSpUnSel;
    public Sprite BtnSpSel;
    public Sprite CrownSel;

    public Sprite CrownInTime;
    public Sprite CrownAfter;
    public Sprite CrownShadow;


    public Sprite BadgeShadow;
    public Sprite Badge1;
    public Sprite Badge2;
    public Sprite Badge3;


    public Sprite[] RightSprites;
    public Sprite[] KingsSprites;

    public Sprite[] NumSprites;

    void Awake()
    {

        current = this;
    }




    public Sprite GetIcon( CardColor col,int CardNum)
    {
        if ((int)col > RightSprites.Length - 1)
        {
            Debug.Log("col " + (int)col);
        }
        if(CardNum > 10)
        {
            int index = (int)col * 3  + (CardNum - 11);
            return KingsSprites[index];
        }
        return RightSprites[(int)col];
    }

    public Sprite GetNum( int num)
    {
        return NumSprites[num-1];
    }

    [Header("State Play Menus")]
    public GameObject ChallengePlayMenu;
    public GameObject NormalPlayMenu;
}
