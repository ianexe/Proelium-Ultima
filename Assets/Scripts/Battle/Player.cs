using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerState
{
    IDLE,
    MOVING,
    ATTACKING,
    STUNNED,
    DAMAGED,
    NULL
}

public enum PlayerInput
{
    MOVE,
    STOP_MOVE,
    ATTACK,
    STOP_DAMAGE,
    NULL
}

public class Player : MonoBehaviour
{
    [SerializeField]
    PlayerState state;

    public PanelTeam team;
    public Vector2 pos_id;
    public Vector2 real_pos_id; //Used in case it invades an enemy field

    public PanelManager panel_manager;

    private PlayerInput next_input;
    private Vector2 next_move;
    private int next_attack;

    public float buffer_frames;

    public GridMovement grid_movement;
    public AttackSystem attack_system;
    private InputManager input_manager;
    public SpriteRenderer sprite_renderer;
    public Animator animator;
    public SoundFXController sfx_controller;

    public int max_hp;
    public int current_hp;

    public float max_stamina;
    public float current_stamina;
    public float stamina_per_second;
    private bool regenerating_stamina;

    public float max_mana;
    public float current_mana;
    public float mana_per_second;
    private bool regenerating_mana;

    public bool untargeteable;
    public float recovery_frames;
    public float blink_alpha;
    public float blink_frames;

    // Start is called before the first frame update
    void Start()
    {
        grid_movement = GetComponent<GridMovement>();
        attack_system = GetComponent<AttackSystem>();
        input_manager = GetComponent<InputManager>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        sfx_controller = GetComponent<SoundFXController>();

        state = PlayerState.IDLE;

        next_input = PlayerInput.NULL;
        next_move = Vector2.zero;
        next_attack = 0;

        if (team == PanelTeam.RED)
            sprite_renderer.flipX = true;

        current_hp = max_hp;

        current_stamina = max_stamina;
        regenerating_stamina = false;

        current_mana = max_mana;
        regenerating_mana = false;

        untargeteable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (current_stamina < max_stamina && !regenerating_stamina)
            StartCoroutine(RegenerateStamina());

        if (current_mana < max_mana && !regenerating_mana)
            StartCoroutine(RegenerateMana());
    }

    //Check for Buffered Inputs when entering Idle
    void SetIdle()
    {
        state = PlayerState.IDLE;
        PlayerInput to_send = next_input;
        next_input = PlayerInput.NULL;
        if (to_send != PlayerInput.NULL)
        {
            ReceiveInput(to_send);
        }
        next_move = Vector2.zero;
    }

    public void ReceiveInput(PlayerInput received)
    {
        switch (state)
        {
            case PlayerState.IDLE:
                if (received == PlayerInput.MOVE && current_stamina >= 1f)
                {
                    state = PlayerState.MOVING;
                    grid_movement.Move(next_move);
                }

                if (received == PlayerInput.ATTACK)
                {
                    state = PlayerState.ATTACKING;
                    attack_system.Attack(next_attack);
                }
                break;

            case PlayerState.MOVING:
                if (received == PlayerInput.STOP_MOVE)
                {
                    SetIdle();
                }

                //Store Next Move in Buffer
                if (received == PlayerInput.MOVE)
                {
                    BufferInput(received);
                }
                if (received == PlayerInput.ATTACK)
                {
                    BufferInput(received);
                }
                break;

            case PlayerState.ATTACKING:
                if (received == PlayerInput.STOP_MOVE)
                {
                    if (real_pos_id != pos_id)
                        grid_movement.Teleport(pos_id);

                    SetIdle();
                }
                //Store Next Move in Buffer
                if (received == PlayerInput.MOVE)
                {
                    BufferInput(received);
                }
                if (received == PlayerInput.ATTACK)
                {
                    BufferInput(received);
                }
                break;

            case PlayerState.STUNNED:
                break;

            case PlayerState.DAMAGED:
                if (received == PlayerInput.STOP_DAMAGE)
                {
                    StartCoroutine(Recover());
                    StartCoroutine(BlinkSprite());
                    SetIdle();
                }
                //Store Next Move in Buffer
                if (received == PlayerInput.MOVE)
                {
                    BufferInput(received);
                }
                if (received == PlayerInput.ATTACK)
                {
                    BufferInput(received);
                }
                break;
        }
    }

    public void Move(Vector2 value)
    {
        if (next_input != PlayerInput.NULL || panel_manager.IsGamePaused())
            return;

        Debug.Log(value);

        if (value.x > 0.5 || value.x < -0.5 || value.y > 0.5 || value.y < -0.5)
        {
            next_move = value;
            ReceiveInput(PlayerInput.MOVE);
        }
    }

    //Regenerate System
    //------------------------------------------------------------------------------------
    private IEnumerator RegenerateStamina()
    {
        //Debug.Log("Starting Recovery");
        regenerating_stamina = true;
        while (current_stamina < max_stamina)
        {
            float result_stamina = current_stamina;

            result_stamina += (stamina_per_second * Time.deltaTime);

            if (result_stamina > max_stamina)
                result_stamina = max_stamina;

            current_stamina = result_stamina;
            yield return null;
        }
        regenerating_stamina = false;
        //Debug.Log("Ending Recovery at " + Time.frameCount);
    }

