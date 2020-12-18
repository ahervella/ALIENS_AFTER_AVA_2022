using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class TerrObject : RunnerGameObject
{
    public static event System.Action<TerrObject> terrObjDestroyed = delegate { };

    //TODO MERGE THESE ENUMS AND THEIR USE IN OTHER SCRIPTS
    public enum OBJ_TYPE { STATIC, STATIC_HAZ, ROCK, TEMP_GUN, ENEMY, THROWABLE };

    public enum HAZ_TYPE { LOG, BUSH, TREE, ALIEN_SWIPE, ALIEN_SLASH, ALIEN_JUMP, NONE };

    public const float ATTACHMENT_SPACING = 0.2f;
    //public enum ACTION { JUMP, ROLL, SPRINT}

    public float alienAttackTime = 0f;
    public GameObject alienEye;
    public const float ALIEN_ATTACK_THRESHOLD = 0.2f;
    public bool played = false;

    public OBJ_TYPE objType = OBJ_TYPE.STATIC;

    public HAZ_TYPE hazType = HAZ_TYPE.NONE;

    public PLAYER_STATE actionNeeded = PLAYER_STATE.NONE;

    public int hitBoxUnitWidth = 1;

    public bool canFlip = true;

    [SerializeField]
    public float appearanceLikelihood = 0.2f;
    public float AppearanceLikelihood => appearanceLikelihood;

    public float elevationOffsetPerc = 0f;

    public bool centerXPosWithHitBox = false;

    public int laneOffset = 0;

    public float minHeightUnitsForNextHaz = 0f;

    public bool canHaveAttachments = false;

    public List<TerrObject> AttachmentObjects { get; private set; } = new List<TerrObject>();
    public TerrObject parentAttachmentObject;

    public bool IsFlipped { get; private set; } = false;


    public AAudioWrapper alienSound;

    [SerializeField]
    private List<LaneRule> laneRules = new List<LaneRule>();

    [Serializable]
    public class LaneRule
    {
        [SerializeField]
        public int offSetFromDirection = 0;
        [SerializeField]
        public int minDistForConflict = 0;
        [SerializeField]
        public List<OBJ_TYPE> appliesToTypes = new List<OBJ_TYPE>();

        public TerrObject owner { get; private set; }

        public void Initialize(TerrObject owner)
        {
            this.owner = owner;
        }

        public bool Allowed(int laneCount, int myCol, int otherCol, TerrObject otherObj, float spawnZPos)
        {
            //we start with type given most checks will be against grass
            if (!appliesToTypes.Contains(otherObj.objType))
            {
                return true;
            }

            myCol += owner.IsFlipped ? offSetFromDirection * -1 : offSetFromDirection;
            //assuming first column is 0
            myCol %= laneCount;

            if (myCol != otherCol)
            {
                return true;
            }

            return spawnZPos - owner.transform.position.z > minDistForConflict;
        }
    }

    public void OnEnable()
    {
        if (alienEye != null) { alienEye.SetActive(false); }

        foreach(LaneRule lr in laneRules)
        {
            lr.Initialize(this);
        }

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
        if (alienSound != null)
        {
            RunnerSounds.Current.PlayAudioWrapper(alienSound, gameObject);
        }
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Animator>().enabled = true;
        played = true;
        if (parentAttachmentObject == null || parentAttachmentObject.alienEye == null) { return; }
       parentAttachmentObject.alienEye.SetActive(false);
    }


    public void PlaceOnEnvironmentCoord(Vector3 coord)
    {
        transform.position = new Vector3(coord.x, HitBoxElevOffset() + coord.y, coord.z);
    }

    float HitBoxElevOffset()
    {
        return (hitBox.size.y / 2f * transform.localScale.y) * (1f - elevationOffsetPerc);
    }




    public void RandomizeSpriteType()
    {
        int index = UnityEngine.Random.Range(0, animClips.Length);
        animOC[animIndex] = animClips[index];
    }

    public Vector3 GetOffsetFromHitBox()
    {
        return gameObject.GetComponent<BoxCollider>().center * gameObject.transform.localScale.x;
    }

    //TODO: see if you can override this for situations where the bullet wants
    //to destroy which means we don't actually destroy it but make it dissapear and keep the go to still be able to play the sound
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
        
        IsFlipped = flipMultiplyer > 0 ? false : true;
        GetComponent<SpriteRenderer>().flipX = flipMultiplyer > 0 ? false : true;
        for (int i = 0; i < transform.childCount; i++)
        {
            var go = transform.GetChild(i);
            //TODO: does this save computation time? Can work either way
            if (go.gameObject == alienEye) { continue; }
            Vector3 pos = go.transform.position;
            go.transform.position = new Vector3(pos.x * flipMultiplyer, pos.y, pos.z);
        }
    }

    public void AddAlienAttachment(TerrObject alien)
    {
        AttachmentObjects.Add(alien);
        alien.parentAttachmentObject = this;
        if (alienEye == null) { return; }
        if (alien.IsFlipped)
        {
            Vector3 eyePos = alienEye.transform.localPosition;
            Vector3 eyeScale = alienEye.transform.localScale;

            alienEye.transform.localPosition = new Vector3(eyePos.x * -1, eyePos.y, eyePos.z);
            alienEye.transform.localScale = new Vector3(eyeScale.x * -1, eyePos.y, eyePos.z);
        }
        
        alienEye.SetActive(true);
    }

    

    public bool CanAddToLane(int laneCount, int myCol, TerrObject otherObj, int otherCol, float spawnZPos)
    {
        foreach(LaneRule lr in laneRules)
        {
            if (!lr.Allowed(laneCount, myCol, otherCol, otherObj, spawnZPos))
            {
                return false;
            }
        }
        return true;
    }


    public float GetDepthOffset(float heightUnit, float? parentDepthOffset = null)
    {
        return parentDepthOffset != null ? (float)parentDepthOffset + ATTACHMENT_SPACING : Random.Range(-(heightUnit) / 2f, heightUnit / 2f);
    }

    public float GetHorizontalOffset(float widthUnit, float flipMultiplyer)
    {
        //can only randomiize this offset if it's a STATIC type of object, like grass
        if (objType == OBJ_TYPE.STATIC)
        {
            return Random.Range(-(widthUnit) / 2f, widthUnit / 2f);
        }

        if (centerXPosWithHitBox)
        {
            return GetOffsetFromHitBox().x * flipMultiplyer;
        }

        return 0f;
    }

    public float GetElevationOffset()
    {
        return (hitBox.size.y / 2f * transform.localScale.y) * (1f - elevationOffsetPerc);
    }

    public void GetTheoreticalSpawnInfo(int colIndex, int laneCount, out int resultColIndex, out int outFlipMultiplyer, TerrObject parentTerrObj = null)
    {
        int flipMultiplyer = 1;
        if (canFlip)
        {
            if (parentTerrObj != null)
            {
                if (parentTerrObj.IsFlipped)
                {
                    flipMultiplyer = -1;
                }
            }
            else
            {
                flipMultiplyer = Random.Range(0, 2) * 2 - 1;
            }
        }

        outFlipMultiplyer = flipMultiplyer;

        resultColIndex = (colIndex + laneOffset * flipMultiplyer) % laneCount;

    }

    public TerrObject InitTerrObj(Vector3 finalSpawnPos, int flipMultiplyer, TerrObject parentTerrObj = null)
    {
        parentAttachmentObject = parentTerrObj;
        flipTerrObj(flipMultiplyer);
        TerrObject terrObjInst = Instantiate(this);
        terrObjInst.transform.position = finalSpawnPos;
        //TODO: check if its an alien type of terr object? don't assume all attachments are aliens?
        parentTerrObj.AddAlienAttachment(terrObjInst);
        return terrObjInst;
    }
}
