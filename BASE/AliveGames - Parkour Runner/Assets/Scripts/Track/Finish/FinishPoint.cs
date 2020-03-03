﻿using System.Collections;
using UnityEngine;
using ParkourRunner.Scripts.Managers;
using ParkourRunner.Scripts.Player.InvectorMods;

public class FinishPoint : MonoBehaviour
{
    [SerializeField] private float _resultWindowDelay;

    private void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponent<ParkourThirdPersonController>();

        if (target != null)
        {
            StartCoroutine(FinishLevelProcess());
        }
    }

    private IEnumerator FinishLevelProcess()
    {
        var hud = HUDManager.Instance;
        var player = ParkourThirdPersonController.instance;
        var manager = GameManager.Instance;

        var input = player.GetComponent<ParkourThirdPersonInput>();
        input.Stop();

        yield return new WaitForSeconds(_resultWindowDelay);

        GameManager.Instance.CompleteLevel();
        hud.PostMortemScreen.CheckRateMe();
    }
}