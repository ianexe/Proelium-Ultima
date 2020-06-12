using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIPlayer : MonoBehaviour
{
    PanelTeam team = PanelTeam.NULL;
    TeamManager team_manager;
    public bool ready = false;
    UnityEngine.InputSystem.PlayerInput player_input;

    // Start is called before the first frame update
    void Start()
    {
        player_input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
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

        if (team == PanelTeam.NULL)
            JoinTeam(value);

        else if (team != PanelTeam.NULL && !ready)
            LeaveTeam(value);

    }

    private void JoinTeam(Vector2 value)
    {
        if (player_input == null)
            return;

        if (value.x <= -0.75)
        {
            Debug.Log("Join Blue");
            if (team_manager.JoinTeam(player_input, PanelTeam.BLUE))
                team = PanelTeam.BLUE;
        }

        else if (value.x >= 0.75)
        {
            Debug.Log("Join Red");
            if (team_manager.JoinTeam(player_input, PanelTeam.RED))
                team = PanelTeam.RED;
        }
    }

    private void LeaveTeam(Vector2 value)
    {
        if (player_input == null)
            return;

        if (value.x <= -0.75 && team == PanelTeam.RED)
        {
            team_manager.LeaveTeam(player_input);
            team = PanelTeam.NULL;
        }

        else if (value.x >= 0.75 && team == PanelTeam.BLUE)
        {
            team_manager.LeaveTeam(player_input);
            team = PanelTeam.NULL;
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

    public void CancelReady(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;

        if (ready)
        {
            ready = false;
            if (team == PanelTeam.BLUE)
                team_manager.blue_ready.gameObject.SetActive(false);

            else if (team == PanelTeam.RED)
                team_manager.red_ready.gameObject.SetActive(false);
        }
    }
}
