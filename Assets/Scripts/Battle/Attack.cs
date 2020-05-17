using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Range
{
    SINGLE,
    ROW,
    COLUMN,
    DIAGONAL
}

public enum AttackType
{
    ATTACK,
    PROJECTILE,
    ENTITY,
    NULL
}

public enum DamageAnimation
{
    NONE,
    DAMAGE,
    STAGGER
}

public enum SecondaryEffect
{
    NONE,
    MOVE,
    PARALYSIS,
    BURN
}

public enum AttackMoveType
{
    FIRST_ENTITY,
    ENEMY,
    NULL
}

[CreateAssetMenu]
public class Attack : ScriptableObject
{
    public BoolArray range = new BoolArray(5, 3);
    public AttackType attack_type;
    public GameObject entity;
    public Range range_type;
    public float frame_delay;
    public float duration_frames;
    public bool end_when_hit;
    public int mana;
    public int damage;
    public int animation;
    public DamageAnimation damage_animation;

    //Sound FX
    public int sfx_fail_global;
    public int sfx_hit_global;
    public int sfx_fail_jeane = -1;
    public int sfx_hit_jeane = -1;


    public List<SecondaryEffect> secondary_effects;
    //public Sprite image;

    public bool move;
    public bool move_teleport;
    public float frames_to_move;
    public bool fixed_move_time;
    public AttackMoveType move_type;

    private PanelTeam team;

    public void SetTeam(PanelTeam to_set)
    {
        team = to_set;
    }

    public PanelTeam GetTeam()
    {
        return team;
    }
}
