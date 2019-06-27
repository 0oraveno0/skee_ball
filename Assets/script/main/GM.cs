using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GM : MonoBehaviour
{
    public in_game _in_game;
    public menu _menu;
    public leaderboard _leaderboard;
    public static GM instance = null;
    public level_data _level_data;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void load_text()
    {
        string path = "";
        string json = "";
#if UNITY_EDITOR
        path = Application.dataPath + "/StreamingAssets/level_data.txt";
        json = File.ReadAllText(path);
#elif UNITY_ANDROID
        path = "jar:file://" + Application.dataPath + "!/assets/level_data.txt";
        WWW wwwfile = new WWW(path);
        while (!wwwfile.isDone) { }
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, "level_data.t");
        File.WriteAllBytes(filepath, wwwfile.bytes);
   
        StreamReader wr = new StreamReader(filepath);
            string line;
            while ((line = wr.ReadLine()) != null)
            {
                json += line;
            }
#endif

        _level_data = JsonUtility.FromJson<level_data>(json);
    }

    public void Start()
    {
        _menu.init();
        _in_game.init();
        load_text();
    }

    [System.Serializable]
    public class level_data
    {
        public int[] timer;
        public int[] force;
    }
}
