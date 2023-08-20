using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elympics;
using TMPro;
using System;

public class GameFinisher : ElympicsMonoBehaviour
{
    public TaskManager[] allPlayers = null;
    private Transform currentPlayer;
    private bool hasKey;
    [SerializeField] private GameObject Key;

    private ElympicsInt numberOfWinners = new ElympicsInt(0);

    public void Start()
    {
        allPlayers = FindObjectsOfType<TaskManager>();
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if(currentPlayer == null && col.transform.tag == "Work") 
        {
            currentPlayer = col.transform;
            if (currentPlayer.Find("Key(Clone)")) hasKey = true;
        }
    }

    public void OnTriggerStay2D(Collider2D col)
    {
        if(col.transform == currentPlayer)
        {
            if(currentPlayer.GetComponentInParent<PlayerHandler>().wantsToFinish && !hasKey)
            {
                if (!Elympics.IsServer) return;
                if (!CanFinish()) return;
                hasKey = true;
                numberOfWinners.Value += 1;
                currentPlayer.GetComponentInParent<PlayerHandler>().wantsToFinish = false;
                var key = ElympicsInstantiate("Key", ElympicsPlayer.All);
                key.AddComponent<Key>().OnCreate(currentPlayer);
                Win();

            }
        }
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (currentPlayer != null && col.transform == currentPlayer) {
            currentPlayer = null;
            hasKey = false;
        }
    }

    private bool CanFinish()
    {
        return currentPlayer.GetComponentInParent<TaskManager>().AreTasksCompleted();
    }

    private void Win()
    {
        if (numberOfWinners.Value == allPlayers.Length-1 || numberOfWinners.Value == allPlayers.Length) 
        {
            //currentPlayer.GetComponentInParent<PlayerHandler>().enabled = false;
            foreach(TaskManager player in allPlayers)
            {
                // if(player.AreTasksCompleted())
                // {
                    player.finished.Value = true;
                    Debug.Log("cant take much more");
                // }
            }
            if(!Elympics.IsServer) return;
            GetComponent<GameManager>().ChangeGameState(GameState.MatchEnded);
            StartCoroutine(GetComponent<GameManager>().WaitToEnd());
            // Elympics.EndGame();
            // this.gameObject.SetActive(false);
            GetComponent<GameFinisher>().enabled = false;
        }
    }
}
