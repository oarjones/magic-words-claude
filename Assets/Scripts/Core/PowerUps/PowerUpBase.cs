using MagicWords.Core.Config;
using UnityEngine;

namespace MagicWords.Core.PowerUps
{
    public abstract class PowerUpBase : MonoBehaviour, IPowerUp
    {
        [SerializeField] protected PowerUpConfig config;
        [SerializeField] protected PowerUpVisuals visuals;

        protected bool isActive;
        protected int remainingUses;
        protected float currentCooldown;
        protected bool isInitialized;

        public PowerUpType Type => config != null ? config.type : PowerUpType.ChangeCell;
        public bool IsActive => isActive;
        public bool CanActivate => !isActive && remainingUses > 0 && currentCooldown <= 0;
        public int RemainingUses => remainingUses;
        public float CooldownTime => config != null ? config.cooldown : 0;
        public float RemainingCooldown => currentCooldown;

        public virtual void Initialize(PowerUpConfig config)
        {
            this.config = config;
            remainingUses = config.initialCount;
            currentCooldown = 0;
            isActive = false;
            isInitialized = true;

            if (visuals != null)
            {
                visuals.UpdateVisuals(isActive, remainingUses, 0);
            }
        }

        public virtual bool Activate()
        {
            if (!isInitialized || !CanActivate) return false;

            isActive = true;
            remainingUses--;

            if (visuals != null)
            {
                visuals.ShowActivationEffect();
                visuals.UpdateVisuals(isActive, remainingUses, 0);
            }

            return true;
        }

        public virtual void Deactivate()
        {
            if (!isActive) return;

            isActive = false;
            currentCooldown = config.cooldown;

            if (visuals != null)
            {
                visuals.ShowDeactivationEffect();
                visuals.UpdateVisuals(isActive, remainingUses, 1);
            }
        }

        public virtual void Reset()
        {
            isActive = false;
            currentCooldown = 0;
            remainingUses = config.initialCount;

            if (visuals != null)
            {
                visuals.UpdateVisuals(isActive, remainingUses, 0);
            }
        }

        public virtual void UpdateCooldown(float deltaTime)
        {
            if (currentCooldown > 0)
            {
                currentCooldown = Mathf.Max(0, currentCooldown - deltaTime);

                if (visuals != null)
                {
                    float progress = 1 - (currentCooldown / config.cooldown);
                    visuals.ShowCooldownProgress(progress);
                }
            }
        }

        protected virtual void OnEnable()
        {
            if (config != null && isInitialized)
            {
                Reset();
            }
        }

        protected virtual void Update()
        {
            if (isInitialized)
            {
                UpdateCooldown(Time.deltaTime);
            }
        }
    }

    [System.Serializable]
    public class PowerUpVisuals
    {
        public ParticleSystem activationEffect;
        public ParticleSystem deactivationEffect;
        public UnityEngine.UI.Image cooldownImage;
        public TMPro.TextMeshProUGUI countText;
        public UnityEngine.UI.Image iconImage;

        public void ShowActivationEffect()
        {
            if (activationEffect != null)
            {
                activationEffect.Play();
            }
        }

        public void ShowDeactivationEffect()
        {
            if (deactivationEffect != null)
            {
                deactivationEffect.Play();
            }
        }

        public void UpdateVisuals(bool isActive, int remainingUses, float cooldownPercent)
        {
            if (countText != null)
            {
                countText.text = remainingUses.ToString();
                countText.color = remainingUses > 0 ? Color.white : Color.gray;
            }

            if (iconImage != null)
            {
                iconImage.color = isActive ? Color.white :
                    (remainingUses > 0 ? Color.white * 0.7f : Color.gray);
            }

            ShowCooldownProgress(1 - cooldownPercent);
        }

        public void ShowCooldownProgress(float progress)
        {
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = progress;
            }
        }
    }
}