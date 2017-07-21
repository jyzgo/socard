using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingInst : MonoBehaviour {

    public RectTransform _rect;

    public Text t;

    void Awake()
    {

        w = 300;
        h = 80;
        x = 55;
        y = 0;
    }

    void OnDisable()
    {
        AdMgr.HideNativeBanner(); 
    }


    public int w;
    public int h;
    public int x;
    public int y;
    void OnEnable()
    {
        UpdateT();

        AdMgr.ShowNativeBanner(w,h,x,y);
    }

    void UpdateT()
    {
        if (t != null)
        {
            t.text = "x:" + x + " y:" + y + " w:" + w + " h:" + h;
        }

    }



    public void addW()
    {
        w += 10;
      
    }
    public void addH()
    {
        h += 10;
    }
    public void addX()
    {
        x += 10;
    }
    public void addY()
    {
        y += 10;
    }

    public void minW()
    {
        w -= 10;
    }

    public void minH()
    {
        h -= 10;
    }
    public void minX()
    {
        x -= 10;
    }
    public void minY()
    {
        y -= 10;
    }

}
