using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMove : MonoBehaviour
{
    private Player player;
    public GameObject test;

    public float min_swipe = 10;

    private Vector2 touch0_start;
    private Vector2 touch1_start;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch_0 = Input.GetTouch(0);
            if (touch_0.phase == TouchPhase.Began)
            {
                touch0_start = touch_0.position;
            }

            if (touch_0.phase == TouchPhase.Ended)
            {
                MoveByTouch(touch_0);
            }

            if (Input.touchCount > 1)
            {
                Touch touch_1 = Input.GetTouch(1);
                if (touch_1.phase == TouchPhase.Began)
                {
                    touch1_start = touch_1.position;
                }

                if (touch_1.phase == TouchPhase.Ended)
                {
                    MoveByTouch(touch_1);
                }
            }
        }
    }

    void MoveByTouch(Touch touch)
    {
        Vector2 touch_start;
        touch_start = Vector2.zero;
        if (touch.fingerId == 0)
            touch_start = touch0_start;
        else if (touch.fingerId == 1)
            touch_start = touch1_start;

        Vector2 delta = touch.position - touch_start;

        Vector2 delta_nosign = delta;
        if (delta_nosign.x < 0)
            delta_nosign.x *= -1;

        if (delta_nosign.y < 0)
            delta_nosign.y *= -1;

        Vector2 delta_normalized = delta.normalized;

        if (delta_normalized.x >= 0.75 && delta_nosign.x >= min_swipe)
            player.Move(Vector2.right);
        else if (delta_normalized.x <= -0.75 && delta_nosign.x >= min_swipe)
            player.Move(Vector2.left);
        if (delta_normalized.y >= 0.75 && delta_nosign.y >= min_swipe)
            player.Move(Vector2.up);
        else if (delta_normalized.y <= -0.75 && delta_nosign.y >= min_swipe)
            player.Move(Vector2.down);

        if (touch.deltaPosition == Vector2.zero)
        {
            int random = Random.Range(1,4);
            player.Attack(random);
        }
        //test.SetActive(true);
        //gameObject.SetActive(false);

        if (touch.fingerId == 0)
            touch0_start = Vector2.zero;
        else if (touch.fingerId == 1)
            touch1_start = Vector2.zero;
    }
}
