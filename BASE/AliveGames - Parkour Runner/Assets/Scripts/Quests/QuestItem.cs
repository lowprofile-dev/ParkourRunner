﻿using UnityEngine;

public class QuestItem : MonoBehaviour
{
    [SerializeField] private QuestData _data;

    public QuestData Data { get { return _data; } }
}