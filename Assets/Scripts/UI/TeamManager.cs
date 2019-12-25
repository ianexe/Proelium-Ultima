using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TeamManager : MonoBehaviour
{
    public Image controller_image;
    public Image keyboard_image;

    public Text blue_ready;
    public Text red_ready;

    public float image_offset;

    private PlayerInputManager input_manager;
    private List<UnityEngine.InputSystem.PlayerInput> active_players;
    private List<Image> active_players_ui;

    public static TeamManager Instance;

    private UnityEngine.InputSystem.PlayerInput blue_player;
    private UnityEngine.InputSystem.PlayerInput red_player;

    // Start is called before the first frame update
    void Start()
    {
        input_manager = GetComponent<PlayerInputManager>();
        active_players = new List<UnityEngine.InputSystem.PlayerInput>();
        active_players_ui = new List<Image>();
        blue_player = null;
        red_player = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (ArePlayersReady())
        {
            GlobalData.Instance.blue_device = blue_player.devices[0];
            GlobalData.Instance.red_device = red_player.devices[0];
            SceneManager.LoadScene("SampleScene");
        }
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

    public bool JoinTeam(UnityEngine.InputSystem.PlayerInput player, PanelTeam team)
    {
        Debug.Log("Joining");
        bool ret = false;
        for (int i = 0; i < active_players.Count; i++)
        {
            if (active_players[i] == player)
            {
                Vector3 base_pos = active_players_ui[i].rectTransform.position;

                float x_value = active_players_ui[i].rectTransform.position.x;
                if (team == PanelTeam.BLUE && !IsPlayerInTeam(player))
                {
                    active_players_ui[i].rectTransform.position = new Vector3(470, 450, base_pos.z);
                    blue_player = player;
                    ret = true;
                }
                    
                else if (team == PanelTeam.RED && !IsPlayerInTeam(player))
                {
                    active_players_ui[i].rectTransform.position = new Vector3(1470, 450, base_pos.z);
                    red_player = player;
                    ret = true;
                }
            }
        }
        return ret;
    }

    private bool IsPlayerInTeam(UnityEngine.InputSystem.PlayerInput player)
    {
        return (red_player == player || blue_player == player);
    }

    private bool ArePlayersReady()
    {
        bool ret = false;

        if (red_player == null || blue_player == null)
            return false;

        ret = blue_player.GetComponent<UIPlayer>().ready && red_player.GetComponent<UIPlayer>().ready;

        return ret;
    }

    public UnityEngine.InputSystem.PlayerInput GetBluePlayer()
    {
        return blue_player;
    }

    public UnityEngine.InputSystem.PlayerInput GetRedPlayer()
    {
        return red_player;
    }
}
