using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIPlayer : MonoBehaviour
{
    public PanelTeam team = PanelTeam.NULL;
    public Character character = Character.JEANE;
    TeamManager team_manager;
    public bool selected = false;
    public bool ready = false;
    UnityEngine.InputSystem.PlayerInput player_input;

    // Start is called before the first frame update
    void Start()
    {
        character = Character.JEANE;
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

        else if (team != PanelTeam.NULL && !selected)
            LeaveTeam(value);

        else if (team != PanelTeam.NULL && selected && !ready)
            ChooseCharacter(value);
            

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

    private void ChooseCharacter(Vector2 value)
    {
        if (value.x <= -0.75)
        {
            Debug.Log("Previous Character");
            character -= 1;
            if ((int)character == -1)
                character = Character.NULL - 1;

            team_manager.ChooseCharacter(team, character);
        }

        else if (value.x >= 0.75)
        {
            Debug.Log("Next Character");
            character += 1;
            if (character == Character.NULL)
                character = 0;

            team_manager.ChooseCharacter(team, character);
        }
    }

    public void SetReady(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;

        if (team != PanelTeam.NULL && !selected)
        {
            selected = true;
            team_manager.SwitchCharacterSelect(team);
            team_manager.ChooseCharacter(team, character, true);
        }

        else if (team != PanelTeam.NULL && character != Character.NULL && !ready)
        {
            ready = true;
            team_manager.SwitchReadyText(team);
            team_manager.ChooseCharacter(team, character, true);
        }
    }

    public void CancelReady(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;

        if (selected && !ready)
        {
            selected = false;
            team_manager.SwitchCharacterSelect(team, true);
        }

        else if (ready)
        {
            ready = false;
            team_manager.SwitchReadyText(team, false);
        }
    }
}
