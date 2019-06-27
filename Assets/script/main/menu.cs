using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class menu : MonoBehaviour
{
    public Text highest_score_text;
    public int highest_score_int;
    public Text diffcult_text;
    public Text bar_speed_text;
    public Image force_bar;
    public GameObject setting_popup;
    public EventTrigger shoot;
    private float hold_time;

    private float _bar_speed;
    public float bar_speed
    {
        get {
            return PlayerPrefs.GetFloat("bar_speed");
        }
        set
        {
            _bar_speed = value;
            bar_speed_text.text = _bar_speed.ToString();
            PlayerPrefs.SetFloat("bar_speed", _bar_speed);
        }
    }

    private string _diffcult_str;
    public string diffcult_str
    {
        get
        {
            return PlayerPrefs.GetString("diffcult");
        }
        set
        {
            _diffcult_str = value;
            diffcult_text.text = _diffcult_str;
            if(_diffcult_str == "normal")
            {
                GM.instance._in_game.target_board[0].gameObject.SetActive(true);
                GM.instance._in_game.target_board[1].gameObject.SetActive(false);
            }
            else if(_diffcult_str == "hard")
            {
                GM.instance._in_game.target_board[0].gameObject.SetActive(false);
                GM.instance._in_game.target_board[1].gameObject.SetActive(true);
            }
            PlayerPrefs.SetString("diffcult", _diffcult_str);
        }
    }


    public void init()
    {
        if(PlayerPrefs.GetString("diffcult") == "")
        {
            diffcult_str = "normal";
        }
        else
        {
            diffcult_str = PlayerPrefs.GetString("diffcult");
        }
        if (PlayerPrefs.GetFloat("bar_speed") == 0)
        {
            bar_speed = 6;
        }
        else
        {
            bar_speed = PlayerPrefs.GetFloat("bar_speed");
        }
        GM.instance._leaderboard.get_from_database();

        if (PlayerPrefs.GetString("leaderboard") != "") {
            string[] _s = PlayerPrefs.GetString("leaderboard").Split(',');
            //highest_score_int[0] = int.Parse(_s[0]);
        }
        if (PlayerPrefs.GetString("leaderboard_hard") != "") {
            string[] _s = PlayerPrefs.GetString("leaderboard_hard").Split(',');
            //highest_score_int[1] = int.Parse(_s[0]);
        }
    }

    public void start_btn()
    {
        gameObject.SetActive(false);
        GM.instance._in_game.in_game_UI.SetActive(true);
        GM.instance._in_game.start_game();
    }

    public void clear_btn()
    {
        PlayerPrefs.SetString("leaderboard","");
        PlayerPrefs.SetString("leaderboard_hard", "");
        bar_speed = 6;
        diffcult_str = "normal";
        PlayerPrefs.SetString("diffcult", diffcult_text.text);
    }

    public void diffcult_btn()
    {
        if(diffcult_text.text == "normal")
        {
            diffcult_str = "hard";
        }
        else
        {
            diffcult_str = "normal";
        }
    }

    public void sub_bar_speed()
    {
        if(bar_speed > 3)
        {
            bar_speed -= 0.5f;
        }
    }

    public void add_bar_speed()
    {
        if (bar_speed < 8)
        {
            bar_speed += 0.5f;
        }
    }

    public void on_shoot_down()
    {
        force_bar.fillAmount = 0f;
        StartCoroutine("force_bar_ani");
    }

    public void on_shoot_up()
    {
        StopCoroutine("force_bar_ani");
        StartCoroutine("fire_cool_down");
    }

    IEnumerator fire_cool_down()
    {
        shoot.enabled = false;
        float bar_fill_amount = force_bar.fillAmount;
        for (hold_time = 1f; hold_time >= 0f; hold_time -= Time.deltaTime)
        {
            force_bar.fillAmount = hold_time * bar_fill_amount;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        force_bar.fillAmount = 0;
        shoot.enabled = true;
    }

    IEnumerator force_bar_ani()
    {
        float _bar_speed = bar_speed;
        while (true)
        {
            hold_time += Time.deltaTime * _bar_speed;
            force_bar.fillAmount = Mathf.Abs(Mathf.Sin(hold_time));
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
