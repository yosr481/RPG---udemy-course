﻿using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable
    {
        [SerializeField] private float startingBalance = 1000f;

        private float balance = 0;

        public event Action onChange;

        private void Awake()
        {
            balance = startingBalance;
        }

        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
            onChange?.Invoke();
        }
        public object CaptureState()
        {
            return balance;
        }
        public void RestoreState(object state)
        {
            balance = (float)state;
        }
    }
}