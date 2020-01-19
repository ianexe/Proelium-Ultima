using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelTeam
{
    RED,
    BLUE,
    NULL
}

public enum PanelState
{
    NORMAL,
    CRACKED,
    BROKEN,
    POISON,
    FIRE
}

public class Panel : MonoBehaviour
{
    public PanelTeam panel_team;
    public PanelState panel_state;

    public Vector2 position_id;

    public List<Attack> active_attacks;
    public List<Entity> active_entities;

    private Color start_color;
    private Color warning_color;
    private Color damage_color;

    public PanelManager panel_manager;

    // Use this for initialization
    void Start()
    {
        panel_state = PanelState.NORMAL;

        warning_color = Color.yellow;
        warning_color.a = 0.5f;

        damage_color = Color.green;
        damage_color.a = 0.5f;

        if (panel_team == PanelTeam.BLUE)
        {
            start_color = Color.blue;
        }


        if (panel_team == PanelTeam.RED)
        {
            start_color = Color.red;
        }
    }

    // Update is called once per frame
    void Update()
    {
        List<Attack> to_iterate = new List<Attack>(active_attacks);

        foreach(Attack attack in to_iterate)
        {
            if (panel_manager.IsEnemyInPanel(position_id, attack.GetTeam()))
            {
                panel_manager.DoDamageToEnemy(position_id, attack.damage, attack.secondary_effects, (int)attack.damage_animation);
                RemoveAttack(attack);
            } 
        }

        to_iterate.Clear();
    }

    public void SetPosition(int x, int y)
    {
        position_id.x = x;
        position_id.y = y;
    }

    public void SetTeam(PanelTeam team)
    {
        panel_team = team;

        //Debug
        if (team == PanelTeam.BLUE)
        {
            start_color = Color.blue;
        }
                

        if (team == PanelTeam.RED)
        {
            start_color = Color.red;
        }

        GetComponent<SpriteRenderer>().color = start_color;
    }

    public bool IsWalkable(PanelTeam team)
    {
        bool ret = false;

        if (panel_state != PanelState.BROKEN && team == panel_team)
            ret = true;

        foreach (Entity entity in active_entities)
        {
            if (entity.solid)
            {
                ret = false;
                break;
            }
        }

        return ret;
    }

    public void AddAttack(Attack attack)
    {
        active_attacks.Add(attack);
        ChangeColor(damage_color);
        if (!CheckAttack(attack))
            StartCoroutine(RemoveAttackByTime(attack));
    }

    public void RemoveAttack(Attack attack)
    {
        foreach (Attack attack_t in active_attacks)
        {
            if (attack_t.GetInstanceID() == attack.GetInstanceID())
            {
                active_attacks.Remove(attack);
                ChangeColor(start_color);
                return;
            }
        }  
    }

    public void AddEntity(Entity entity)
    {
        entity.GetComponent<SpriteRenderer>().sortingOrder = (int)-position_id.y;
        active_entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        foreach (Entity entity_t in active_entities)
        {
            if (entity_t.GetInstanceID() == entity.GetInstanceID())
            {
                active_entities.Remove(entity_t);
                return;
            }
        }
    }

    private bool CheckAttack(Attack attack)
    {
        bool ret = false;

        if (panel_manager.IsEnemyInPanel(position_id, attack.GetTeam()))
        {
            panel_manager.DoDamageToEnemy(position_id, attack.damage, attack.secondary_effects, (int)attack.damage_animation);
            RemoveAttack(attack);

            ret = true;
        }

        return ret;
    }

    IEnumerator RemoveAttackByTime(Attack attack)
    {
        yield return new WaitForSeconds(FrameToTime(attack.duration_frames));
        RemoveAttack(attack);
    }

    void ChangeColor(Color new_color)
    {
        GetComponent<SpriteRenderer>().color = new_color;
    }

    float FrameToTime(float frame)
    {
        //Debug.Log(frame + " Frames = " + frame / 60f + " seconds");
        return (frame / 60f);
    }

    float TimeToFrame(float time)
    {
        //Debug.Log(frame + " Frames = " + frame / 60f + " seconds");
        return (time * 60f);
    }
}
