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
	[SerializeField]
	private ChangeTarget changeHandTarget;

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

	private Animator hitmarkerAnimator;

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

			hitmarkerAnimator = GameObject.FindGameObjectWithTag("MainHUD").GetComponent<MainHUD>().hitmarkerController;
		}

	}

	void Start()
    {
		// dans la start car si on la met dans OnStartNetwork alors les setactive du sniper ne marchent pas
		// faut pas oublier d'activer le snipe par defaut, sinon bug qui casse la tete
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
		changeHandTarget.ChangeHandleTarget(defaultWeapon);
	}

    private void SwitchWeapon()
    {
		// change en premier l'arme de maniere locale, et ensuite sur le serv, pour aller plus vite cot� joueur

		// variable temporaire pour eviter de trop modifier currentweapon
		int tempWeapon = currentWeapon;
		// local
		ChangeWeaponGraphicsLocal(this, false, tempWeapon);
		tempWeapon = 1 - tempWeapon;
		// local
		ChangeWeaponGraphicsLocal(this, true, tempWeapon);

		//serveur
		ChangeWeaponGraphics(this, false, currentWeapon);
		currentWeapon = 1 - currentWeapon;
		//serveur
		ChangeWeaponGraphics(this, true, currentWeapon);

		currentWeaponNetworkAnimator = weapons[currentWeapon].GetComponent<NetworkAnimator>();
		currentWeaponAnimator = weapons[currentWeapon].GetComponent<Animator>();
		shootPoint = weapons[currentWeapon].GetComponentInChildren<ShootPoint>().transform;
		effect = weapons[currentWeapon].GetComponentInChildren<ParticleSystem>();
		laserLine = weapons[currentWeapon].GetComponentInChildren<LineRenderer>();
		changeHandTarget.ChangeHandleTarget(currentWeapon);
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
				bool didHit = Shoot(shootCamera.position, shootCamera.TransformDirection(Vector3.forward), weaponStats[currentWeapon].damage, weaponStats[currentWeapon].range);
				if (didHit)
					DoHitmarkerEffect(hitmarkerAnimator);
				Debug.Log("A tir�");
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
		/*
        if (Input.GetKeyDown(KeyCode.K))
        {
			hitmarkerAnimator.SetTrigger("Hit");
        }*/
	}

	bool Shoot(Vector3 firePointPosition, Vector3 firePointDirection, float damage, float range)
    {
		ShootingEffects(currentWeaponNetworkAnimator);

		if (Physics.Raycast(firePointPosition, firePointDirection, out RaycastHit hit, range, ~ignoreOnShootRaycast))
		{
			if (hit.transform.parent.TryGetComponent(out Pawn pawn))
			{
				Debug.Log("A touch� un ennemi");
				ServerDealDamage(pawn, damage);
				DoHitmarkerEffect(hitmarkerAnimator);
				return true;
			}

			DoTrailEffect(shootPoint.position, hit.point, weaponStats[currentWeapon].laserLifeTime, this);
			return false;
		}
		else
		{
			DoTrailEffect(shootPoint.position, shootPoint.position + (shootPoint.forward * range), weaponStats[currentWeapon].laserLifeTime, this);
			return false;
		}
	}

	[ServerRpc]
	void ServerDealDamage(Pawn pawn, float damage)
    {
		pawn.ReceiveDamage(damage);
	}

	void DoTrailEffect(Vector3 start, Vector3 end, float timeToWait, PawnWeapon script)
    {
		if (base.IsOwner)
        {
			laserLine.SetPosition(0, start);
			laserLine.SetPosition(1, end);
			StartCoroutine(ShootLaser(timeToWait));
		}
		
		ServerDoTrailEffect(start, end, timeToWait, script);

    }

	[ServerRpc]
	void ServerDoTrailEffect(Vector3 start, Vector3 end, float timeToWait, PawnWeapon script)
    {
		ObserverDoTrailEffect(start, end, timeToWait, script);
    }

	[ObserversRpc]
	void ObserverDoTrailEffect(Vector3 start, Vector3 end, float timeToWait, PawnWeapon script)
	{
		// on ignore si on est le joureur qui a tir�, car on l'a fait en local
		if(this == script) // pour le faire on compare le script qui appelle et celui qui recoit
        {
			return;
        }

		script.laserLine.SetPosition(0, start);
		script.laserLine.SetPosition(1, end);
		script.StartCoroutine(ShootLaser(timeToWait));
	}

	void ShootingEffects(NetworkAnimator animator)
    {
		animator.SetTrigger("Shoot");
		//EndShootingEffect(animator);
		if (IsOwner)
			effect.Play();
		else
			DoParticleEffect();

    }

	[ServerRpc(RequireOwnership = false)]
	public void DoParticleEffect()
    {
		DoParticleEffectObserver();
    }

	[ObserversRpc]
	void DoParticleEffectObserver()
    {
		effect.Play();
    }
	/*
	[ServerRpc(RequireOwnership = false)]
	private bool ServerFire(Vector3 firePointPosition, Vector3 firePointDirection, float damage, float range)
	{
		
	}*/

	void DoHitmarkerEffect(Animator animator)
    {
		animator.SetTrigger("Hit");
	}
	

	IEnumerator ShootLaser(float timeToWait)
    {
		laserLine.enabled = true;
		yield return new WaitForSeconds(timeToWait);
		laserLine.enabled = false;
    }

	[ServerRpc(RequireOwnership=false)]
	private void ChangeWeaponGraphics(PawnWeapon script, bool newState, int currentWeapon)
    {
		ChangeWeaponGraphicsObserver(script, newState, currentWeapon);

	}

	private void ChangeWeaponGraphicsLocal(PawnWeapon script, bool newState, int currentWeapon)
	{
		script.weapons[currentWeapon].SetActive(newState);
	}

	[ObserversRpc]
	private void ChangeWeaponGraphicsObserver(PawnWeapon script, bool newState, int currentWeapon)
    {
		Debug.Log(script.weapons[currentWeapon].name + " mis a " + newState.ToString());
		// on desactive l'arme actuelle
		if (!base.IsOwner)
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
