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
        while (t < 1)
        {
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
}
