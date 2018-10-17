using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

    public Transform WeaponHold;
    public Gun[] Guns;
    Gun EquippedGun;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void EquipGun(int Index)
    {
        EquipGun(Guns[Index]);
    }

    public void EquipGun(Gun ToEquip)
    {
        if(EquippedGun != null)
        {
            Destroy(EquippedGun.gameObject);
        }

        EquippedGun = Instantiate(ToEquip, WeaponHold.position, WeaponHold.rotation) as Gun;
        EquippedGun.transform.parent = WeaponHold.transform;
    }

    public void OnTriggerHold()
    {
        if(EquippedGun != null)
        {
            EquippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (EquippedGun != null)
        {
            EquippedGun.OnTriggerRelease();
        }
    }

    public void Aim(Vector3 AimPoint)
    {
        if (EquippedGun != null)
        {
            EquippedGun.Aim(AimPoint);
        }
    }

    public void Reload()
    {
        if (EquippedGun != null)
        {
            EquippedGun.Reload();
        }
    }

    public float WeaponHeight
    {
        get
        {
            return WeaponHold.position.y;
        }
    }
}
