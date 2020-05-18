// so this file just has all our custom types

public enum RumbleStrength
{
    WEAK,
    MEDIUM,
    INTENSE
}

public enum ShakeStrength
{
    WEAK,
    MEDIUM,
    INTENSE
}

public enum ShakeLength
{
    SHORT,
    MEDIUM,
    LONG
}

public enum RotationDirection {
    CLOCKWISE, 
    COUNTERCLOCKWISE
}

public enum ShoeType
{
    BAREFOOT,
    BOOTS,
    FLIPFLOPS
}

public enum AIInterest
{
    MAID,
    TEST_AI_1,
    TEST_AI_2,
    RECEPTIONIST,
    SUITCASE_PACKER,
    STOOL_BOI,
    BARTENDER,
    TSA,
    AIRPORT_CROWD,
    AIRPORT_WORKER,
    AIRPORT_RESTAURANT_WORKER,
    TSA_2,
    TSA_3,
    MAID_CRISIS,
    MAID_CRISIS_2,
    SUITCASE_PACKER_2,
    STOOL_BOI_2,
    OWNER
}

public enum ShoeSightType
{
    INTERACTABLE,
    ENEMY,
    OBJECTIVE,
    BLIND_ENEMY
}

public enum StepType
{
    MOVE,
    STAY,
    INTERACT,
    INVESTIGATE,
    RUSH,
    DIE
}

public enum Direction
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}

public enum UIDirection
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public enum StoplightColor
{
    GREEN,
    YELLOW,
    RED
}

public enum MainMenuState
{
    MAIN,
    PLAY,
    OPTIONS,
    JIBBZ
}

public enum SoundSettings
{
    MASTER_VOLUME,
    AMBIENCE_VOLUME,
    MUSIC_VOLUME,
    SFX_VOLUME
}

public enum GraphicsSetting
{
    RESOLUTION_INDEX,
    FULLSCREEN,
    QUALITY_INDEX
}

public enum DamageSource {
    TANK,
    HELICOPTER
}

public enum ChaseBehaviors
{
    CATCH,
    SUMMON,
    SEQUENCE
}

public enum AIState { IDLE, INVESTIGATE, INVESTIGATING, CHASE, INTERACT }