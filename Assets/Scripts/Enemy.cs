using UnityEngine;
using System.Collections;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State
    {
        Idle, Chasing, Attacking
    };

    State CurrentState;

    public ParticleSystem DeathEffect;

    NavMeshAgent PathFinder;
    Transform Target;

    Material EnemyMaterial;
    Color OriginalColor;

    LivingEntity TargetEntity;

    float AttackDistance = .5f;
    float TimeBetweenAttacks = 1;

    float NextAttackTime;
    float CollisionRadius;
    float TargetCollisionRadius;

    float Damage = 1;

    bool HasTarget;

    void Awake()
    {
        PathFinder = GetComponent<NavMeshAgent>();
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Target = GameObject.FindGameObjectWithTag("Player").transform;
            TargetEntity = Target.GetComponent<LivingEntity>();

            CollisionRadius = GetComponent<CapsuleCollider>().radius;
            TargetCollisionRadius = Target.GetComponent<CapsuleCollider>().radius;

            HasTarget = true;
        }
    }

	// Use this for initialization
	protected override void Start () {
        base.Start();

        if(HasTarget)
        {
            TargetEntity.OnDeath += OnTargetDeath;
            
            CurrentState = State.Chasing;
            StartCoroutine(UpdatePath());
            
        }
	}

    public void SetProperties(float MoveSpeed, int HitsToKillPlayer, float Health, Color SkinColour)
    {
        PathFinder.speed = MoveSpeed;
        if (HasTarget)
        {
            Damage = Mathf.Ceil(TargetEntity.StartingHealth / HitsToKillPlayer);
        }
        StartingHealth = Health;

        EnemyMaterial = GetComponent<Renderer>().sharedMaterial;
        EnemyMaterial.color = SkinColour;
        EnemyMaterial = GetComponent<Renderer>().material;
        EnemyMaterial.color = SkinColour;
        OriginalColor = EnemyMaterial.color;
    }

    void OnTargetDeath()
    {
        HasTarget = false;
        CurrentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (HasTarget)
        {
            if (Time.time > NextAttackTime)
            {
                float DistanceSquared = (Target.position - transform.position).sqrMagnitude;

                if (DistanceSquared < Mathf.Pow(AttackDistance + CollisionRadius + TargetCollisionRadius, 2))
                {
                    NextAttackTime = Time.time + TimeBetweenAttacks;
                    AudioManager.INSTANCE.PlaySound("EnemyAttack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
    }

    public override void TakeHit(float Damage, Vector3 HitPoint, Vector3 HitDirection)
    {
        AudioManager.INSTANCE.PlaySound("Impact", transform.position);
        if(Damage >= Health)
        {
            AudioManager.INSTANCE.PlaySound("EnemyDeath", transform.position);

            Destroy(Instantiate(DeathEffect.gameObject, HitPoint, Quaternion.FromToRotation(Vector3.forward, HitDirection)) as GameObject, DeathEffect.startLifetime);
        }
        base.TakeHit(Damage, HitPoint, HitDirection);
    }

    IEnumerator Attack()
    {
        CurrentState = State.Attacking;
        PathFinder.enabled = false;
        Vector3 StartPosition = transform.position;
        Vector3 DirectionToTarget = (Target.position - transform.position).normalized;
        Vector3 AttackPosition = Target.position - DirectionToTarget * (CollisionRadius);

        float MovePercentage = 0;
        float AttackSpeed = 3;

        EnemyMaterial.color = Color.red;

        bool DamageApplied = false;
        while(MovePercentage <= 1)
        {

            if(MovePercentage >= .5f && !DamageApplied)
            {
                DamageApplied = true;
                TargetEntity.TakeDamage(Damage);
            }

            MovePercentage += Time.deltaTime * AttackSpeed;
            float Interpolation = 4 * (-Mathf.Pow(MovePercentage, 2) + MovePercentage);
            transform.position = Vector3.Lerp(StartPosition, AttackPosition, Interpolation);

            yield return null;
        }

        EnemyMaterial.color = OriginalColor;
        CurrentState = State.Chasing;
        PathFinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float RefreshRate = 0.25f;
        while(HasTarget)
        {
            if(CurrentState == State.Chasing)
            {
                Vector3 DirectionToTarget = (Target.position - transform.position).normalized;
                Vector3 TargetPosition = Target.position - DirectionToTarget * (CollisionRadius + TargetCollisionRadius + AttackDistance / 2);
                if (!Dead)
                {
                    PathFinder.SetDestination(TargetPosition);
                }
            }
            yield return new WaitForSeconds(RefreshRate);
        }
    }
}
