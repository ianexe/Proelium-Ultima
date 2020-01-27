using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    private Player player;
    public ParticleSystem particle;
    //public BoolArray test = new BoolArray(4, 3);
    public Attack attack1;
    public Attack attack2;
    public Attack attack3;
    public Attack attack4;
    public Attack current_attack = null;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    public void Attack(int value)
    {
        if (value == 1)
            current_attack = attack1;
        else if (value == 2)
            current_attack = attack2;
        else if (value == 3)
            current_attack = attack3;
        else if (value == 4)
            current_attack = attack4;
        else
        {
            Debug.Log("Invalid Attack");
            EndAttack();
            return;
        }

        if (current_attack.mana > player.current_mana)
        {
            Debug.Log("Not Enough Mana");
            EndAttack();
            return;
        }

        player.current_mana -= current_attack.mana;
        player.animator.SetInteger("Attack", current_attack.animation);
    }

    public void DoAttack()
    {
        Vector2 target = player.pos_id;

        for (int i = 0; i < current_attack.range.column.Length; i++)
        {
            for (int j = 0; j < current_attack.range.column[i].row.Length; j++)
            {
                if (current_attack.range.column[i].row[j])
                {
                    if (player.team == PanelTeam.BLUE)
                        target += new Vector2(j, i - 1);
                    else
                        target -= new Vector2(j, i - 1);

                    bool exists = (target.x < 0 || target.y < 0 || target.x >= player.panel_manager.x_size * 2 || target.y >= player.panel_manager.y_size);

                    //If Panel Exists Do Attack
                    if (!exists)
                    {
                        if (current_attack.range_type == Range.SINGLE)
                            SendAttack(current_attack, target);

                        if(current_attack.range_type == Range.ROW)
                        {
                            int scale = 1;
                            if (player.team == PanelTeam.RED)
                                scale = -1;

                            StartCoroutine(RowAttack(target, scale));
                                
                        }

                        if (current_attack.range_type == Range.COLUMN)
                        {
                            StartCoroutine(ColumnAttack(target));
                        }
                    }

                }
                target = player.pos_id;
            }
        }
    }

    IEnumerator RowAttack(Vector2 target, int scale)
    {
        Attack send_attack = current_attack;
        bool end_attack = false;
        for (int t = (int)target.x; t <= player.panel_manager.panel_list.GetUpperBound(0) && t >= 0; t += scale)
        {
            target.x = t;

            if (send_attack.end_when_hit && player.panel_manager.IsEnemyInPanel(target, player.team, true))
                end_attack = true;

            SendAttack(send_attack, target);

            if (end_attack)
                yield break;

            float timeStart = Time.time;
            int frameStart = Time.frameCount;

            if (send_attack.frame_delay > 0)
                yield return new WaitForSeconds(player.FrameToTime(send_attack.frame_delay));
            //Debug.Log(string.Format("Row Attack Time = {0}", Time.time - timeStart));
            //Debug.Log(string.Format("Row Attack Frame = {0}", Time.frameCount - frameStart));
            //Debug.Log(string.Format("Row Attack FrameRate = {0}", 1f / Time.deltaTime));
            //Debug.Log(string.Format("Row Attack Framerate = {0}", Time.frameCount - frameStart));
        }
    }

    IEnumerator ColumnAttack(Vector2 target)
    {
        Attack send_attack = current_attack;
        bool end_attack = false;

        if (send_attack.end_when_hit && (player.panel_manager.IsEnemyInPanel(target, player.team, true)))
            end_attack = true;

        SendAttack(send_attack, target);
        Vector2 target_up = target;
        Vector2 target_down = target;

        if (end_attack)
            yield break;

        if (send_attack.frame_delay > 0)
            yield return new WaitForSeconds(player.FrameToTime(send_attack.frame_delay));

        for (int t = 0; t < player.panel_manager.panel_list.GetUpperBound(1); t++)
        {
            target_up.y++;
            target_down.y--;

            if (send_attack.end_when_hit && (player.panel_manager.IsEnemyInPanel(target_up, player.team, true) || player.panel_manager.IsEnemyInPanel(target_down, player.team, true)))
                end_attack = true;

            if (player.panel_manager.PanelExists((int)target_up.x, (int)target_up.y))
                SendAttack(send_attack, target_up);
            if (player.panel_manager.PanelExists((int)target_down.x, (int)target_down.y))
                SendAttack(send_attack, target_down);

            if (end_attack)
                yield break;

            if (send_attack.frame_delay > 0)
                yield return new WaitForSeconds(player.FrameToTime(send_attack.frame_delay));
        }
    }

    void SendAttack(Attack attack, Vector2 target)
    {
        //Do Attack

        //Debug Particle
        //---------------
        if (attack.attack_type == AttackType.ATTACK)
        {
            GameObject instance = Instantiate(particle, player.panel_manager.panel_list[(int)target.x, (int)target.y].transform.position, Quaternion.identity).gameObject;
            Destroy(instance, 3);
        }
        //---------------

        Debug.Log("Attack " + player.panel_manager.IsEnemyInPanel(target, player.team));
        /*
        if (player.panel_manager.IsEnemyInPanel(target, player.team))
            player.panel_manager.DoDamageToEnemy(target, attack.damage, attack.secondary_effect, (int)attack.damage_animation);
            */

        Attack to_send = Instantiate(attack);
        to_send.SetTeam(player.team);

        if (player.panel_manager.PanelExists((int)target.x, (int)target.y))
            player.panel_manager.SendAttackToPanel(target, to_send);
    }

    public void EndAttack()
    {
        current_attack = null;
        player.animator.SetInteger("Attack", 0);
        player.ReceiveInput(PlayerInput.STOP_MOVE);
    }

    
}
