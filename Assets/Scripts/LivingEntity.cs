using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

    public float StartingHealth;
    public float Health;
    protected bool Dead;

    public event System.Action OnDeath;

	// Use this for initialization
	protected virtual void Start () {
        Health = StartingHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void TakeHit(float Damage, Vector3 HitPoint, Vector3 HitDirection)
    {
        TakeDamage(Damage);
    }

    public virtual void TakeDamage(float Damage)
    {
        Health -= Damage;
        if (Health <= 0 && !Dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    public virtual void Die()
    {
        Dead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
}
