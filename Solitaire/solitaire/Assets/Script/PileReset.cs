using UnityEngine;
using System.Collections;

public class PileReset : CardAbstract {

	// Use this for initialization
	public override void Start () {
        base.Start();
        lastPresstime = Time.time;
	}

    // Update is called once per frame
    float lastPresstime = 0;
	void OnMouseDown () {
        if(lastPresstime + 0.3f > Time.time)
        {
            return;
        }
        lastPresstime = Time.time;

        LevelMgr.current.BlockBtn(0.3f);
        LevelMgr.current.RefreshPile();

	}

    public override bool isCardPutable(CardAbstract card)
    {
        return false;
    }



    }
