using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static int maxEnemies = 10;
    public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();
    private static int nextEnemyId = 1;

    public int id;
    public EnemyState state;
    public Player target;
    public CharacterController controller;
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 8f;
    public float health;
    public float maxHealth = 100f;
    public float detectionRange = 30f;
    public float shootRange = 15f;
    public float shootAccuracy = 0.1f;
    public float patrolDuration = 3f;
    public float idleDuration = 1f;

    private bool isPatrolRoutineRunning;
    private float yVelocity = 0;

    private void Start()
    {
        id = nextEnemyId;
        nextEnemyId++;
        enemies.Add(id, this);

        ServerSend.SpawnEnemy(this);

        state = EnemyState.patrol;
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        patrolSpeed *= Time.fixedDeltaTime;
        chaseSpeed *= Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case EnemyState.idle:
                LookForPlayer();
                break;
            case EnemyState.patrol:
                if (!LookForPlayer())
                {
                    Patrol();
                }
                break;
            case EnemyState.chase:
                Chase();
                break;
            case EnemyState.attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private bool LookForPlayer()
    {
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                Vector3 _enemyToPlayer = _client.player.transform.position - transform.position;
                if (_enemyToPlayer.magnitude <= detectionRange)
                {
                    if (Physics.Raycast(shootOrigin.position, _enemyToPlayer, out RaycastHit _hit, detectionRange))
                    {
                        if (_hit.collider.CompareTag("Player"))
                        {
                            target = _hit.collider.GetComponent<Player>();
                            if (isPatrolRoutineRunning)
                            {
                                isPatrolRoutineRunning = false;
                                StopCoroutine(StartPatrol());
                            }

                            state = EnemyState.chase;
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private void Patrol()
    {
        if (!isPatrolRoutineRunning)
        {
            StartCoroutine(StartPatrol());
        }

        Move(transform.forward, patrolSpeed);
    }

    private IEnumerator StartPatrol()
    {
        isPatrolRoutineRunning = true;
        Vector2 _randomPatrolDirection = Random.insideUnitCircle.normalized;
        transform.forward = new Vector3(_randomPatrolDirection.x, 0f, _randomPatrolDirection.y);

        yield return new WaitForSeconds(patrolDuration);

        state = EnemyState.idle;

        yield return new WaitForSeconds(idleDuration);

        state = EnemyState.patrol;
        isPatrolRoutineRunning = false;
    }

    private void Chase()
    {
        if (CanSeeTarget())
        {
            Vector3 _enemyToPlayer = target.transform.position - transform.position;
            
            if (_enemyToPlayer.magnitude <= shootRange)
            {
                state = EnemyState.attack;
            }
            else
            {
                Move(_enemyToPlayer, chaseSpeed);
            }
        }
        else
        {
            target = null;
            state = EnemyState.patrol;
        }
    }

    private void Attack()
    {
        if (CanSeeTarget())
        {
            Vector3 _enemyToPlayer = target.transform.position - transform.position;
            transform.forward = new Vector3(_enemyToPlayer.x, 0f, _enemyToPlayer.z);

            if (_enemyToPlayer.magnitude <= shootRange)
            {
                Shoot(_enemyToPlayer);
            }
            else
            {
                Move(_enemyToPlayer, chaseSpeed);
            }
        }
        else
        {
            target = null;
            state = EnemyState.patrol;
        }
    }

    private void Move(Vector3 _direction, float _speed)
    {
        _direction.y = 0f;
        transform.forward = _direction;
        Vector3 _movement = transform.forward * _speed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
        }
        yVelocity += gravity;

        _movement.y = yVelocity;
        controller.Move(_movement);

        ServerSend.EnemyPosition(this);
    }

    private void Shoot(Vector3 _shootDirection)
    {
        if (Physics.Raycast(shootOrigin.position, _shootDirection, out RaycastHit _hit, shootRange))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                if (Random.value <= shootAccuracy)
                {
                    _hit.collider.GetComponent<Player>().TakeDamage(50f);
                }
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;

            enemies.Remove(id);
            Destroy(gameObject);
        }

        ServerSend.EnemyHealth(this);
    }

    private bool CanSeeTarget()
    {
        if (target == null)
        {
            return false;
        }

        if (Physics.Raycast(shootOrigin.position, target.transform.position - transform.position, out RaycastHit _hit, detectionRange))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
}

public enum EnemyState
{
    idle,
    patrol,
    chase,
    attack
}
