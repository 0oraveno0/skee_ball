using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    public int shoot_bool = 0;
    public Transform t;
    public Rigidbody rigi;
    public int score;

    public void spawn_ball()
    {
        StartCoroutine("_spawn_ball");
    }

    IEnumerator _spawn_ball()
    {
        yield return new WaitForSeconds(5f);
        rigi.velocity = Vector3.zero;
        t.position = GM.instance._in_game.ball_spawn_point.position;
        shoot_bool = 0;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.name == "block_2")
        {
            shoot_bool++;
            t.position = new Vector3(0, transform.position.y, transform.position.z);
            rigi.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rigi.isKinematic = true;
            GM.instance._in_game.block[1].gameObject.SetActive(true);
            GM.instance._in_game.block[0].gameObject.SetActive(false);
            GM.instance._in_game.holding_ball_index = int.Parse(gameObject.name);
        }
        else
        {
            GM.instance._in_game.main_score_int += int.Parse(col.name) + score;
            gameObject.transform.position = new Vector3(0, -999, 0);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "block_1")
        {
            GM.instance._in_game.block[0].gameObject.SetActive(true);
        }
    }
}
