using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using MTUnity.Actions;
using UnityEngine.EventSystems;

public enum CardColor
{
    Spade,
    Heart,
    Club,
    Dimaond,
}

public enum CardState
{
    None,
    
    InPile,
    InPileReady,
    InPlatform,
    InTarget
}

public class Card : CardAbstract {

    public SpriteRenderer Center;
    public SpriteRenderer Num;
    public SpriteRenderer Back;
    public SpriteRenderer Front;

    Color centerColor;
    Color numColor;
    Color backColor;
    Color FrontColor;

    Color OriginalNumColor;

   public override void BackColor()
    {
        if (nextCard != null)
        {
            nextCard.BackColor();
        }

        Num.color = OriginalNumColor;
        Back.color = Color.white;
        Front.color = Color.white;
        Center.color = Color.white;

    }

    public override void UpdateBack(Sprite sp)
    {
        Back.flipX = true;
        Back.sprite = sp;
        
    }

    // Use this for initialization
    public override void Start () {
        base.Start();

        _disableTime = Time.time + 0.2f;
        UpdateCardView();
	}

    float _disableTime = 0;

    public override void UpdateCardByNum(int index)
    {
        cardColor = (CardColor)(index / 13);
        CardNum = index % 13 + 1;

        UpdateCardView(); 
    }


    public override void UpdateCardView()
    {
        Center.sprite = ResMgr.current.GetIcon(cardColor);  //  cardData.Center;
                                                                    // Icon.sprite = cardData.Icon;
        Num.sprite = ResMgr.current.GetNum(CardNum);// [CardNum-1];


        if ((int)cardColor % 2 == 0)
        {
            Num.color =  new Color(0.12f, 0.12f, 0.12f);
        }
        else
        {
            Num.color = new Color(0.77f, 0.1f, 0);
        }

        //if (CardNum <= 10)
        //{
        //    Center.transform.localScale = new Vector3(1f, 1f, 1);
        //}else
        //{
        //    Center.transform.localScale = new Vector3(0.8f, 0.8f, 1);
        //}
        OriginalNumColor = Num.color;
        centerColor = Center.color;
        numColor = Num.color;
        backColor = Back.color;
        FrontColor = Front.color;
        gameObject.name = "card " + cardColor.ToString() + CardNum.ToString();

        CardIndex = (int)cardColor * 13 + CardNum - 1;

    }

   

    public override void SetCardAlpha(float r)
    {
        if (nextCard != null)
        {
            nextCard.SetCardAlpha(r);
        }
        if (isUp())
        {
            Front.color = new Color(FrontColor.r, FrontColor.g, FrontColor.b, r);
            Back.color = new Color(backColor.r, backColor.g, backColor.b, 0);
            Center.color = new Color(centerColor.r, centerColor.g, centerColor.b, r);
            Num.color = new Color(numColor.r, numColor.g, numColor.b, r);
        }
        else
        {
            Front.color = new Color(FrontColor.r, FrontColor.g, FrontColor.b, 0);
            Center.color = new Color(centerColor.r, centerColor.g, centerColor.b, 0);
            Num.color = new Color(numColor.r, numColor.g, numColor.b, 0);
            Back.color = new Color(backColor.r, backColor.g, backColor.b, r);

        }

    }



    public override bool isUp()
    {
        if (transform.eulerAngles.y > 160f && transform.eulerAngles.y < 190f)
        {
            return false;
        }
        return true;
    }

