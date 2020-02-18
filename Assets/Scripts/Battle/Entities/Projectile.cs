using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Entity
{
    public int y_size;
    private int x_size = 1;

    public int frames_per_tile;
    public Attack attack;
    private float x_distance;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator Move()
    {
        var currentPos = transform.position;
        var finalPos = currentPos;
        finalPos.x += x_distance;
        var t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime / FrameToTime(frames_per_tile);
            transform.position = Vector3.Lerp(currentPos, finalPos, t);
            yield return null;
        }

        StartCoroutine(Move());
    }

    public void SetAttack(Attack to_set)
    {
        if (to_set != null)
            attack = to_set;
    }

    public void SetXDistance(float to_set)
    {
        x_distance = to_set;
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Panel"))
        {
            Panel col_panel = col.GetComponent<Panel>();
            col_panel.AddProjectile(this);
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Panel"))
        {
            Panel col_panel = col.GetComponent<Panel>();
            col_panel.RemoveProjectile(this);
        }

        if (GetPanelsCount() <= 0)
            Destroy(gameObject, 1);
    }
}
