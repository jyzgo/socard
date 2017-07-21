using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardEdit : MonoBehaviour
{
    public CardColor cardColor;

    [Range(1, 13)]
    public int cardNum = 1;

    public SpriteRenderer Icon;
    public SpriteRenderer Icon2;
    public Text text;
    // Use this for initialization

    void Start()
    {
        CardEditMgr.current._list.Add(this);
        UpdateView();
    }
    public void UpdateView()
    {
        Sprite sp = null;
        switch (cardColor)
        {
            case CardColor.Club:
               sp = ResMgr.current.club;
                break;
            case CardColor.Heart:
                sp = ResMgr.current.heart;
                break;
            case CardColor.Spade:
                sp = ResMgr.current.spade;
                break;
            case CardColor.Dimaond:
                sp = ResMgr.current.diamond;
                break;
            default:
                break;
        }

        Icon.sprite = sp;
        Icon2.sprite = sp;
        string cardstr = string.Empty;
        switch (cardNum)
        {
            case 1:
                cardstr = "A";
                break;
            case 11:
                cardstr = "J";
                break;
            case 12:
                cardstr = "Q";
                break;
            case 13:
                cardstr = "K";
                break;
            default:
                cardstr = cardNum.ToString();
                break;
        }

        text.text = cardstr.ToString();


    }

    void OnMouseDrag()
    {


        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = new Vector3( objPos.x,objPos.y,-9f);


    }
    HashSet<CardLoader> _set = new HashSet<CardLoader>();
    void OnMouseUp()
    {

        if(currentLoader!= null)
        {
            Debug.Log("load " + currentLoader.name);
            currentLoader.AddCard(this);


            CardEditMgr.current._list.Remove(this);
            if (holdLoader != null)
            {
                holdLoader.RemoveCard(this);
                
            }
            holdLoader = currentLoader;
        }


    }

    CardLoader holdLoader;


    CardLoader currentLoader;
    void OnTriggerEnter2D(Collider2D col)
    {
        var loader = col.GetComponent<CardLoader>();
        if(loader != null)
        {
            currentLoader = loader;
            _set.Add(loader);
        }

    }


    void OnTriggerExit2D(Collider2D col)
    {
        var loader = col.GetComponent<CardLoader>();
        if (loader != null)
        {
            _set.Remove(loader);
            if (_set.Count == 0)
            {
                currentLoader = null;
            }
        }
    }
}

