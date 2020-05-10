using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject panel_prefab;
    public GameObject player_prefab;

    public int x_size;
    public int y_size;

    public float x_distance;
    public float y_distance;

    public Panel[,] panel_list;
    public Player[] player_list;

    public Text left_text;
    public Text right_text;
    public Text left_text2;
    public Text right_text2;
    public Text left_text3;
    public Text right_text3;
    public Text fps_text;

    public GameObject pause_menu;
    public GameObject end_battle_hud;
    public Button resume_button;

    public float camera_offset_Y;
    public GameObject test_background;

    private bool game_paused;
    private bool game_finished;

    // Use this for initialization
    void Start()
    {
        panel_list = new Panel[x_size * 2, y_size];
        SetPanelGrid();

        player_list = new Player[2];
        SpawnPlayers();

        //Camera Center
        Vector3 cam_pos = Camera.main.transform.position;
        cam_pos.x = panel_list[x_size, (int)(y_size / 2)].transform.position.x;
        cam_pos.x -= x_distance / 2;
        cam_pos.y = panel_list[x_size, (int)(y_size / 2)].transform.position.y;
        cam_pos.y += camera_offset_Y;

        Camera.main.transform.position = cam_pos;


        game_paused = false;
        game_finished = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (game_paused)
            return;

        left_text.text = "HP: " + GetPlayerInTeam(PanelTeam.BLUE).current_hp.ToString();
        right_text.text = "HP: " + GetPlayerInTeam(PanelTeam.RED).current_hp.ToString();
        left_text2.text = "STA: " + GetPlayerInTeam(PanelTeam.BLUE).current_stamina.ToString();
        right_text2.text = "STA: " + GetPlayerInTeam(PanelTeam.RED).current_stamina.ToString();
        left_text3.text = GetPlayerInTeam(PanelTeam.BLUE).current_mana.ToString();
        right_text3.text = GetPlayerInTeam(PanelTeam.RED).current_mana.ToString();
        fps_text.text = "FPS: " + (int)(1f / Time.deltaTime);
    }

    void LateUpdate()
    {
        foreach (Player player in player_list)
        {
            if (player.current_hp <= 0)
            {
                end_battle_hud.SetActive(true);
                Text to_set = end_battle_hud.GetComponentInChildren<Text>();
                if (player.team == PanelTeam.BLUE)
                {
                    to_set.text = "RED WINS";
                    to_set.color = Color.red;
                }


                else if (player.team == PanelTeam.RED)
                {
                    to_set.text = "BLUE WINS";
                    to_set.color = Color.blue;
                }

                game_finished = true;
                break;
            }
        }
    }

    void SetPanelGrid()
    {
        for (int x = 0; x < x_size * 2; x++)
        {
            for (int y = 0; y < y_size; y++)
            {
                GameObject to_instantiate = panel_prefab;
                Panel panel_instance = to_instantiate.GetComponent<Panel>();

                panel_instance.SetPosition(x, y);
                panel_instance.panel_manager = this;

                if (x >= x_size)
                    panel_instance.SetTeam(PanelTeam.RED);
                else
                    panel_instance.SetTeam(PanelTeam.BLUE);

                GameObject clone = Instantiate(to_instantiate, new Vector3(x * x_distance, y * y_distance, 0), Quaternion.identity);

                panel_list[x, y] = clone.GetComponent<Panel>();
            }
        }
    }

    public bool PanelExists(int x, int y)
    {
        bool ret = true;

        if (x < 0 || y < 0 || x >= x_size * 2 || y >= y_size || panel_list[x, y] == null)
            ret = false;

        return ret;
    }

    public bool IsPanelWalkable(int x, int y, PanelTeam team)
    {
        bool ret = false;

        if (!PanelExists(x, y))
            return false;

        if (panel_list[x, y].IsWalkable(team))
            ret = true;

        return ret;
    }

    public bool IsEnemyInPanel(Vector2 position, PanelTeam team, bool check_untargeteable = false, bool check_entity = true)
    {
        if (!PanelExists((int)position.x, (int)position.y))
            return false;

        if (check_entity && IsEntityInPanel(position, team))
            return true;

        for (int i = 0; i < player_list.Length; i++)
        {
            if (player_list[i].pos_id == position && player_list[i].team != team)
            {
                if (check_untargeteable && player_list[i].untargeteable)
                    return false;

                else
                    return true;
            }
        }

        return false;
    }

    public bool IsEntityInPanel(Vector2 position, PanelTeam team)
    {
        foreach (Entity entity in panel_list[(int)position.x, (int)position.y].active_entities)
        {
            if (entity.solid)
                return true;
        }

        return false;
    }

    public void SendAttackToPanel(Vector2 target, Attack attack)
    {
        switch(attack.attack_type)
        {
            case AttackType.ATTACK:
                panel_list[(int)target.x, (int)target.y].AddAttack(attack);
                break;

            case AttackType.ENTITY:
                panel_list[(int)target.x, (int)target.y].AddEntity(attack);
                break;

            case AttackType.PROJECTILE:
                panel_list[(int)target.x, (int)target.y].SpawnProjectile(attack);
                break;
        }
    }

    public void DoDamageToEnemy(Vector2 position, int damage, List<SecondaryEffect> effects, int animation)
    {
        for (int i = 0; i < player_list.Length; i++)
        {
            if (player_list[i].pos_id == position && !player_list[i].untargeteable)
            {
                foreach (SecondaryEffect effect in effects)
                    player_list[i].DoSecondaryEffect(effect);

                player_list[i].ReceiveDamage(animation);
                int new_hp = player_list[i].current_hp;
                new_hp -= damage;

                if (new_hp < 0)
                    new_hp = 0;

                player_list[i].current_hp = new_hp;
            }
        }

        DoDamageToEntity(position, damage);
    }

    public void DoDamageToEntity(Vector2 position, int damage)
    {
        foreach (Entity entity in panel_list[(int)position.x, (int)position.y].active_entities)
        {
            if (entity.destroyable)
            {
                entity.hp -= damage;
            }
        }
    }

    Player GetPlayerInTeam(PanelTeam team)
    {
        Player ret = null;

        for (int i = 0; i < player_list.Length; i++)
        {
            if (player_list[i].team == team)
            {
                ret = player_list[i];
            }
        }

        return ret;
    }

    void SpawnPlayers()
    {
        int spawned_players = 0;

        for (int x = 0; x < x_size * 2; x++)
        {
            for (int y = 0; y < y_size; y++)
            {
                if (spawned_players == 0)
                {
                    GameObject to_instantiate = player_prefab;
                    Player player_instance = to_instantiate.GetComponent<Player>();

                    player_instance.panel_manager = this;
                    player_instance.Spawn(PanelTeam.BLUE, panel_list[x, y].position_id);

                    GameObject clone = Instantiate(to_instantiate, PositionWithOffset(panel_list[x, y].position_id), Quaternion.identity);
                    player_list[spawned_players] = clone.GetComponent<Player>();

                    if (GlobalData.Instance.blue_device != null)
                        player_list[spawned_players].GetComponent<UnityEngine.InputSystem.PlayerInput>().SwitchCurrentControlScheme(GlobalData.Instance.blue_device);

                    spawned_players++;
                }

                if (spawned_players == 1 && panel_list[x, y].panel_team == PanelTeam.RED)
                {
                    GameObject to_instantiate = player_prefab;
                    Player player_instance = to_instantiate.GetComponent<Player>();

                    player_instance.panel_manager = this;
                    player_instance.Spawn(PanelTeam.RED, panel_list[x, y].position_id);

                    GameObject clone = Instantiate(to_instantiate, PositionWithOffset(panel_list[x, y].position_id), Quaternion.identity);
                    player_list[spawned_players] = clone.GetComponent<Player>();
                    clone.GetComponent<TouchMove>().enabled = false;

                    if (GlobalData.Instance.red_device != null)
                        player_list[spawned_players].GetComponent<UnityEngine.InputSystem.PlayerInput>().SwitchCurrentControlScheme(GlobalData.Instance.red_device);

                    spawned_players++;
                }
            }
        }

    }

    public Vector2 PositionWithOffset(Vector2 position)
    {
        Vector2 ret = position;
        ret.x *= x_distance;
        ret.y *= y_distance;
        return ret;
    }

    public bool IsGamePaused()
    {
        return game_paused || game_finished;
    }

    public void PauseGame()
    {
        if (game_finished)
            return;

        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            game_paused = true;
            pause_menu.SetActive(true);

            foreach (Player player in player_list)
                player.SwitchInput(false);

            UnityEngine.EventSystems.EventSystem e = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            e.SetSelectedGameObject(resume_button.gameObject);
        }


        else
        {
            Time.timeScale = 1;
            game_paused = false;
            pause_menu.SetActive(false);

            foreach (Player player in player_list)
                player.SwitchInput(true);

            UnityEngine.EventSystems.EventSystem e = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            e.SetSelectedGameObject(null);
        }
    }

    public void ExitGame()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("UITest");
    }
}
