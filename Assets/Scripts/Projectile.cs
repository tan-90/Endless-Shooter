using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public LayerMask CollisionMask;
    float Speed = 10;
    float Damage = 1;

    float LifeTime = 3;
    float SkinWidth = .1f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, LifeTime);
        Collider[] Collisions = Physics.OverlapSphere(transform.position, .1f, CollisionMask);
        if(Collisions.Length > 0)
        {
            OnHitObject(Collisions[0], transform.position);
        }

	}
	
	// Update is called once per frame
	void Update () {
        float MoveDistance = Speed * Time.deltaTime;
        CheckCollisions(MoveDistance);
        transform.Translate(Vector3.forward * MoveDistance);
	}

    public void SetSpeed(float Speed)
    {
        this.Speed = Speed;
    }

    void CheckCollisions(float MoveDistance)
    {
        Ray BulletRay = new Ray(transform.position, transform.forward);
        RaycastHit Hit;

        if(Physics.Raycast(BulletRay, out Hit, MoveDistance + SkinWidth, CollisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(Hit.collider, Hit.point);
        }
    }

    void OnHitObject(Collider Object, Vector3 HitPoint)
    {
        IDamageable Damageable = Object.GetComponent<IDamageable>();
        if (Damageable != null)
        {
            Damageable.TakeHit(Damage, HitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
