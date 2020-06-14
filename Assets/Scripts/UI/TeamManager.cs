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
    public RectTransform idle_pos;
    public RectTransform idle_pos2;

    public AudioSource player_enter_sfx;
    public AudioSource player_move_sfx;
    public AudioSource team_enter_sfx;
    public AudioSource champ_move_sfx;
    public AudioSource ready_sfx;
    public AudioSource cancel_sfx;

    public GameObject blue_ready;
    public GameObject red_ready;
    public GameObject blue_arrows;
    public GameObject red_arrows;

    public GameObject blue_character_ui;
    public GameObject red_character_ui;

    public RectTransform press_button;

    private PlayerInputManager input_manager;
    public List<UnityEngine.InputSystem.PlayerInput> active_players;
    private List<Image> active_players_ui;

    public static TeamManager Instance;

    public UnityEngine.InputSystem.PlayerInput blue_player;
    public UnityEngine.InputSystem.PlayerInput red_player;

    private Character blue_character = Character.NULL;
    private Character red_character = Character.NULL;

    public List<Image> char_sprites;

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
        if (active_players.Count > 0)
        {
            //press_button.gameObject.SetActive(false);
        }

        if (ArePlayersReady())
        {
            GlobalData.Instance.blue_device = blue_player.devices[0];
            GlobalData.Instance.red_device = red_player.devices[0];
            GlobalData.Instance.blue_character = blue_character;
            GlobalData.Instance.red_character = red_character;
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

        float y_value = idle_pos.position.y;
        y_value -= (input_manager.playerCount - 1) * GetControllerOffset();

        instance.rectTransform.position = new Vector3(idle_pos.position.x, y_value, idle_pos.position.z);
        active_players_ui.Add(instance);

        Vector3 press_position = press_button.position;
        press_position.y -= GetControllerOffset();
        press_button.position = press_position;

        player_enter_sfx.Play();
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
                if (team == PanelTeam.BLUE && !IsPlayerInTeam(team))
                {
                    active_players_ui[i].rectTransform.position = blue_ready.GetComponentInParent<RectTransform>().position;
                    blue_player = player;
                    ret = true;

                    player_move_sfx.Play();
                }
                    
                else if (team == PanelTeam.RED && !IsPlayerInTeam(team))
                {
                    active_players_ui[i].rectTransform.position = red_ready.GetComponentInParent<RectTransform>().position;
                    red_player = player;
                    ret = true;

                    player_move_sfx.Play();
                }
            }
        }
        return ret;
    }

    public void LeaveTeam(UnityEngine.InputSystem.PlayerInput player)
    {
        for (int i = 0; i < active_players.Count; i++)
        {
            if (active_players[i] == player)
            {
                float y_value = idle_pos.position.y;
                y_value -= i * GetControllerOffset();

                active_players_ui[i].rectTransform.position = new Vector3(idle_pos.position.x, y_value, 0);

                if (active_players[i] == blue_player)
                    blue_player = null;

                else if (active_players[i] == red_player)
                    red_player = null;

                player_move_sfx.Play();
            }
        }
    }

    private bool IsPlayerInTeam(PanelTeam team)
    {
        bool ret = false;

        if (team == PanelTeam.BLUE)
            ret = (blue_player != null);

        if (team == PanelTeam.RED)
            ret = (red_player != null);

        return ret;
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

    float GetControllerOffset()
    {
        return idle_pos.position.y - idle_pos2.position.y;
    }

    public void SwitchReadyText(PanelTeam team, bool ready=true)
    {
        if (team == PanelTeam.BLUE)
        {
            blue_ready.SetActive(!blue_ready.activeInHierarchy);
            blue_arrows.SetActive(!blue_arrows.activeInHierarchy);
        }
        else if (team == PanelTeam.RED)
        {
            red_ready.SetActive(!red_ready.activeInHierarchy);
            red_arrows.SetActive(!red_arrows.activeInHierarchy);
        }

        if (ready)
            ready_sfx.Play();
        else
            cancel_sfx.Play();
    }

    public void SwitchCharacterSelect(PanelTeam team, bool cancel = false)
    {
        for (int i = 0; i < active_players.Count; i++)
        {
            if (active_players[i].GetComponent<UIPlayer>().team == team)
                active_players_ui[i].gameObject.SetActive(!active_players_ui[i].gameObject.activeInHierarchy);
        }

        if (team == PanelTeam.BLUE)
            blue_character_ui.SetActive(!blue_character_ui.activeInHierarchy);

        else if (team == PanelTeam.RED)
            red_character_ui.SetActive(!red_character_ui.activeInHierarchy);

        if (cancel)
            cancel_sfx.Play();
    }

    public void ChooseCharacter(PanelTeam team, Character character, bool first_select = false)
    {
        if (team == PanelTeam.BLUE)
        {
            blue_character = character;
            blue_character_ui.GetComponentInChildren<Image>().sprite = char_sprites[(int)character].sprite;
        }
            
        else if (team == PanelTeam.RED)
        {
            red_character = character;
            red_character_ui.GetComponentInChildren<Image>().sprite = char_sprites[(int)character].sprite;
        }

        if (first_select)
            team_enter_sfx.Play();
        else
            champ_move_sfx.Play();
    }
}
