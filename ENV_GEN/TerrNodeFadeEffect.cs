using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TerrNodeFadeEffect : MonoBehaviour
{
    [SerializeField]
    private SO_TerrNodeFadeSettings settings = null;

    [SerializeField]
    private bool fadeOnlyIntoDistance = false;

    private SpriteRenderer spriteRenderer = null;

    private Color ogColor;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ogColor = spriteRenderer.color;

        SetTransparentInvisible();
    }

    private void Update()
    {
        SetFadeEffect();
        /*
        if (passedFadePoint) { return; }


        if (transform.position.z < fadeInRefPoint.Value.z)
        {
            passedFadePoint = true;
            //spriteRenderer.color = new Color(1, 1, 1, 1);
            return;
        }
        float distance = Vector3.Distance(transform.position, fadeInRefPoint.Value);
        float a = (initDistanceToFadePoint - distance) / initDistanceToFadePoint;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.r, spriteRenderer.color.r, a);*/
    }



    public void SetFadeEffect()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);
        float aComp = 1;
        float yComp;

        if (pos.y > settings.CachedFadeZPos)
        {
            SetInvisible();
            return;
        }
        else if (pos.y > settings.CachedVisibleZPos)
        {
            float totalFadeDist = settings.CachedFadeZPos - settings.CachedVisibleZPos;
            float dist = settings.CachedFadeZPos - pos.y;
            yComp = dist / totalFadeDist;
        }
        else if (pos.y > settings.CachedPlayerZPos)
        {
            yComp = 1;
        }
        else if (fadeOnlyIntoDistance) { return; }
        else if (pos.y > settings.CachedFadeZPosBehindPlayer)
        {
            float totalFadeDist = settings.CachedPlayerZPos - settings.CachedFadeZPosBehindPlayer;
            float dist = pos.y - settings.CachedFadeZPosBehindPlayer;
            aComp = dist / totalFadeDist;
            yComp = 1;
        }
        else
        {
            SetTransparentInvisible();
            return;
        }


        float xComp;

        if (pos.x < settings.CachedFadeXLeftPos)
        {
            SetInvisible();
            return;
        }
        else if (pos.x < settings.CachedVisibleXLeftPos)
        {
            float totalFadeDist = settings.CachedVisibleXLeftPos - settings.CachedFadeXLeftPos;
            float dist = pos.x - settings.CachedFadeXLeftPos;
            xComp = dist / totalFadeDist;
        }
        else if (pos.x < settings.CachedVisibleXRightPos)
        {
            xComp = 1;
        }
        else if (pos.x < settings.CachedFadeXRightPos)
        {
            float totalFadeDist = settings.CachedFadeXRightPos - settings.CachedVisibleXRightPos;
            float dist = settings.CachedFadeXRightPos - pos.x;
            xComp = dist / totalFadeDist;
        }
        else
        {
            SetInvisible();
            return;
        }


        float val = yComp * xComp;

        spriteRenderer.color = new Color(val * ogColor.r, val * ogColor.g, val * ogColor.b, aComp);
    }

    private void SetInvisible()
    {
        spriteRenderer.color = new Color(0, 0, 0, 1);
    }

    private void SetTransparentInvisible()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }
}
