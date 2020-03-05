﻿using System;
using Managers;
using UnityEngine;
using ParkourRunner.Scripts.Managers;
using ParkourRunner.Scripts.Player.InvectorMods;
using Photon.Pun;

public class FinishMessage : MonoBehaviour
{
    public static event Action OnFinishLevelMessage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PhotonGameManager.IsMultiplayer) {
                if(other.GetComponent<PhotonView>().IsMine) {
                    HUDManager.Instance.ShowGreatMessage(HUDManager.Messages.LevelComplete);
                }

                if (PhotonNetwork.IsMasterClient) {
                    PhotonGameManager.OnPlayerFinish(other.gameObject);
                }
                return;
            }
            HUDManager.Instance.ShowGreatMessage(HUDManager.Messages.LevelComplete);
            EnvironmentController.CheckKeys();

            int level = PlayerPrefs.GetInt(EnvironmentController.LEVEL_KEY);
            int maxLevel = PlayerPrefs.GetInt(EnvironmentController.MAX_LEVEL);

            bool isBaseLevels = PlayerPrefs.GetInt(EnvironmentController.ENDLESS_KEY) == 0 && PlayerPrefs.GetInt(EnvironmentController.TUTORIAL_KEY) == 0;

            if (isBaseLevels)
                OnFinishLevelMessage.SafeInvoke();

            if (isBaseLevels && level == maxLevel)
            {
                maxLevel++;
                PlayerPrefs.SetInt(EnvironmentController.MAX_LEVEL, maxLevel);
                PlayerPrefs.Save();
            }
        }
    }
}