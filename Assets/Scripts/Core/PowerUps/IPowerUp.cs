using UnityEngine;
using System;
using MagicWords.Core.Config;
using MagicWords.Core.Board;

namespace MagicWords.Core.PowerUps
{
    public interface IPowerUp
    {
        PowerUpType Type { get; }
        bool IsActive { get; }
        bool CanActivate { get; }
        int RemainingUses { get; }
        float CooldownTime { get; }
        float RemainingCooldown { get; }

        void Initialize(PowerUpConfig config);
        bool Activate();
        void Deactivate();
        void Reset();
        void UpdateCooldown(float deltaTime);
    }

    public interface IPowerUpManager
    {
        event Action<PowerUpType> OnPowerUpActivated;
        event Action<PowerUpType> OnPowerUpDeactivated;
        event Action<PowerUpType, int> OnPowerUpCountChanged;

        bool ActivatePowerUp(PowerUpType type);
        void DeactivatePowerUp(PowerUpType type);
        void AddPowerUpUse(PowerUpType type, int count = 1);
        bool HasAvailablePowerUp(PowerUpType type);
        int GetPowerUpCount(PowerUpType type);
        float GetPowerUpCooldown(PowerUpType type);
    }

    public interface IPowerUpEffect
    {
        PowerUpType Type { get; }
        void ApplyEffect(Tile targetTile = null);
        void RemoveEffect();
        bool CanApplyTo(Tile tile);
    }

    public interface IPowerUpVisuals
    {
        void ShowActivationEffect();
        void ShowDeactivationEffect();
        void UpdateVisuals(bool isActive, int remainingUses, float cooldownPercent);
        void ShowCooldownProgress(float progress);
    }
}