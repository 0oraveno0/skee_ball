using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class in_game : MonoBehaviour
{
    public ball[] _ball;
    public GameObject[] block, target_board;
    public GameObject in_game_UI, count_bg, result_popup;
    public Transform ball_spawn_point;
    public EventTrigger shoot;
    public Image force_bar;
    public Text start_count, main_timer_text, main_score_text;
    public float force;
    public int holding_ball_index;
    public bool start_game_bool = false;
    private Vector3[] ball_reset_pos;
    private float hold_time = 0;

    private int _main_score_int;
    public int main_score_int
    {
        get
        {
            return _main_score_int;
        }
        set
        {
            _main_score_int = value;
            main_score_text.text = "score\n" + _main_score_int.ToString();
        }
    }

    private int _main_timer_int;
    public int main_timer_int
    {
        get
        {
            return _main_timer_int;
        }
        set
        {
            _main_timer_int = value;
            main_timer_text.text = "timer\n" + _main_timer_int.ToString();
            if (_main_timer_int > 0)
            {
                StartCoroutine("main_timer_count");
            }
            else
            {
                result();
            }
        }
    }

    public void init()
    {
        ball_reset_pos = new Vector3[_ball.Length];
        for (int i = 0; i < _ball.Length; i++)
        {
            _ball[i].rigi.mass = 1 + Random.Range(-0.2f,0.2f);
            ball_reset_pos[i] = _ball[i].transform.position;
        }
    }

    public void start_game()
    {
        reset();
        start_game_bool = true;
        StartCoroutine("_start_game");
    }

    public void reset()
    {
        holding_ball_index = 0;
        start_game_bool = false;
        main_score_int = 0;
        main_timer_text.text = "timer\n";
        result_popup.gameObject.SetActive(false);
        force_bar.fillAmount = 0;
        block[0].SetActive(true);
        block[1].SetActive(true);
        for (int i = 0; i < _ball.Length; i++)
        {
            _ball[i].StopAllCoroutines();
            _ball[i].rigi.velocity = Vector3.zero;
            _ball[i].transform.position = ball_reset_pos[i];
            _ball[i].shoot_bool = 0;
            _ball[i].rigi.isKinematic = false;
        }
    }

    IEnumerator _start_game()
    {
        start_count.text = "3";
        yield return new WaitForSeconds(1f);
        start_count.text = "2";
        yield return new WaitForSeconds(1f);
        start_count.text = "1";
        yield return new WaitForSeconds(1f);
        start_count.text = "start";
        block[1].gameObject.SetActive(false);
        force_bar.gameObject.SetActive(true);

        StartCoroutine("fire_cool_down");
        yield return new WaitForSeconds(1f);
        start_count.text = "";

        if(GM.instance._menu.diffcult_str == "normal")
        {
            main_timer_int = GM.instance._level_data.timer[0];
            force = GM.instance._level_data.force[0];
        }
        else if (GM.instance._menu.diffcult_str == "hard")
        {
            main_timer_int = GM.instance._level_data.timer[1];
            force = GM.instance._level_data.force[1];
        }
        count_bg.SetActive(true);
    }

    IEnumerator main_timer_count()
    {
        yield return new WaitForSeconds(1f);
        main_timer_int--;
    }

    private void Update()
    {
        ball_ready_position_y(holding_ball_index);

        if (Input.GetKeyDown(KeyCode.Escape) && GM.instance._menu.gameObject.activeSelf)
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && result_popup.gameObject.activeSelf)
        {
            result_popup.gameObject.SetActive(false);
            in_game_UI.gameObject.SetActive(false);
            GM.instance._menu.gameObject.SetActive(true);
        }
    }

    public void ball_ready_position_y(int index)
    {
        if (_ball[index].shoot_bool != 1 || start_game_bool == false)
        {
            return;
        }

        Ray ray =  Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            _ball[index].transform.position = new Vector3(
                Mathf.Clamp(raycastHit.point.x, -0.8f, 0.8f),
                _ball[index].transform.position.y,
                _ball[index].transform.position.z);
        }
    }

    public void on_shoot_down()
    {
        if (_ball[holding_ball_index].shoot_bool != 1)
        {
            return;
        }

        force_bar.fillAmount = 0f;
        StartCoroutine("force_bar_ani");
    }

    public void on_shoot_up()
    {
        if (_ball[holding_ball_index].shoot_bool != 1)
        {
            return;
        }

        _ball[holding_ball_index].shoot_bool++;
        StopCoroutine("force_bar_ani");
        _ball[holding_ball_index].rigi.isKinematic = false;
        _ball[holding_ball_index].rigi.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _ball[holding_ball_index].rigi.AddForce(transform.forward * (force + 700f * force_bar.fillAmount));
        _ball[holding_ball_index].spawn_ball();
        block[1].gameObject.SetActive(false);
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
        float _bar_speed = GM.instance._menu.bar_speed;
        while (true)
        {
            hold_time += Time.deltaTime * _bar_speed;
            force_bar.fillAmount = Mathf.Abs(Mathf.Sin(hold_time));
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void result()
    {
        start_game_bool = false;
        StopAllCoroutines();
        shoot.enabled = false;
        force_bar.gameObject.SetActive(false);
        count_bg.SetActive(false);
        result_popup.gameObject.SetActive(true);
        result_panel rp = result_popup.GetComponent<result_panel>();
        rp.score.text = main_score_int.ToString();

        int diffcult_str = 0;
        if (GM.instance._menu.diffcult_str == "normal")
        {
            diffcult_str = 0;
        }
        else if (GM.instance._menu.diffcult_str == "hard")
        {
            diffcult_str = 1;
        }
        if (main_score_int > GM.instance._menu.highest_score_int)
        {
            rp.msg.text = "Amazing! You got the highest score!";
            rp.try_again.gameObject.SetActive(true);
        }
        else
        {
            rp.msg.text = "Your score is low. Try again";
            rp.try_again.gameObject.SetActive(false);
        }
        GM.instance._leaderboard.set_leaderBoard(diffcult_str, main_score_int.ToString() + ",");
    }
}
