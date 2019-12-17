﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickTriggerController : MonoBehaviour
{
    [SerializeField] private Collider _collider;

    private void OnEnable()
    {
        if (_collider == null)
            _collider = this.gameObject.GetComponentInChildren<Collider>();

        if (_collider != null)
            _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_collider != null)
                _collider.enabled = false;
        }
    }
}