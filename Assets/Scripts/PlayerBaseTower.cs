using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;

namespace Default
{
    public class PlayerBaseTower : MonoBehaviour, ITower
    {
        public GameManager GameManager { get; set; }

        public GameObject TowerObject => gameObject;
        public TowerType Type => type;
        public float Health => health;

        public Action Destroyed { get; set; }

        [SerializeField] private LineRenderer projectileRenderer;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image rechargeBar;

        [Header("Config")]
        [SerializeField] private TowerType type;
        [SerializeField] private float initialHealth = 300f;
        [SerializeField] private float attackRange = 20f;
        [SerializeField] private float attackDamage = 70f;
        [SerializeField] private float rechargeTime = 4f;

        private float health;
        private bool isCharged = false;
        private float chargeTimer = 0f;

        private Tween attackTween;
        private bool gameRunning;

        private const float ATTACK_ANIMATION_DURATION = 0.4f;


        public void Initialize(GameManager gameManager)
        {
            GameManager = gameManager;

            Reset();
            gameRunning = true;
        }

        private void Update()
        {
            if (gameRunning)
            {
                HandleRecharge();
            }
        }

        private void Die()
        {
            GameManager.OnPlayerBaseDestroyed();

            this.DOKill();
        }

        public void Reset()
        {
            chargeTimer = rechargeTime;
            health = initialHealth;
        }

        private void HandleRecharge()
        {
            if (!isCharged)
            {
                chargeTimer -= Time.deltaTime;
                rechargeBar.fillAmount = 1f - (chargeTimer / rechargeTime);

                if (chargeTimer <= 0f)
                {
                    isCharged = true;
                }
            }
            else
            {
                if (isCharged)
                {
                    foreach (var enemy in GameManager.AllAliveEnemies)
                    {
                        var distance = Vector2.Distance(transform.position, enemy.EnemyObject.transform.position);

                        if (distance <= attackRange)
                        {
                            Attack(enemy);
                            Recharge();
                        }
                    }
                }
            }
        }

        private void Attack(IEnemyUnit target)
        {
            target.TakeDamage(attackDamage);
            AttackAnimation(target.EnemyObject.transform);
        }

        private void AttackAnimation(Transform target)
        {
            var timer = 0f;

            attackTween = DOTween.To(() => timer, x => timer = x, ATTACK_ANIMATION_DURATION, ATTACK_ANIMATION_DURATION)
                .SetTarget(this)
                .OnUpdate(() =>
                {
                    if (target == null)
                    {
                        this.DOKill(true);
                        return;
                    }
                    projectileRenderer.SetPosition(1, target.position - transform.position);
                })
                .OnComplete(() => projectileRenderer.SetPosition(1, Vector3.zero));
        }

        private void Recharge()
        {
            isCharged = false;
            chargeTimer = rechargeTime;
        }

        public void TakeDamage(float damage)
        {
            health -= damage;

            healthBar.fillAmount = health / initialHealth;

            if (health <= 0)
            {
                Die();
            }
        }
    }
}
