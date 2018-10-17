using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
public class Player : LivingEntity {

    public float MovementSpeed = 5;

    public Crosshairs Crosshair;

    Camera MainCamera;
    PlayerController Controller;
    GunController WeaponController;

	protected override void Start ()
    {
        base.Start();

	}

    void Awake()
    {
        Controller = GetComponent<PlayerController>();
        WeaponController = GetComponent<GunController>();
        MainCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }
	
    void OnNewWave(int WaveNumber)
    {
        Health = StartingHealth;
        WeaponController.EquipGun(WaveNumber - 1);
    }

    public override void Die()
    {
        AudioManager.INSTANCE.PlaySound("PlayerDeath", transform.position);
        base.Die();
    }

    void Update ()
    {
        Vector3 MovementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 MovementVelocity = MovementInput.normalized * MovementSpeed;
        Controller.Move(MovementVelocity);

        Ray CursorRay = MainCamera.ScreenPointToRay(Input.mousePosition);
        Plane GroundPlane = new Plane(Vector3.up, Vector3.up * WeaponController.WeaponHeight);
        float RayDistance;
        if(GroundPlane.Raycast(CursorRay, out RayDistance))
        {
            Vector3 Intersection = CursorRay.GetPoint(RayDistance);
            // Debug.DrawLine(CursorRay.origin, Intersection, Color.black);
            Controller.LookAt(Intersection);
            Crosshair.transform.position = Intersection;
            Crosshair.DetectTargets(CursorRay);
        }

        if(Input.GetMouseButton(0))
        {
            WeaponController.OnTriggerHold();
        }
        if(Input.GetMouseButtonUp(0))
        {
            WeaponController.OnTriggerRelease();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            WeaponController.Reload();
        }
	}
}
