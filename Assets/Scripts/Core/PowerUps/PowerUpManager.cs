using UnityEngine;
using System;
using System.Collections.Generic;
using MagicWords.Core.Config;

namespace MagicWords.Core.PowerUps
{
    public class PowerUpManager : MonoBehaviour, IPowerUpManager
    {
        [SerializeField] private Transform powerUpsContainer;
        [SerializeField] private PowerUpConfig[] powerUpConfigs;

        private Dictionary<PowerUpType, IPowerUp> powerUps;
        private bool isInitialized;

        public event Action<PowerUpType> OnPowerUpActivated;
        public event Action<PowerUpType> OnPowerUpDeactivated;
        public event Action<PowerUpType, int> OnPowerUpCountChanged;

        private void Awake()
        {
            powerUps = new Dictionary<PowerUpType, IPowerUp>();
            InitializePowerUps();
        }

        private void InitializePowerUps()
        {
            if (isInitialized) return;

            foreach (var config in powerUpConfigs)
            {
                CreatePowerUp(config);
            }

            isInitialized = true;
        }

        private void CreatePowerUp(PowerUpConfig config)
        {
            if (powerUps.ContainsKey(config.type))
            {
                Debug.LogWarning($"PowerUp of type {config.type} already exists!");
                return;
            }

            // Create GameObject for the PowerUp
            var powerUpGO = new GameObject($"PowerUp_{config.type}");
            powerUpGO.transform.SetParent(powerUpsContainer);

            // Add specific PowerUp component based on type
            IPowerUp powerUp = config.type switch
            {
                PowerUpType.ChangeCell => powerUpGO.AddComponent<ChangeCellPowerUp>(),
                PowerUpType.ChangeLetter => powerUpGO.AddComponent<ChangeLetterPowerUp>(),
                PowerUpType.FreezeTrap => powerUpGO.AddComponent<FreezeTrapPowerUp>(),
                _ => throw new ArgumentException($"Unknown PowerUp type: {config.type}")
            };

            // Initialize PowerUp
            powerUp.Initialize(config);
            powerUps.Add(config.type, powerUp);
        }

        public bool ActivatePowerUp(PowerUpType type)
        {
            if (!powerUps.TryGetValue(type, out var powerUp))
            {
                Debug.LogWarning($"PowerUp of type {type} not found!");
                return false;
            }

            if (!powerUp.CanActivate) return false;

            if (powerUp.Activate())
            {
                OnPowerUpActivated?.Invoke(type);
                OnPowerUpCountChanged?.Invoke(type, powerUp.RemainingUses);
                return true;
            }

            return false;
        }

        public void DeactivatePowerUp(PowerUpType type)
        {
            if (!powerUps.TryGetValue(type, out var powerUp))
            {
                Debug.LogWarning($"PowerUp of type {type} not found!");
                return;
            }

            powerUp.Deactivate();
            OnPowerUpDeactivated?.Invoke(type);
        }

        public void AddPowerUpUse(PowerUpType type, int count = 1)
        {
            if (!powerUps.TryGetValue(type, out var powerUp))
            {
                Debug.LogWarning($"PowerUp of type {type} not found!");
                return;
            }

            var currentCount = powerUp.RemainingUses;
            ((PowerUpBase)powerUp).Reset(); // This will reset to initial count
            powerUp.Initialize(new PowerUpConfig
            {
                type = type,
                initialCount = currentCount + count,
                cooldown = powerUp.CooldownTime
            });

            OnPowerUpCountChanged?.Invoke(type, powerUp.RemainingUses);
        }

        public bool HasAvailablePowerUp(PowerUpType type)
        {
            return powerUps.TryGetValue(type, out var powerUp) && powerUp.CanActivate;
        }

        public int GetPowerUpCount(PowerUpType type)
        {
            return powerUps.TryGetValue(type, out var powerUp) ? powerUp.RemainingUses : 0;
        }

        public float GetPowerUpCooldown(PowerUpType type)
        {
            return powerUps.TryGetValue(type, out var powerUp) ? powerUp.RemainingCooldown : 0;
        }

        private void OnDestroy()
        {
            powerUps.Clear();
            OnPowerUpActivated = null;
            OnPowerUpDeactivated = null;
            OnPowerUpCountChanged = null;
        }
    }
}