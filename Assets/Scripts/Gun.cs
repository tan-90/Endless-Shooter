using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public enum FireMode
    {
        Auto, Burst, Single
    };

    [Header("Base settings")]
    public FireMode GunFireMode;
    public Transform[] ProjectileSpawn;
    public Projectile ProjectileType;
    public float MsBetweenShots = 100;
    public float MuzzleVelocity = 35;
    public int BurstShotCount;
    public int MagazineSize;
    public float ReloadTime = .3f;
    [Header("Recoil")]
    public Vector2 RecoilMinMax = new Vector2(0.01f, 0.02f);
    public float RecoilRecoverTime = .1f;
    [Header("Effects")]
    MuzzleFlash Flash;
    public Transform Shell;
    public Transform ShellEjector;

    public AudioClip ShootAudio;
    public AudioClip ReloadAudio;

    float NextShotTime;

    bool TriggerReleasedSinceLastShot;
    int RemainingBurstFireCount;
    int RemainingProjectiles;
    bool IsReloading;

    Vector3 RecoilVelocity;
	// Use this for initialization
	void Start ()
    {
        Flash = GetComponent<MuzzleFlash>();
        RemainingBurstFireCount = BurstShotCount;
        RemainingProjectiles = MagazineSize;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref RecoilVelocity, RecoilRecoverTime);
        if(!IsReloading && RemainingProjectiles == 0)
        {
            Reload();
        }
    }

    void Shoot()
    {
        if(!IsReloading && Time.time > NextShotTime && RemainingProjectiles > 0)
        {
            if(GunFireMode == FireMode.Burst)
            {
                if(RemainingBurstFireCount == 0)
                {
                    return;
                }
                RemainingBurstFireCount--;
            }
            else if(GunFireMode == FireMode.Single)
            {
                if(!TriggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            for(int i = 0; i < ProjectileSpawn.Length; i++)
            {
                if(RemainingProjectiles == 0)
                {
                    break;
                }
                NextShotTime = Time.time + MsBetweenShots / 1000;
                Projectile NewProjectile = Instantiate(ProjectileType, ProjectileSpawn[i].position, ProjectileSpawn[i].rotation) as Projectile;
                NewProjectile.SetSpeed(MuzzleVelocity);
                RemainingProjectiles--;
            }
            Instantiate(Shell, ShellEjector.position, ShellEjector.rotation);
            Flash.Activate();
            AudioManager.INSTANCE.PlaySound(ShootAudio, transform.position);
        }
        transform.localPosition -= Vector3.right * Random.Range(RecoilMinMax.x, RecoilMinMax.y);
    }

    public void Reload()
    {
        if(!IsReloading && RemainingProjectiles != MagazineSize)
        {
            StartCoroutine(AnimateReload());
        }
        AudioManager.INSTANCE.PlaySound(ReloadAudio, transform.position);

    }

    IEnumerator AnimateReload()
    {
        IsReloading = true;
        yield return new WaitForSeconds(.2f);
        float Percentage = 0;
        float ReloadSpeed = 1 / ReloadTime;
        Vector3 InitialRotation = transform.localEulerAngles;
        float MaxReloadAngle = 30;
        
        while(Percentage < 1)
        {
            Percentage += ReloadSpeed * Time.deltaTime;
            float Interpolation = 4 * (-Mathf.Pow(Percentage, 2) + Percentage);
            float ReloadAngle = Mathf.Lerp(0, MaxReloadAngle, Interpolation);
            transform.localEulerAngles = InitialRotation + Vector3.forward * ReloadAngle;
            yield return null;
        }
        RemainingProjectiles = MagazineSize;
        IsReloading = false;
    }

    public void OnTriggerHold()
    {
        Shoot();
        TriggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        TriggerReleasedSinceLastShot = true;
        RemainingBurstFireCount = BurstShotCount;
    }

    public void Aim(Vector3 AimPoint)
    {
        transform.LookAt(AimPoint);
    }
}
