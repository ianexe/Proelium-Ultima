using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool solid;
    public bool destroyable;
    public int hp;
    public int lifetime;
    private Panel panel;
    private PanelTeam team = PanelTeam.NULL;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (destroyable && hp <= 0)
            panel.RemoveEntity(this);
    }

    public void SetPanel(Panel to_set)
    {
        if (panel != null)
            return;

        panel = to_set;
    }

    public void SetTeam(PanelTeam to_set)
    {
        if (team != PanelTeam.NULL)
            return;

        team = to_set;
    }
}
