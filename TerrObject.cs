using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class TerrObject : RunnerGameObject
{
    public static event System.Action<TerrObject> terrObjDestroyed = delegate { };

    public enum OBJ_TYPE { STATIC, STATIC_HAZ, ROCK, TEMP_GUN, ENEMY, THROWABLE };

    public const float ATTACHMENT_SPACING = 0.2f;
    //public enum ACTION { JUMP, ROLL, SPRINT}

    public float alienAttackTime = 0f;
    public GameObject alienEye;
    public const float ALIEN_ATTACK_THRESHOLD = 0.2f;
    public bool played = false;

    public OBJ_TYPE objType = OBJ_TYPE.STATIC;

    public PLAYER_STATE actionNeeded = PLAYER_STATE.NONE;

    public int hitBoxUnitWidth = 1;

    public bool canFlip = true;

    public float appearanceLikelihood = 0.2f;

    public float elevationOffsetPerc = 0f;

    public bool centerXPosWithHitBox = false;

    public int laneOffset = 0;

    public float minHeightUnitsForNextHaz = 0f;

    public bool canHaveAttachments = false;

    public List<TerrObject> AttachmentObjects { get; private set; } = new List<TerrObject>();
    public TerrObject parentAttachmentObject;

    public bool IsFlipped { get; private set; } = false;

    public void OnEnable()
    {
        if (alienEye != null) { alienEye.SetActive(false); }

        if (objType == OBJ_TYPE.ENEMY)
        {
            //stop animation
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

        PlayAnimation();
    }

    public void PlayAnimation()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Animator>().enabled = true;
        played = true;
        if (parentAttachmentObject == null || parentAttachmentObject.alienEye == null) { return; }
       parentAttachmentObject.alienEye.SetActive(false);
    }

    public void RandomizeSpriteType()
    {
        int index = Random.Range(0, animClips.Length);
        animOC[animIndex] = animClips[index];
    }

    public Vector3 getOffsetFromHitBox()
    {
        return gameObject.GetComponent<BoxCollider>().center * gameObject.transform.localScale.x;
    }

    private void OnDestroy()
    {
        foreach(TerrObject attachment in AttachmentObjects)
        {
            Destroy(attachment);
        }

        terrObjDestroyed(this);
    }

    public void flipTerrObj(int flipMultiplyer)
    {
        //I thought this should be the other way around but it's working...?
        IsFlipped = flipMultiplyer > 0 ? true : false;
        GetComponent<SpriteRenderer>().flipX = flipMultiplyer > 0 ? false : true;
    }

    public void AddAlienAttachment(TerrObject alien)
    {
        AttachmentObjects.Add(alien);
        alien.parentAttachmentObject = this;
        if (alienEye == null) { return; }
        if (alien.IsFlipped)
        {
            Vector3 eyePos = alienEye.transform.localPosition;
            alienEye.transform.localPosition = new Vector3(eyePos.x * -1, eyePos.y, eyePos.z);
        }
        
        alienEye.SetActive(true);
    }
}
