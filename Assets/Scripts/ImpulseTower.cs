using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Default
{
    public class ImpulseTower : MonoBehaviour, ITower
    {
        public GameManager GameManager { get; set; }

        public GameObject TowerObject => gameObject;
        public TowerType Type => type;
        public float Health => health;

        public Action Destroyed { get; set; }

        [SerializeField] private Image healthBar;
        [SerializeField] private Image chargeBar;
        [SerializeField] private RawImage impulseEffectImage;
        [SerializeField] private Color impulseEffectColor;

        [Header("Config")]
        [SerializeField] private TowerType type;
        [SerializeField] private float initialHealth = 100f;
        [SerializeField] private float attackRange = 20f;
        [SerializeField] private float maxDamage = 70f;
        [SerializeField] private int minTargets = 5; // Minimum number of targets before impulse
        [SerializeField] private float chargeUpTime = 12f; // Charge up time before sending impulse

        private float health;
        private bool isCharged = false;
        private float chargeTimer = 0f;

        private Tween impulseTween;


        private void Start()
        {
            health = initialHealth;

            GameManager.AllAliveTowers.Add(this);
        }

        private void Update()
        {
            HandleImpulseChargeUp();
        }

        private void OnDestroy()
        {
            GameManager.OnTowerDestroyed(this);
            GameManager.AllAliveTowers.Remove(this);

            this.DOKill();
        }

        private void HandleImpulseChargeUp()
        {
            if (!isCharged)
            {
                chargeTimer -= Time.deltaTime;
                chargeBar.fillAmount = 1f - (chargeTimer / chargeUpTime);

                if (chargeTimer <= 0f)
                {
                    isCharged = true;
                }
            }
            else
            {
                int targetsInRange = 0;

                foreach (var enemy in GameManager.AllAliveEnemies)
                {
                    var distance = Vector2.Distance(transform.position, enemy.EnemyObject.transform.position);
                    if (distance <= attackRange) targetsInRange++;
                }

                if (targetsInRange >= minTargets && isCharged)
                {
                    SendImpulseAttack();
                    ResetCharge();
                }
            }
        }

        private void SendImpulseAttack()
        {
            foreach (var enemy in GameManager.AllAliveEnemies)
            {
                var distance = Vector2.Distance(transform.position, enemy.EnemyObject.transform.position);

                if (distance <= attackRange)
                {
                    var normalizedDistance = distance / attackRange;
                    var falloff = DamageFalloff(normalizedDistance);
                    var damageAmount = maxDamage * falloff;

                    impulseEffectImage.color = impulseEffectColor;
                    impulseTween = DOTween.ToAlpha(() => impulseEffectImage.color, x => impulseEffectImage.color = x, 0f, 0.5f).SetTarget(this);

                    enemy.TakeDamage(damageAmount);
                }
            }

        }

        private void ResetCharge()
        {
            isCharged = false;
            chargeTimer = chargeUpTime;
        }

        private float DamageFalloff(float x) // simple ease in oout square function
        {
            return x < 0.5f ? 2 * x * x : 1 - ((-2 * x + 2) * (-2 * x + 2)) / 2;
        }

        public void TakeDamage(float damage)
        {
            health -= damage;

            healthBar.fillAmount = health / initialHealth;

            if (health <= 0)
            {
                Destroyed?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
