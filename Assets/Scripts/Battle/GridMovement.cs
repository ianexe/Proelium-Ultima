using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridMovement : MonoBehaviour
{
    private Vector3 pos;
    private Transform tr;

    public float movement_frames = 1.0f;

    private Player player;

    void Start()
    {
        player = GetComponent<Player>();

        pos = transform.position;
        tr = transform;

    }

    public void Move(Vector2 dest, bool animation = true)
    {
        Vector3 direction = Vector3.zero;
        if (dest.x >= 0.5)
            direction = Vector3.right;
        else if (dest.x <= -0.5)
            direction = Vector3.left;
        else if (dest.y >= 0.5)
            direction = Vector3.up;
        else if (dest.y <= -0.5)
            direction = Vector3.down;

        if (player.panel_manager.IsPanelWalkable((int)player.pos_id.x + (int)direction.x, (int)player.pos_id.y + (int)direction.y, player.team))
        {
            //Debug Animation
            if (animation)
                player.animator.SetBool("Moving", true);

            player.current_stamina -= 1f;
            player.pos_id += (Vector2)direction;
            player.real_pos_id = player.pos_id;
            direction.x *= player.panel_manager.x_distance;
            direction.y *= player.panel_manager.y_distance;
            Vector3 end = pos += direction;
            StartCoroutine(MoveToPosition(tr, end, animation));
        }

        else
        {
            Debug.Log("Unable to walk");
            if (animation)
                player.ReceiveInput(PlayerInput.STOP_MOVE);
        }
        
    }

    private IEnumerator MoveToPosition(Transform transform, Vector3 position, bool animation)
    {
        var currentPos = transform.position;
        var t = 0f;
        bool layer_changed = false;
        while (t < 1)
        {
            if (t >= 0.5f && !layer_changed)
            {
                float y_movement = position.y - currentPos.y;
                int sorting_order = 0;
                if (y_movement != 0)
                {
                    if (y_movement < 0)
                        sorting_order = 1;
                    else
                        sorting_order = -1;
                }

                player.GetComponent<SpriteRenderer>().sortingOrder += sorting_order;
                layer_changed = true;
            }

            t += Time.deltaTime / player.FrameToTime(movement_frames);
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }

        if (animation)
        {
            player.animator.SetBool("Moving", false);
            player.ReceiveInput(PlayerInput.STOP_MOVE);
        }
    }

    public void Teleport (Vector2 dest, bool check_team = true)
    {
        PanelTeam team_to_check = PanelTeam.NULL;
        if (check_team)
            team_to_check = player.team;

        if (player.panel_manager.IsPanelWalkable((int)dest.x, (int)dest.y, team_to_check))
        {
            if (dest.y != player.real_pos_id.y)
            {
                float y_movement = dest.y - player.real_pos_id.y;
                player.GetComponent<SpriteRenderer>().sortingOrder -= (int)y_movement;
            }

            if (check_team)
            {
                player.pos_id = dest;
                player.real_pos_id = player.pos_id;
            }

            else
            {
                player.real_pos_id = dest;
            }

            Vector2 direction = dest;
            direction.x *= player.panel_manager.x_distance;
            direction.y *= player.panel_manager.y_distance;

            transform.position = direction;
        }

        else
        {
            Debug.Log("Unable to teleport");
        }
    }

    //Move Attack Logic
    public void MoveAttack(Vector2 dest, float move_frames)
    {
        if (player.panel_manager.IsPanelWalkable((int)dest.x, (int)dest.y, PanelTeam.NULL))
        {
            player.real_pos_id = dest;

            Vector2 direction = dest;
            direction.x *= player.panel_manager.x_distance;
            direction.y *= player.panel_manager.y_distance;
            player.animator.SetBool("MoveAttack", true);
            StartCoroutine(MoveToAttack(tr, direction, move_frames));
        }

        else
        {
            Debug.Log("Unable to walk");
            player.animator.SetBool("MoveAttack", false);
            player.ReceiveInput(PlayerInput.STOP_MOVE);
        }
    }

    private IEnumerator MoveToAttack(Transform transform, Vector3 position, float frames)
    {
        var currentPos = transform.position;
        var t = 0f;
        bool layer_changed = false;
        while (t < 1)
        {
            if (transform.position != Vector3.Lerp(currentPos, position, t))
                break;

            if (t >= 0.5f && !layer_changed)
            {
                float y_movement = position.y - currentPos.y;
                int sorting_order = 0;
                if (y_movement != 0)
                {
                    if (y_movement < 0)
                        sorting_order = 1;
                    else
                        sorting_order = -1;
                }

                player.GetComponent<SpriteRenderer>().sortingOrder += sorting_order;
                layer_changed = true;
            }

            t += Time.deltaTime / player.FrameToTime(frames);
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }

        if (t >= 1)
        {
            player.animator.SetBool("MoveAttack", false);
            player.animator.SetInteger("Attack", player.attack_system.current_attack.animation);
        }
    }
}