    private IEnumerator RegenerateMana()
    {
        float time_start = Time.time;
        //Debug.Log("Starting Mana Regeneration");
        regenerating_mana = true;
        while (current_mana < max_mana)
        {
            float result_mana = current_mana;

            result_mana += (mana_per_second * Time.deltaTime);

            if (result_mana > max_mana)
                result_mana = max_mana;

            current_mana = result_mana;
            yield return null;
        }
        regenerating_mana = false;
        //Debug.Log("Ending Regeneration at " + (Time.time - time_start));
    }
    //------------------------------------------------------------------------------------

    //Damage System
    //------------------------------------------------------------------------------------
    public void ReceiveDamage(int animation)
    {
        if (animation > 0 && state != PlayerState.DAMAGED)
        {
            CancelAttack();
            state = PlayerState.DAMAGED;
            animator.SetTrigger("Damage");
            //StartCoroutine(Recover());
            //StartCoroutine(BlinkSprite());
        }
    }

    private IEnumerator Recover()
    {
        Debug.Log("Starting Recovery");
        untargeteable = true;
        yield return new WaitForSeconds(FrameToTime(recovery_frames));
        Debug.Log("Ending Recovery at " + Time.frameCount);
        untargeteable = false;
    }

    private IEnumerator BlinkSprite()
    {
        var color = sprite_renderer.color;
        color.a = blink_alpha;
        sprite_renderer.color = color;
        while (untargeteable)
        {
            sprite_renderer.enabled = !sprite_renderer.enabled;
            yield return new WaitForSeconds(FrameToTime(blink_frames));
        }
        color.a = 1f;
        sprite_renderer.color = color;
        sprite_renderer.enabled = true;
        Debug.Log("Ending Blink at " + Time.frameCount);
    }

    private void CancelAttack()
    {
        next_attack = 0;
        animator.SetInteger("Attack", 0);
        if (real_pos_id != pos_id)
            grid_movement.Teleport(pos_id);
    }
    //------------------------------------------------------------------------------------

    //Secondary Effects
    //------------------------------------------------------------------------------------
    public void DoSecondaryEffect(SecondaryEffect effect)
    {
        switch(effect)
        {
            case SecondaryEffect.MOVE:
            {
                    int team_scale = 1;
                    if (team == PanelTeam.BLUE)
                        team_scale = -1;
                    grid_movement.Move(Vector2.right * team_scale, false);
            }
            break;
        }
    }
    //------------------------------------------------------------------------------------

    //Input Receivers
    //------------------------------------------------------------------------------------
    public void MoveUp(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();

        if (value != 0)
            Move(Vector2.up);
    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();

        if (value != 0)
            Move(Vector2.down);
    }

    public void MoveRight(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();

        if (value != 0)
            Move(Vector2.right);
    }

    public void MoveLeft(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();

        if (value != 0)
            Move(Vector2.left);
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (next_input != PlayerInput.NULL || panel_manager.IsGamePaused())
            return;

        var value = context.ReadValue<float>();

        if (value != 0)
        {
            next_attack = (int)value;
            ReceiveInput(PlayerInput.ATTACK);
        }
            
    }

    public void Attack(int value)
    {
        if (next_input != PlayerInput.NULL || panel_manager.IsGamePaused())
            return;

        if (value != 0)
        {
            next_attack = (int)value;
            ReceiveInput(PlayerInput.ATTACK);
        }

    }
    //------------------------------------------------------------------------------------

    //Input Buffer System
    //------------------------------------------------------------------------------------
    void BufferInput(PlayerInput to_buffer)
    {
        if (next_input == PlayerInput.NULL)
        {
            Debug.Log("Buffering Input");
            next_input = to_buffer;
            StartCoroutine(CleanBuffer());
        }
    }

    private IEnumerator CleanBuffer()
    {
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / FrameToTime(buffer_frames);
            yield return null;
        }
        if (next_input != PlayerInput.NULL)
        {
            Debug.Log("Cleaning Buffer");
            next_input = PlayerInput.NULL;
        }
    }
    //------------------------------------------------------------------------------------

    public void Spawn(PanelTeam start_team, Vector2 start_pos)
    {
        team = start_team;
        pos_id = start_pos;
        real_pos_id = pos_id;
        transform.position = panel_manager.PositionWithOffset(start_pos);
    }

    public float FrameToTime(float frame)
    {
        //Debug.Log(frame + " Frames = " + frame / 60f + " seconds");
        return (frame / 60f);
    }

    public float TimeToFrame(float time)
    {
        //Debug.Log(frame + " Frames = " + frame / 60f + " seconds");
        return (time*60f);
    }

    //Pause (Debug)
    public void Pause(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();

        if (value == 0)
            return;

        panel_manager.PauseGame();
            
    }

    public void SwitchInput(bool active)
    {
        if (active)
            input_manager.SwitchToPlayer();

        else
            input_manager.SwitchToUI();
    }
}
