using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 MovementVelocity;
    Rigidbody PlayerRigidBody;

	void Start ()
    {
        PlayerRigidBody = GetComponent<Rigidbody>();
	}
	
	void Update ()
    {
	
	}

    void FixedUpdate()
    {
        PlayerRigidBody.MovePosition(PlayerRigidBody.position + MovementVelocity * Time.fixedDeltaTime);
    }

    public void Move(Vector3 MovementVelocity)
    {
        this.MovementVelocity = MovementVelocity;
    }

    public void LookAt(Vector3 LookAtPoint)
    {
        Vector3 CorrectedLookAtPoint = new Vector3(LookAtPoint.x, transform.position.y, LookAtPoint.z);
        transform.LookAt(CorrectedLookAtPoint);
    }
}
