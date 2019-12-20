using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class InputManager : MonoBehaviour
{
    private Player player;
    private UnityEngine.InputSystem.PlayerInput player_input;
    private InputActionAsset action_asset;
    private InputActionMap ui_map;
    bool ui_enabled;
    private InputActionMap player_map;
    bool player_enabled;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        player_input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        action_asset = player_input.actions;
        ui_map = action_asset.FindActionMap("UI");
        ui_enabled = ui_map.enabled;
        player_map = action_asset.FindActionMap("Player");
        player_enabled = player_map.enabled;
    }

    void Update()
    {

    }

    void LateUpdate()
    {
        if (ui_enabled != ui_map.enabled)
        {
            if (ui_enabled)
                ui_map.Enable();
            else
                ui_map.Disable();
        }

        if (player_enabled != player_map.enabled)
        {
            if (player_enabled)
                player_map.Enable();
            else
                player_map.Disable();
        }
    }

    public void SwitchToUI(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();


        if (value != 0)
        {
            ui_enabled = true;
            player_enabled = false;
        }

    }

    public void SwitchToPlayer(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();

        if (value != 0)
        {
            ui_enabled = false;
            player_enabled = true;
        }
    }

    public void SwitchToUI()
    {
        ui_enabled = true;
        player_enabled = false;
    }

    public void SwitchToPlayer()
    {
        ui_enabled = false;
        player_enabled = true;
    }

    /*
    public void SetUIInput(InputSystemUIInputModule ui_module)
    {
        ui_module.actionsAsset = action_asset;
        ui_module.move.Set(ui_map.FindAction("Move"));
        ui_module.submit.Set(ui_map.FindAction("Cross"));
        ui_module.cancel.Set(ui_map.FindAction("Circle"));
    }
    */
}
