using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

[Serializable]
public class MenuButtonGroup<T>
{
    [SerializeField]
    private List<MenuButtonWrapper<T>> menuButtonWrappers = new List<MenuButtonWrapper<T>>();

    public MenuButton GetMenuButton(T enumVal)
    {
        MenuButtonWrapper<T> wrapper =  GetWrapperFromFunc(
            menuButtonWrappers,
            mbw => mbw.EnumVal,
            enumVal, LogEnum.WARNING,
            null);

        return wrapper.MenuButton;
    }

    public void ForEachButton(Action<MenuButton> method)
    {
        foreach(MenuButtonWrapper<T> mbw in menuButtonWrappers)
        {
            method(mbw.MenuButton);
        }
    }

    [Serializable]
    private class MenuButtonWrapper<T>
    {
        [SerializeField]
        private T enumVal = default;
        public T EnumVal => enumVal;

        [SerializeField]
        private MenuButton menuButton = null;
        public MenuButton MenuButton => menuButton;
    }
}


