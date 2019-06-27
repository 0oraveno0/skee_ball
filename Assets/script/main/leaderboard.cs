using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Proyecto26;

public class leaderboard : MonoBehaviour
{
    public Transform content;
    public GameObject _record;
    public Text diffcult_btn_text;
    public string leaderboard_str;
    public string leaderboard_str_hard;

    public void set_leaderBoard(int diffcult, string v)
    {
        if (diffcult == 0)
        {
            _set_leaderBoard(diffcult, PlayerPrefs.GetString("leaderboard"), v);
        }
        else if (diffcult == 1)
        {
            _set_leaderBoard(diffcult, PlayerPrefs.GetString("leaderboard_hard"), v);
        }
    }
    private void _set_leaderBoard(int diffcult, string str, string v)
    {
        str = str + v;
        string[] s = str.Split(',');
        int[] _s = new int[s.Length - 1];
        for (int i = 0; i < _s.Length; i++)
        {
            _s[i] = int.Parse(s[i]);
        }
        Array.Sort(_s);
        str = "";
        for (int i = 0; i < _s.Length; i++)
        {
            if (i < 5)
            {
                int _i = (_s.Length - 1) - i;
                str += _s[_i] + ",";
                if (i == 0)
                {
                    if (GM.instance._menu.highest_score_int < _s[_i])
                    {
                        GM.instance._menu.highest_score_int = _s[_i];
                        GM.instance._menu.highest_score_text.text = "highest score(from server)\n" + GM.instance._menu.highest_score_int.ToString();
                        put_to_database(GM.instance._menu.highest_score_int);
                    }
                }
            }
        }

        if (diffcult == 0)
        {
            PlayerPrefs.SetString("leaderboard", str);
        }
        else if (diffcult == 1)
        {
            PlayerPrefs.SetString("leaderboard_hard", str);
        }
    }

    public void diffcult_btn()
    {
        if (diffcult_btn_text.text == "normal")
        {
            diffcult_btn_text.text = "hard";
            leaderboard_btn();
        }
        else if (diffcult_btn_text.text == "hard")
        {
            diffcult_btn_text.text = "normal";
            leaderboard_btn();
        }
    }
    public void leaderboard_btn()
    {
        string[] _s = new string[5];
        if (diffcult_btn_text.text == "normal")
        {
            _s = PlayerPrefs.GetString("leaderboard").Split(',');
        }
        else if (diffcult_btn_text.text == "hard")
        {
            _s = PlayerPrefs.GetString("leaderboard_hard").Split(',');
        }

        foreach (Transform c in content)
        {
            GameObject.Destroy(c.gameObject);
        }
        for (int i = 0; i < _s.Length - 1; i++)
        {
            if (i < 5)
            {
                GameObject go = Instantiate(_record, content);
                record r = go.GetComponent<record>();
                r.rank.text = (i + 1).ToString();
                r.name.text = "uname";
                r.score.text = _s[i].ToString();
            }
        }
    }

    public void put_to_database(int highest_score)
    {
        server_record _record = new server_record();
        _record.highest_score = highest_score;
        RestClient.Put("https://skeeballsimulator.firebaseio.com/" + _record.id + ".json", _record);
    }
    
    public void get_from_database()
    {
        RestClient.Get<server_record>("https://skeeballsimulator.firebaseio.com/"  + "highest_score" + ".json").Then(response =>
        {
            server_record _server_record = response;
            GM.instance._menu.highest_score_int = _server_record.highest_score;
            GM.instance._menu.highest_score_text.text = "highest score(from server)\n" + _server_record.highest_score.ToString();
        });
    }

    public void delete_from_database()
    {
        RestClient.Delete("https://skeeballsimulator.firebaseio.com/" + ".json");
    }

    [System.Serializable]
    public class server_record{
        public string id = "highest_score";
        public int highest_score;
    }
}
