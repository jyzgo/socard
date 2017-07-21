using UnityEngine;
using System.Collections;

public class ThemeSelector : CardBgSelector {

    public override void Select()
    {
        ThemeMgr.current.UpdateThemeSelectGlow(_index);
    }
}
