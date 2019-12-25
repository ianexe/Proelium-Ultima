using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIPlayer : MonoBehaviour
{
    PanelTeam team = PanelTeam.NULL;
    TeamManager team_manager;
    public bool ready = false;

    // Start is called before the first frame update
    void Start()
    {
        team_manager = transform.parent.GetComponent<TeamManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;

        var value = context.ReadValue<Vector2>();

        if (value.x <= -0.75)
        {
            Debug.Log("Join Blue");
            if (team_manager.JoinTeam(GetComponent<UnityEngine.InputSystem.PlayerInput>(), PanelTeam.BLUE))
                team = PanelTeam.BLUE;
        }

        else if (value.x >= 0.75)
        {
            Debug.Log("Join Red");
            if (team_manager.JoinTeam(GetComponent<UnityEngine.InputSystem.PlayerInput>(), PanelTeam.RED))
                team = PanelTeam.RED;
        }

    }

    public void SetReady(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;

        if (team != PanelTeam.NULL)
        {
            ready = true;
            if (team == PanelTeam.BLUE)
                team_manager.blue_ready.gameObject.SetActive(true);

            else if (team == PanelTeam.RED)
                team_manager.red_ready.gameObject.SetActive(true);
        }
    }
}
