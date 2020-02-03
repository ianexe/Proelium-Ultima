using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool solid;
    public bool destroyable;
    public int hp;
    public int lifetime;
    private List<Panel> panels = new List<Panel>();
    private PanelTeam team = PanelTeam.NULL;

    private SpriteRenderer sprite_renderer;
    private float spawn_time;
    public float blink_alpha;
    public int blink_frames;
    private bool blinking = false;

    // Start is called before the first frame update
    void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        spawn_time = Time.time;

        if (lifetime > 0)
            StartCoroutine(RemoveEntityByTime());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (!blinking && lifetime > 0 && (Time.time-spawn_time >= (FrameToTime(lifetime)/5)*4))
        {
            blinking = true;
            StartCoroutine(BlinkSprite());
        }

        if (destroyable && hp <= 0)
        {
            RemoveEntity();
        }   
    }

    public void SetPanel(Panel to_set)
    {
        panels.Add(to_set);
    }

    public void SetTeam(PanelTeam to_set)
    {
        if (team != PanelTeam.NULL)
            return;

        team = to_set;
    }

    private IEnumerator BlinkSprite()
    {
        var color = sprite_renderer.color;
        color.a = blink_alpha;
        sprite_renderer.color = color;
        while (true)
        {
            sprite_renderer.enabled = !sprite_renderer.enabled;
            yield return new WaitForSeconds(FrameToTime(blink_frames));
        }
        /*
        color.a = 1f;
        sprite_renderer.color = color;
        sprite_renderer.enabled = true;
        Debug.Log("Ending Blink at " + Time.frameCount);
        */
    }

    private IEnumerator RemoveEntityByTime()
    {
        yield return new WaitForSeconds(FrameToTime(lifetime));
        RemoveEntity();
    }

    public void RemoveEntity()
    {
        foreach (Panel panel in panels)
            panel.RemoveEntity(this);

        Destroy(gameObject);
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

