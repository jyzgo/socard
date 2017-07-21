using UnityEngine;

using System.Collections;
using UnityEngine.UI;

public class HintText : MonoBehaviour {

    public Text _text; 

    public void ShowText(string str)
    {
        _text.text = str;
        StartCoroutine(Fade());

    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
