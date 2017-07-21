using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MTUnity.Utils;
using System.IO;
using MTUnity.Actions;
using System.Linq;

public class CardEditMgr : MonoBehaviour {


    public static CardEditMgr current;

    void Awake()
    {
        current = this;
    }

	// Use this for initialization
	void Start () {

        int index = 0;
        foreach (CardColor _cardColr in Enum.GetValues(typeof(CardColor)))
        {

            Transform curTar = targets[index];
            index++;

            var curPos = curTar.position;
            for (int i = 0; i < 13; i++)
            {
                var gb = Instantiate<GameObject>(cardProtypePrefab);
                var cardSc = gb.GetComponent<CardPrototype>();
                _hashList.Add(cardSc);
                cardSc.cardState = CardState.InTarget;
                cardSc.cardColor = _cardColr;
                cardSc.CardNum = i + 1;
                cardSc.preCard = cardSc.nextCard = null;
                cardSc.transform.parent = null;
                gb.name = "card " + _cardColr.ToString() + (i + 1).ToString();
                cardSc.UpdateCardView();
                //gb.transform.position = new Vector3(curPos.x, curPos.y, i * -0.1f);
            }
        }

        UpdateOnOffBtn();
        ResetGame();

    }

    void ResetGame()
    {
        for(int i = 0; i < _hashList.Count; i ++)
        {
            int c = i / 13;
            var curCard = _hashList[i];
            Transform curTar = targets[c];
            var curPos = curTar.position;
            curCard.transform.position = new Vector3(curPos.x, curPos.y, i * -0.1f);

        }
    }

    List<CardPrototype> _hashList = new List<CardPrototype>();

    public Transform[] targets;

    public CardLoader[] cardLoaders;

    public GameObject cardProtypePrefab;


    public List<CardEdit> _list = new List<CardEdit>();
	
	// Update is called once per frame
	void Update () {
	
	}

    public string saveFileName = string.Empty;
    public string loadFileName = string.Empty;
    public void GenP()
    {
        Debug.Log("Card not in use " + _list.Count);

        if(_list.Count >0 )
        {
            //return;
        }
        SerializeToJson();
    }

    void SerializeToJson()
    {
        MTJSONObject data = MTJSONObject.CreateDict();
        for(int i = 0; i < cardLoaders.Length;i ++)
        {
            var curCardLoader = cardLoaders[i];
            var curList = curCardLoader._list;
            MTJSONObject listJs = MTJSONObject.CreateList();
            //List<MTJSONObject> listJs = new List<MTJSONObject>();
            for(int j = 0; j <curList.Count;j++)
            {
                MTJSONObject cardJs = MTJSONObject.CreateDict();

                var curCardEdit = curList[j];
                int cardColor = (int)curCardEdit.cardColor;
                int cardNum = curCardEdit.cardNum;
                cardJs.Set("CardColor", cardColor);
                cardJs.Set("CardNum", cardNum);
                listJs.Add(cardJs);
            }
            
            data.Set(i.ToString(), listJs);
        }

        string fileName = Application.persistentDataPath+ "/" + saveFileName;
        Debug.Log("f " + fileName);
        File.WriteAllText(fileName, data.ToString());

    }

    public void LoadFrom()
    {
        string content = File.ReadAllText(Application.persistentDataPath + "/" + loadFileName);
        Debug.Log(" read " + loadFileName + "  : "  + content);
        MTJSONObject js = MTJSON.Deserialize(content);
        
        Debug.Log(" jj " + js.Get("1").ToString());
    }

    public void Save()
    {
        var oldList = new List<CardAbstract>();
        for(int i = 0; i < _hashList.Count; i ++)
        {
            oldList.Add(_hashList[i]);
        }
        var newList = new List<CardAbstract>();
     // oldList = _hashList.Select(item => (CardPrototype)item.Clone()).ToList();
        for (int i  = 0; i < cardLoaders.Length; i ++)
        {
            var curLoader = cardLoaders[i];

            if(OutCard(curLoader) != i + 1)
            {
                Debug.LogError("Num not match " + (i + 1));
                return;
            }

            var nextCard = curLoader.nextCard;
            while(nextCard != null)
            {

                newList.Add(nextCard);
                oldList.Remove(nextCard);
                nextCard = nextCard.nextCard;
            }
             
        }

        oldList.Sort((a, b) => a.transform.position.x .CompareTo( b.transform.position.x));
        newList.AddRange(oldList);

        if(newList.Count != 52)
        {
            Debug.LogError("Total num not match!");
            return;
        }


        string fileName = Application.persistentDataPath + "/" + saveFileName;

        if (File.Exists(fileName))
        {
            Debug.LogError(saveFileName + " already exist!!!");
        }
        else
        {
            string s = string.Empty;
            for (int i = 0; i < newList.Count; i++)
            {
                var curC = newList[i];
                s += (curC.CardIndex + "\n");
            }
            File.WriteAllText(fileName, s);
            Debug.Log("save success " + fileName);
        }
    }

    int OutCard(CardAbstract card)
    {
        int count = 0;
        var nc = card.nextCard;
        while(nc != null)
        {
            count++;

            nc = nc.nextCard;
        }
        return count;
    }

    public void FlipTopCard(int index)
    {

        var curLoader = cardLoaders[index];
        var nc = curLoader.nextCard;
        CardAbstract upCard = null;
        while(nc != null)
        {
            if(nc.isUp())
            {
                upCard = nc;
                break;
            }


            nc = nc.nextCard;

            
        }

        if(upCard != null)
        {
            if(upCard.nextCard != null)
            {
                upCard.nextCard.transform.parent = null;
            }
            upCard.RunAction(new MTRotateTo(0.2f,new Vector3(0, 180, 0)));
        }
    }

    public void BackTopCard(int index)
    {
        var curLoader = cardLoaders[index];
    
        CardAbstract upCard = null;
        var nc = curLoader.GetTopCard();

        while(nc != null && nc != curLoader)
        {
            if(!nc.isUp())
            {
                upCard = nc;
                break;
            }
            nc = nc.preCard;
        }

        if (upCard != null)
        {
            if(upCard.nextCard != null && upCard.nextCard.isUp())
            {

               if(!((int)upCard.cardColor % 2 != (int)upCard.nextCard.cardColor % 2 && upCard.nextCard.CardNum == upCard.CardNum - 1))
                {
                    Debug.LogError("next card is up");


                    return;
                }
          
            }

            upCard.transform.rotation = Quaternion.identity;
            if (upCard.nextCard != null)
            {
                upCard.nextCard.transform.parent = upCard.transform;
            }
            //upCard.RunAction(new MTRotateTo(0.2f, new Vector3(0, 180, 0)));
        }
    }



    public GameObject OnBtn;
    public GameObject OffBtn;

    public bool isAutoFlipOn = false;
    public void SetOn(bool on)
    {
        isAutoFlipOn = on;
        UpdateOnOffBtn();
    }

    void UpdateOnOffBtn()
    {
        OnBtn.SetActive(!isAutoFlipOn);
        OffBtn.SetActive(isAutoFlipOn);

    }

    public CardLoader Pile;

}
