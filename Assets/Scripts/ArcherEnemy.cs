using UnityEngine;
using UnityEngine.UI;

namespace Default
{
    public class ArcherEnemy : MonoBehaviour, IEnemyUnit
    {
        public GameManager GameManager { get; set; }

        public EnemyType Type => type;
        public float Damage => damage;
        public float DamageRate => damageRate;
        public float Health => health;
        public float Speed => speed;
        public float Range => range;
        public GameObject EnemyObject => gameObject;

        [SerializeField] private Image healthBar;
        [SerializeField] private Animator animator;

        [Header("Config")]
        [SerializeField] private EnemyType type;
        [SerializeField] private float damage = 20f;
        [SerializeField] private float damageRate = 2.5f;
        [SerializeField] private float initialHealth = 80f;
        [SerializeField] private float speed = 2f;
        [SerializeField] private float range = 14f;

        private ITower currentTarget;
        private GameObject currentTargetObject;
        private float attackCooldown;
        private float health;
        private int rangeAttackAnimationHash;


        private void Start()
        {
            UpdateCurrentTarget();

            rangeAttackAnimationHash = Animator.StringToHash("ArcherAttack");
            GameManager.AllAliveEnemies.Add(this);
        }

        private void OnDestroy()
        {
            GameManager.OnEnemyUnitKilled(this);
        }

        private void Update()
        {
            HandleAttackCooldown();

            if (currentTargetObject != null)
            {
                if (!IsWithinAttackRange())
                {
                    MoveTowardsTarget();
                    return;
                }

                if (attackCooldown <= 0)
                {
                    AttackTarget();
                    ResetAttackCooldown();
                }
            }
            else
            {
                UpdateCurrentTarget();
            }
        }

        private void MoveTowardsTarget()
        {
            var direction = (currentTargetObject.transform.position - transform.position).normalized;
            transform.position += new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime;
        }

        private bool IsWithinAttackRange()
        {
            var distance = Vector2.Distance(transform.position, currentTargetObject.transform.position);
            return distance <= range;
        }

        private void AttackTarget()
        {
            animator.SetTrigger(rangeAttackAnimationHash);
            currentTarget.TakeDamage(damage);
        }

        private void ResetAttackCooldown()
        {
            attackCooldown = damageRate;
        }

        private void HandleAttackCooldown()
        {
            if (attackCooldown > 0)
            {
                attackCooldown -= Time.deltaTime;
            }
        }

        public void TakeDamage(float damage)
        {
            initialHealth -= damage;

            healthBar.fillAmount = Health / initialHealth;

            if (initialHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        private void UpdateCurrentTarget()
        {
            if (GameManager.AllAliveTowers.Count <= 0) { return; }

            var closestDistance = 1000f;
            ITower closestTower = null;

            foreach (var tower in GameManager.AllAliveTowers)
            {
                if (tower == null) { continue; }

                var distance = Vector2.Distance(transform.position, tower.TowerObject.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTower = tower;
                }
            }

            currentTarget = closestTower;
            currentTargetObject = closestTower.TowerObject;
        }
    }
}