using UnityEngine;

public interface IDamageable
{
    void TakeHit(float Damage, Vector3 HitPoint, Vector3 HitDirection);
    void TakeDamage(float Damage);
}
