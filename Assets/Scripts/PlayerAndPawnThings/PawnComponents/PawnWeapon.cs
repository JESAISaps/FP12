using FishNet.Object;
using FishNet.Component.Animating;
using UnityEngine;
using System.Collections;

public sealed class PawnWeapon : NetworkBehaviour
{
	private PawnInput _input;

	[SerializeField]
	private WeaponData[] weaponStats;
	// important que l'arme et sont ScriptableObject soient en meme indice
	public GameObject[] weapons;

	private Animator currentWeaponAnimator;
	private NetworkAnimator currentWeaponNetworkAnimator;

	[HideInInspector]
	public int currentWeapon = 0;
	[SerializeField]
	private int defaultWeapon;

	private Transform shootCamera;
	private Transform shootPoint;
	private LineRenderer laserLine;
	[SerializeField]
	private float laserLifeTime;

	[SerializeField]
	private LayerMask ignoreOnShootRaycast;

	private ParticleSystem effect;

	private float _timeUntilNextShot;

	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		// on remonte de deux etages car ya d'abord la camera puis le jeureur
		_input = transform.parent.parent.GetComponent<PawnInput>();

		shootCamera = transform.parent.GetComponent<Camera>().transform;

		if (base.Owner.IsLocalClient)
		{
			foreach (GameObject weapon in weapons)
			{
				SetWeaponLayerRecursively(weapon, 3);
			}
		}

	}

    private void Start()
    {
		SetupWeapon();        
    }

    void SetupWeapon()
	{
		weapons[defaultWeapon].SetActive(true);
		weapons[1 - defaultWeapon].SetActive(false);

		currentWeaponNetworkAnimator = weapons[defaultWeapon].GetComponent<NetworkAnimator>();
		currentWeaponAnimator = weapons[defaultWeapon].GetComponent<Animator>();
		shootPoint = weapons[defaultWeapon].GetComponentInChildren<ShootPoint>().transform;
		effect = weapons[defaultWeapon].GetComponentInChildren<ParticleSystem>();
		laserLine = weapons[defaultWeapon].GetComponentInChildren<LineRenderer>();
	}

    private void SwitchWeapon()
    {

		ChangeWeaponGraphics(this, false, currentWeapon);
		currentWeapon = 1 - currentWeapon;
		ChangeWeaponGraphics(this, true, currentWeapon);

		currentWeaponNetworkAnimator = weapons[currentWeapon].GetComponent<NetworkAnimator>();
		currentWeaponAnimator = weapons[currentWeapon].GetComponent<Animator>();
		shootPoint = weapons[currentWeapon].GetComponentInChildren<ShootPoint>().transform;
		effect = weapons[currentWeapon].GetComponentInChildren<ParticleSystem>();
		laserLine = weapons[currentWeapon].GetComponentInChildren<LineRenderer>();
	}

	private void Update()
	{
		if (!IsOwner)
		{
			return;
		}

		if (_timeUntilNextShot <= 0.0f)
		{
			if (_input.fire)
			{
				Shoot(shootCamera.position, shootCamera.TransformDirection(Vector3.forward), weaponStats[currentWeapon].damage, weaponStats[currentWeapon].range);
				Debug.Log("A tiré");
				_timeUntilNextShot = weaponStats[currentWeapon].firerate;
			}
			
			
		}
		else
		{
			_timeUntilNextShot -= Time.deltaTime;
		}
		
		if (_input.changeWeapon)
        {
			SwitchWeapon();
        }
	}

	void Shoot(Vector3 pos, Vector3 dir, float damage, float range)
    {
		ServerFire(pos, dir, damage, range);
		ShootingEffects(currentWeaponNetworkAnimator);
	}

	void ShootingEffects(NetworkAnimator animator)
    {
		animator.SetTrigger("Shoot");
		//EndShootingEffect(animator);
		DoParticleEffect();

    }

	[ServerRpc]
	public void DoParticleEffect()
    {
		DoParticleEffectObserver();
    }

	[ObserversRpc]
	void DoParticleEffectObserver()
    {
		effect.Play();
    }

	// inutile, le trigger se desactive automatiquement
	/*
	private IEnumerator EndShootingEffect(NetworkAnimator animator)
    {
		yield return new WaitForSeconds(.01f);
		animator.ResetTrigger("Shoot");
    }*/

	[ServerRpc]
	private void ServerFire(Vector3 firePointPosition, Vector3 firePointDirection, float damage, float range)
	{
		if (Physics.Raycast(firePointPosition, firePointDirection, out RaycastHit hit, range, ~ignoreOnShootRaycast))
		{
			if (hit.transform.parent.TryGetComponent(out Pawn pawn))
			{
				Debug.Log("A touché un ennemi");
				pawn.ReceiveDamage(damage);
				DoTrailEffect(shootPoint.position, hit.point, weaponStats[currentWeapon].laserLifeTime);

			}
			else
            {
				DoTrailEffect(shootPoint.position, hit.point, weaponStats[currentWeapon].laserLifeTime);
			}
		}
		else
		{
			DoTrailEffect(shootPoint.position, shootPoint.position + (shootPoint.forward * range), weaponStats[currentWeapon].laserLifeTime);
		}
	}

	[ServerRpc]
	void DoTrailEffect(Vector3 start, Vector3 end, float timeToWait)
    {
		ObserverDoTrailEffect(start, end, timeToWait);
    }

	[ObserversRpc]
	void ObserverDoTrailEffect(Vector3 start, Vector3 end, float timeToWait)
	{
		laserLine.SetPosition(0, start);
		laserLine.SetPosition(1, end);
		StartCoroutine(ShootLaser(timeToWait));
	}

	IEnumerator ShootLaser(float timeToWait)
    {
		laserLine.enabled = true;
		yield return new WaitForSeconds(timeToWait);
		laserLine.enabled = false;
    }

	[ServerRpc]
	private void ChangeWeaponGraphics(PawnWeapon script, bool newState, int currentWeapon)
    {
		ChangeWeaponGraphicsObserver(script, newState, currentWeapon);

	}

	[ObserversRpc]
	private void ChangeWeaponGraphicsObserver(PawnWeapon script, bool newState, int currentWeapon)
    {
		Debug.Log(script.weapons[currentWeapon].name + " mis a " + newState.ToString());
		// on desactive l'arme actuelle
		script.weapons[currentWeapon].SetActive(newState);
	}


	private void SetWeaponLayerRecursively(GameObject weapon, int layer)
	{
		weapon.layer = layer; // met la layer 3 qui est celle de weapon

		// applique la layer de maniere recursive a tous les enfants
		foreach (Transform child in weapon.transform)
		{
			child.gameObject.layer = layer;

			if (child.GetComponentInChildren<Transform>())
			{
				SetWeaponLayerRecursively(child.gameObject, layer);
			}
		}
	}
}
