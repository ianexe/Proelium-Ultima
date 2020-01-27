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

[CreateAssetMenu]
public class Attack : ScriptableObject
{
    public BoolArray range = new BoolArray(4, 3);
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
    public List<SecondaryEffect> secondary_effects;
    public Sprite image;

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
