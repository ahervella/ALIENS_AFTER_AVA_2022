public enum PlayerActionEnum
{
    DODGE_L = 0,
    DODGE_R = 1,
    ROLL = 2,
    JUMP = 3,
    LAND = 4,
    LONG_JUMP = 5,
    LJ_LAND = 6,
    SPRINT = 7,
    NONE = 8,
    RUN = 9,
    HURT_CENTER = 10,
    HURT_UPPER = 11,
    HURT_LOWER = 12,
    HURT_AIR = 13,
    //hack: these are specifically for the requirements of things that should
    //hurt the player if they just touch them
    TAKE_DAMAGE_GROUND = 14,
    //TAKE_DAMAGE_AIR = 15,
    //TAKE_DAMAGE = 16,
}