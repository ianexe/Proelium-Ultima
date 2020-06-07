﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    PanelManager panel_manager;

    public Slider left_lifebar_hp;
    public Slider left_lifebar_damage;
    public Slider right_lifebar_hp;
    public Slider right_lifebar_damage;

    public Slider left_stamina_bar;
    public Slider right_stamina_bar;

    Slider[] left_stamina_bars;
    Slider[] right_stamina_bars;

    public float damage_slider_speed = 0.5f;
    /*
    public Text left_text;
    public Text right_text;
    public Text left_text2;
    public Text right_text2;
    public Text left_text3;
    public Text right_text3;
    public Text fps_text;
    */

    // Start is called before the first frame update
    void Start()
    {
        panel_manager = GetComponent<PanelManager>();

        RectTransform rt = left_stamina_bar.GetComponent<RectTransform>();
        float offset = -rt.localPosition.x - left_stamina_bar.GetComponent<RectTransform>().rect.width * rt.localScale.x;

        left_stamina_bars = new Slider[(int)panel_manager.GetPlayerInTeam(PanelTeam.BLUE).max_stamina];
        right_stamina_bars = new Slider[(int)panel_manager.GetPlayerInTeam(PanelTeam.RED).max_stamina];

        //Left Bar
        Vector2 sd = left_stamina_bar.GetComponent<RectTransform>().sizeDelta;
        sd.x /= panel_manager.GetPlayerInTeam(PanelTeam.BLUE).max_stamina;
        left_stamina_bar.GetComponent<RectTransform>().sizeDelta = sd;
        left_stamina_bars[0] = left_stamina_bar;

        for (int i = (int)panel_manager.GetPlayerInTeam(PanelTeam.BLUE).max_stamina - 1; i > 0; i--)
        {
            Slider instance = Instantiate(left_stamina_bar);
            instance.transform.SetParent(left_stamina_bar.transform.parent);
            instance.transform.position = left_stamina_bar.transform.position;
            instance.transform.localScale = left_stamina_bar.transform.localScale;

            RectTransform i_rt = instance.GetComponent<RectTransform>();
            Vector2 lp = i_rt.localPosition;
            lp.x = -(i_rt.rect.width * i_rt.localScale.x)*i - offset;
            instance.GetComponent<RectTransform>().localPosition = lp;

            left_stamina_bars[(int)panel_manager.GetPlayerInTeam(PanelTeam.BLUE).max_stamina - i] = instance;
        }

        //Right Bar
        Vector2 sd2 = right_stamina_bar.GetComponent<RectTransform>().sizeDelta;
        sd2.x /= panel_manager.GetPlayerInTeam(PanelTeam.RED).max_stamina;
        right_stamina_bar.GetComponent<RectTransform>().sizeDelta = sd2;
        right_stamina_bars[0] = right_stamina_bar;

        for (int i = 1; i < (int)panel_manager.GetPlayerInTeam(PanelTeam.RED).max_stamina; i++)
        {
            Slider instance = Instantiate(right_stamina_bar);
            instance.transform.SetParent(right_stamina_bar.transform.parent);
            instance.transform.position = right_stamina_bar.transform.position;
            instance.transform.localScale = right_stamina_bar.transform.localScale;

            RectTransform i_rt = instance.GetComponent<RectTransform>();
            Vector2 lp = i_rt.localPosition;
            lp.x = (i_rt.rect.width * i_rt.localScale.x) * i + offset;
            instance.GetComponent<RectTransform>().localPosition = lp;

            right_stamina_bars[(int)panel_manager.GetPlayerInTeam(PanelTeam.BLUE).max_stamina - i] = instance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        left_lifebar_hp.value = GetNormalizedHealth(PanelTeam.BLUE);
        if (left_lifebar_hp.value < left_lifebar_damage.value)
        {
            float to_set = left_lifebar_damage.value;

            to_set -= Time.deltaTime * damage_slider_speed;

            if (to_set < left_lifebar_hp.value)
                to_set = left_lifebar_hp.value;

            left_lifebar_damage.value = to_set;
        }

        right_lifebar_hp.value = GetNormalizedHealth(PanelTeam.RED);
        if (right_lifebar_hp.value < right_lifebar_damage.value)
        {
            float to_set = right_lifebar_damage.value;

            to_set -= Time.deltaTime * damage_slider_speed;

            if (to_set < right_lifebar_hp.value)
                to_set = right_lifebar_hp.value;

            right_lifebar_damage.value = to_set;
        }

        float blue_decimal_value = panel_manager.GetPlayerInTeam(PanelTeam.BLUE).current_stamina % 1;
        float blue_integer_value = panel_manager.GetPlayerInTeam(PanelTeam.BLUE).current_stamina - blue_decimal_value;
        for (int i = 0; i < panel_manager.GetPlayerInTeam(PanelTeam.BLUE).max_stamina; i++)
        {
            if (blue_integer_value > i)
            {
                left_stamina_bars[i].value = 1;
            }
            else if (blue_integer_value == i)
            {
                left_stamina_bars[i].value = blue_decimal_value;
            }
            else if (blue_integer_value < i)
            {
                left_stamina_bars[i].value = 0;
            }
        }

        float red_decimal_value = panel_manager.GetPlayerInTeam(PanelTeam.RED).current_stamina % 1;
        float red_integer_value = panel_manager.GetPlayerInTeam(PanelTeam.RED).current_stamina - red_decimal_value;
        for (int i = 0; i < panel_manager.GetPlayerInTeam(PanelTeam.RED).max_stamina; i++)
        {
            if (red_integer_value > i)
            {
                right_stamina_bars[i].value = 1;
            }
            else if (red_integer_value == i)
            {
                right_stamina_bars[i].value = red_decimal_value;
            }
            else if (red_integer_value < i)
            {
                right_stamina_bars[i].value = 0;
            }
        }

    }

    float GetNormalizedHealth(PanelTeam team)
    {
        return ((float)panel_manager.GetPlayerInTeam(team).current_hp / (float)panel_manager.GetPlayerInTeam(team).max_hp);
    }
}