    bool isCtrlAble = false;
	// Update is called once per frame
    void OnMouseDrag()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {


            if (LevelMgr.current.isAutoCollecting)
            {
                return;
            }

            if (LevelMgr.current.GetLevelState() != LevelMgr.LevelState.Playing)
            {
                return;
            }

            if (isUp())
            {
                if (isCtrlAble)
                {
                    Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
                    Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePosition);
                    transform.position = new Vector3(objPos.x + offsetPos.x, objPos.y + offsetPos.y, -5f);
                }

            }
        }
    }


    public override bool IsPutAble()
    {
        return isUp();
    }

    float downTime = 0f;
    

    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {

            LevelMgr.current.AddCardPress(this);
            HintMgr.current.StopAllHint();
            if (Time.time < _disableTime)
            {

                return;
            }

            if (LevelMgr.current.GetLevelState() != LevelMgr.LevelState.Playing)
            {
                return;
            }

            if (LevelMgr.current.isAutoCollecting)
            {
                return;
            }


            LevelMgr.current.DoStartTimeTick();
            var curReadyList = LevelMgr.current._pileReadyList;
            isCtrlAble = false;
            if (!curReadyList.Contains(this))
            {
                isCtrlAble = true;
            }
            else
            {
                if (curReadyList[curReadyList.Count - 1] == this)
                {

                    isCtrlAble = true;
                }
            }
            if (isCtrlAble)
            {


                isFloating = true;
                downTime = Time.time;
                Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
                Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePosition);
                offsetPos = transform.position - objPos;
                originalPos = transform.position;

            }
        }
  

    }



    const float pressInterval = 0.1f;
    void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {


            LevelMgr.current.RemovePress(this);
            isFloating = false;
            if (LevelMgr.current.isAutoCollecting)
            {
                isCtrlAble = false;
                return;
            }


            if (isCtrlAble)
            {
                if (Time.time - downTime < 0.2f)
                {

                    Press();
                    BlockTouch();
                }
                else
                {
                    Release();
                }
            }
            isCtrlAble = false;
        }
    }

   // bool disableTouch = false;
    //IEnumerator DisableCard(float t)
    //{
    //    disableTouch = true;
    //    yield return new WaitForSeconds(t * 5);
    //    disableTouch = false;
    //}


    void Press()
    {
        if(!isUp())
        {
            return;
        }
        if(cardState == CardState.InPile)
        {

                if (LevelMgr.current.isAutoFinish)
                {
                    FindBestCard();
                }
                else
                {
                    BackToOriginalPos();
                }

                
            
        }else
        {
            if (LevelMgr.current.isAutoFinish)
            {
                FindBestCard();
            }else
            {
                BackToOriginalPos();
            }
        }

        
    }


    void FindBestCard()
    {
        CardAbstract tarCard = null;
        CardAbstract platCard = null;
        LevelMgr.current.FindTheBestCard(this, ref tarCard, ref platCard);
       
        if(tarCard == null && platCard == null)
        {
            ShowDisableAnim();
            return;
        }

        if(tarCard!= null &&  GetTopCard() == this)
        {
            BlockTouch(0.3f);
            tarCard.PutCard(this);
            return;
        }else
        {
            if(platCard != null)
            {
                BlockTouch(0.3f);
                platCard.PutCard(this);
                return;
            }

        }


        ShowDisableAnim();
        
    }

    void ShowDisableAnim()
    {
        SoundManager.Current.Play_shake_card(0f);
        float shakeTime = 0.03f;
        float shakeDis = 0.05f;
        var finalPos = originalPos;
        if (preCard != null)
        {
            finalPos = preCard.GetNextPos();
        }
        transform.position = finalPos;
        var localPos = transform.localPosition;
        var sequence = new MTSequence(new MTMoveBy(shakeTime, Vector3.left * shakeDis),
            new MTMoveBy(shakeTime, Vector3.right * shakeDis * 2),
            new MTMoveBy(shakeTime, Vector3.left * shakeDis * 2),
            new MTMoveBy(shakeTime, Vector3.right * shakeDis * 2),
            new MTMoveBy(shakeTime, Vector3.left * shakeDis * 2),
            new MTMoveTo(shakeTime, localPos),
            new MTCallFunc(()=>transform.localPosition = localPos),
            new MTCallFunc(()=>this.ResortCardPile())
            );
        this.RunAction(sequence);
        BlockTouch(0.3f);


        //BackToOriginalPos();
    }

    void Release()
    {
       
        GameObject card = null;
        foreach(var en in _enter)
        {
            if(card == null)
            {
                card = en;
            }else
            {

                var curPos = transform.position;
                var curPos2D = new Vector2(curPos.x, curPos.y);

                var otherPos = en.transform.position;
                var other2D = new Vector2(otherPos.x, otherPos.y);

                var cardPos = card.transform.position;
                var cardPos2D = new Vector2(cardPos.x, cardPos.y);

                if (Vector2.Distance(other2D, curPos2D) < Vector2.Distance(cardPos2D, curPos2D))
                {
                    card = en;
                }
            }
        }



        if(card != null)
        {

            var cardSc = card.GetComponent<CardAbstract>();
            cardSc = cardSc.GetTopCard();
           if(cardSc.isCardPutable(this))
            {
                cardSc.PutCard(this);

            }else
            {
               // Debug.Log("card Name is " + card.name);
                BackToOriginalPos();
            }
            
        }else
        {
            BackToOriginalPos();
        }

    }


    public override void BlockTouch(float t = 0.2f)
    {
        float newDisTime = Time.time + t;
        if (newDisTime > _disableTime)
        {
            _disableTime = newDisTime;
        }

    }
    public void BackToOriginalPos(float t =0.2f)
    {
        BlockTouch(t);
        LevelMgr.current.BlockBtn(t + 0.1f);

        this.RunActions(new MTMoveToWorld(0.2f, originalPos),new MTCallFunc(()=>this.ResortCardPile()));

    }


    public override void ResortCardPile()
    {
        if(nextCard != null)
        {
            nextCard.ResortCardPile();
            nextCard.transform.position = GetNextPos();
        }

    }


    public override bool isCardPutable(CardAbstract card)
    {

        if(cardState == CardState.InPile)
        {
           // Debug.Log("in pile ");
            return false;
        }

        if(nextCard != null)
        {
           // Debug.Log("nextcardNot null");
            return false;
        }

        if(IsPutAble() != true)
        {
            //Debug.Log("unputable");
            return false;
        }


        if(cardState == CardState.InPlatform)
        {
            if ((int)cardColor % 2 != (int)card.cardColor % 2 && card.CardNum == CardNum-1 )
            {
                return true;
            }
            else
            {
              //  Debug.Log("not met " + " this card  is " + gameObject.name + "other card is " + card);
                return false;
            }
            
        }else if(cardState == CardState.InTarget)
        {
            if(cardColor == card.cardColor && card.CardNum == CardNum +1)
            {
                return true;
            }
            //Debug.Log("Not met target");
            return false;
        }else if(cardState == CardState.InPile)
        {
            return true;
        }
        //Debug.Log("nothing");
        return false;
    }










}
