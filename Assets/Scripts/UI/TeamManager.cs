using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TeamManager : MonoBehaviour
{
    public Image controller_image;
    public Image keyboard_image;
    public float image_offset;

    private PlayerInputManager input_manager;
    private List<UnityEngine.InputSystem.PlayerInput> active_players;
    private List<Image> active_players_ui;

    private UnityEngine.InputSystem.PlayerInput blue_player = null;
    private UnityEngine.InputSystem.PlayerInput red_player = null;

    // Start is called before the first frame update
    void Start()
    {
        input_manager = GetComponent<PlayerInputManager>();
        active_players = new List<UnityEngine.InputSystem.PlayerInput>();
        active_players_ui = new List<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerJoined()
    {
        UnityEngine.InputSystem.PlayerInput[] inputs_in_scene = FindObjectsOfType<UnityEngine.InputSystem.PlayerInput>();

        inputs_in_scene[0].transform.parent = transform;
        active_players.Add(inputs_in_scene[0]);

        Image instance = null;

        if (inputs_in_scene[0].currentControlScheme == "GamePad")
            instance = Instantiate(controller_image, transform);

        else if (inputs_in_scene[0].currentControlScheme == "KeyBoard")
            instance = Instantiate(keyboard_image, transform);

        Vector3 base_pos = instance.rectTransform.position;
        float y_value = instance.rectTransform.position.y;
        y_value -= (input_manager.playerCount-1) * image_offset;

        instance.rectTransform.position = new Vector3(base_pos.x, y_value, base_pos.z);
        active_players_ui.Add(instance);
    }

    public void OnPlayerLeft()
    {

    }

    public void JoinTeam(UnityEngine.InputSystem.PlayerInput player, PanelTeam team)
    {
        Debug.Log("Joining");
        for (int i = 0; i < active_players.Count; i++)
        {
            if (active_players[i] == player)
            {
                Vector3 base_pos = active_players_ui[i].rectTransform.position;

                float x_value = active_players_ui[i].rectTransform.position.x;
                if (team == PanelTeam.BLUE && blue_player == null)
                {
                    active_players_ui[i].rectTransform.position = new Vector3(450, 400, base_pos.z);
                    blue_player = player;
                }
                    
                else if (team == PanelTeam.RED && red_player == null)
                {
                    active_players_ui[i].rectTransform.position = new Vector3(1450, 400, base_pos.z);
                    red_player = player;
                }
            }
        }
    }
}
