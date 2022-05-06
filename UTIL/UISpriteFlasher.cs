using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UISpriteFlasher : ASpriteFlasher
{
    [SerializeField]
    private Image uiSpriteRef = null;

    private Image flashImg;

    protected override void CacheSpriteComponentRef()
    {
        flashImg = GetComponent<Image>();
    }

    protected override Color GetFlashColor()
    {
        return flashImg.color;
    }

    protected override void SetFlashColor(Color clr)
    {
        flashImg.color = clr;
    }

    protected override void UpdateFlashSprite()
    {
        flashImg.sprite = uiSpriteRef.sprite;
    }
}
