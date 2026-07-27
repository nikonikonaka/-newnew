using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("ターゲット・環境")]
    [SerializeField] private Transform[] players;
    [SerializeField] private LayerMask wallLayer;

    [Header("移動・距離管理")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float attackRange = 6f;
    [SerializeField] private float escapeRange = 2f;
    [SerializeField] private float strafeRadius = 4f;
    [SerializeField] private float strafeSideStepChance = 0.3f;
    [SerializeField] private float sideStepDistance = 1.2f;

    [Header("攻撃・火炎放射")]
    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private float flameDuration = 0.8f;
    [SerializeField] private float flameIntervalMin = 0.15f;
    [SerializeField] private float flameIntervalMax = 0.2f;
    [SerializeField] private float aimMinTime = 0.3f;
    [SerializeField] private float aimMaxTime = 0.5f;
    [SerializeField] private float cooldownTime = 1.0f;
    [SerializeField] private float fireSpawnOffset = 1.2f;
    [SerializeField] private float fireDirSpread = 0.15f;
    [SerializeField] private float firePosJitter = 0.15f;

    [Header("スプライト")]
    [SerializeField] private Sprite spriteUp;
    [SerializeField] private Sprite spriteDown;
    [SerializeField] private Sprite spriteLeft;
    [SerializeField] private Sprite spriteRight;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private Transform target;
    private Vector2 fireDir;
    private float stateTimer = 0f;
    private float flameTimer = 0f;
    private float currentAimTime = 0.4f;
    private float currentFlameInterval = 0.18f;
    private int strafeDirection = 1; // +1:右回り, -1:左回り

    private enum AIState
    {
        Idle,
        Chase,
        Strafe,
        Aim,
        Attack,
        Retreat,
        Cooldown
    }

    private AIState currentState = AIState.Idle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        target = GetNearestPlayer();
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case AIState.Idle: UpdateIdle(); break;
            case AIState.Chase: UpdateChase(); break;
            case AIState.Strafe: UpdateStrafe(); break;
            case AIState.Aim: UpdateAim(); break;
            case AIState.Attack: UpdateAttack(); break;
            case AIState.Retreat: UpdateRetreat(); break;
            case AIState.Cooldown: UpdateCooldown(); break;
        }
    }

    // ====== 各ステート ======

    private void UpdateIdle()
    {
        rb.linearVelocity = Vector2.zero;

        if (target == null) return;

        float dist = DistanceToTarget();
        if (dist <= sightRange && HasLineOfSight())
        {
            ChangeState(AIState.Chase);
        }
    }

    private void UpdateChase()
    {
        if (target == null)
        {
            ChangeState(AIState.Idle);
            return;
        }

        float dist = DistanceToTarget();

        if (dist > sightRange)
        {
            ChangeState(AIState.Idle);
            return;
        }

        AimDirection();
        ChangeSprite();

        // 距離管理
        if (dist > attackRange)
        {
            MoveToward(target.position);
        }
        else if (dist < escapeRange)
        {
            ChangeState(AIState.Retreat);
        }
        else
        {
            // ちょうど良い距離 → 回り込み開始
            DecideStrafeDirection();
            ChangeState(AIState.Strafe);
        }
    }

    private void UpdateStrafe()
    {
        if (target == null)
        {
            ChangeState(AIState.Idle);
            return;
        }

        float dist = DistanceToTarget();

        if (dist > sightRange)
        {
            ChangeState(AIState.Idle);
            return;
        }

        AimDirection();
        ChangeSprite();

        // 円を描くように回り込み
        Vector2 toTarget = (target.position - transform.position).normalized;
        Vector2 tangent = new Vector2(-toTarget.y, toTarget.x) * strafeDirection;

        // 距離を strafeRadius 付近に保つ
        if (dist > strafeRadius + 0.5f)
        {
            MoveToward(target.position);
        }
        else if (dist < strafeRadius - 0.5f)
        {
            MoveAway(target.position);
        }

        rb.MovePosition(rb.position + tangent * (moveSpeed * 0.6f) * Time.deltaTime);

        // 一定確率で左右ステップ
        if (dist < attackRange && Random.value < strafeSideStepChance * Time.deltaTime)
        {
            Vector2 side = tangent.normalized * sideStepDistance;
            rb.MovePosition(rb.position + side);
        }

        // 視界があるなら Aim へ
        if (HasLineOfSight())
        {
            ChangeState(AIState.Aim);
        }
    }

    private void UpdateAim()
    {
        if (target == null)
        {
            ChangeState(AIState.Idle);
            return;
        }

        rb.linearVelocity = Vector2.zero;

        AimDirection();
        ChangeSprite();

        if (!HasLineOfSight())
        {
            ChangeState(AIState.Chase);
            return;
        }

        if (stateTimer >= currentAimTime)
        {
            ChangeState(AIState.Attack);
        }
    }

    private void UpdateAttack()
    {
        if (target == null)
        {
            ChangeState(AIState.Idle);
            return;
        }

        AimDirection();
        ChangeSprite();

        float dist = DistanceToTarget();

        // 距離管理
        if (dist > attackRange)
        {
            MoveToward(target.position);
        }
        else if (dist < escapeRange)
        {
            ChangeState(AIState.Retreat);
            return;
        }
        else
        {
            // 横移動しながら火炎放射
            Vector2 toTarget = (target.position - transform.position).normalized;
            Vector2 side = new Vector2(-toTarget.y, toTarget.x);

            // ランダムで左右
            if (Random.value < 0.5f) side = -side;

            rb.MovePosition(rb.position + side * (moveSpeed * 0.5f) * Time.deltaTime);
        }

        if (!HasLineOfSight())
        {
            ChangeState(AIState.Chase);
            return;
        }

        // 火炎放射連続生成
        flameTimer += Time.deltaTime;
        if (flameTimer >= currentFlameInterval)
        {
            flameTimer = 0f;
            currentFlameInterval = Random.Range(flameIntervalMin, flameIntervalMax);
            ShootFlame();
        }

        if (stateTimer >= flameDuration)
        {
            ChangeState(AIState.Cooldown);
        }
    }

    private void UpdateRetreat()
    {
        if (target == null)
        {
            ChangeState(AIState.Idle);
            return;
        }

        AimDirection();
        ChangeSprite();

        float dist = DistanceToTarget();

        MoveAway(target.position);

        if (dist > escapeRange + 1f)
        {
            ChangeState(AIState.Chase);
        }
    }

    private void UpdateCooldown()
    {
        rb.linearVelocity = Vector2.zero;

        if (stateTimer >= cooldownTime)
        {
            if (target != null && DistanceToTarget() <= sightRange)
                ChangeState(AIState.Chase);
            else
                ChangeState(AIState.Idle);
        }
    }

    // ====== 共通処理 ======

    private void ChangeState(AIState newState)
    {
        currentState = newState;
        stateTimer = 0f;
        flameTimer = 0f;

        if (newState == AIState.Aim)
        {
            currentAimTime = Random.Range(aimMinTime, aimMaxTime);
        }

        if (newState == AIState.Strafe)
        {
            DecideStrafeDirection();
        }

        if (newState == AIState.Attack)
        {
            currentFlameInterval = Random.Range(flameIntervalMin, flameIntervalMax);
        }
    }

    private Transform GetNearestPlayer()
    {
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Transform p in players)
        {
            if (p == null) continue;

            float d = Vector2.Distance(transform.position, p.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = p;
            }
        }
        return nearest;
    }

    private float DistanceToTarget()
    {
        if (target == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, target.position);
    }

    private void AimDirection()
    {
        if (target == null) return;

        Vector2 dir = target.position - transform.position;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            fireDir = (dir.x > 0) ? Vector2.right : Vector2.left;
        else
            fireDir = (dir.y > 0) ? Vector2.up : Vector2.down;
    }

    private void ChangeSprite()
    {
        if (fireDir == Vector2.up)
            sr.sprite = spriteUp;
        else if (fireDir == Vector2.down)
            sr.sprite = spriteDown;
        else if (fireDir == Vector2.left)
            sr.sprite = spriteLeft;
        else if (fireDir == Vector2.right)
            sr.sprite = spriteRight;
    }

    private void MoveToward(Vector3 pos)
    {
        Vector2 dir = (pos - transform.position).normalized;

        // 前方に壁があるなら方向変更
        if (IsWallAhead(dir))
        {
            dir = new Vector2(-dir.y, dir.x); // 90度回転して回避
        }

        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }

    private void MoveAway(Vector3 pos)
    {
        Vector2 dir = (transform.position - pos).normalized;

        if (IsWallAhead(dir))
        {
            dir = new Vector2(-dir.y, dir.x);
        }

        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }

    private bool HasLineOfSight()
    {
        if (target == null) return false;

        Vector2 origin = transform.position;
        Vector2 dir = (target.position - transform.position).normalized;
        float dist = DistanceToTarget();

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist, wallLayer);
        return hit.collider == null;
    }

    private bool IsWallAhead(Vector2 dir)
    {
        float checkDist = 0.8f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, checkDist, wallLayer);
        return hit.collider != null;
    }

    private void DecideStrafeDirection()
    {
        // 左右どちらかをランダムに選ぶ
        strafeDirection = (Random.value < 0.5f) ? 1 : -1;
    }

    private void ShootFlame()
    {
        if (fireBallPrefab == null || target == null) return;

        Vector2 baseDir = fireDir;
        Vector2 randomOffset = Random.insideUnitCircle * fireDirSpread;
        Vector2 finalDir = (baseDir + randomOffset).normalized;

        Vector3 spawnPos = transform.position + (Vector3)fireDir * fireSpawnOffset;
        spawnPos += (Vector3)(Random.insideUnitCircle * firePosJitter);

        GameObject fbObj = Instantiate(fireBallPrefab, spawnPos, Quaternion.identity);

        FireBall fb = fbObj.GetComponent<FireBall>();
        if (fb != null)
        {
            fb.Init(finalDir);
            fb.owner = this.gameObject;
        }
    }
}
