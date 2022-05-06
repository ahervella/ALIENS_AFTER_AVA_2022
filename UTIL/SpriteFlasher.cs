using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HelperUtil;

[RequireComponent (typeof(SpriteRenderer))]
public class SpriteFlasher : ASpriteFlasher
{
    [SerializeField]
    private SpriteRenderer spriteRef = null;

    private SpriteRenderer spriteRend;

    protected override void CacheSpriteComponentRef()
    {
        spriteRend = GetComponent<SpriteRenderer>();
    }

    protected override Color GetFlashColor()
    {
        return spriteRend.color;
    }

    protected override void SetFlashColor(Color clr)
    {
        spriteRend.color = clr;
    }

    protected override void UpdateFlashSprite()
    {
        spriteRend.sprite = spriteRef.sprite;
    }
}
