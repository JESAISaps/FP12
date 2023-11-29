using FishNet.Object;
using FishNet.Component.Animating;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public sealed class PawnWeapon : NetworkBehaviour
{
	private PawnInput _input;

	[SerializeField]
	private WeaponData[] weaponStats;
	// important que l'arme et sont ScriptableObject soient en meme indice
	[SerializeField]
	private GameObject[] weapons;

	private Animator currentWeaponAnimator;
	private NetworkAnimator currentWeaponNetworkAnimator;

	[HideInInspector]
	public int currentWeapon = 1;

	private Transform shootCamera;

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

		SwitchWeapon();
	}

	private void SwitchWeapon()
    {

		ChangeWeaponGraphics(false, weapons[currentWeapon]);
		// permet d'alterner entre l'arme 1, weapon0, et l'arme 2, sniper.
		currentWeapon = 1 - currentWeapon;
		ChangeWeaponGraphics(true, weapons[currentWeapon]);

		if (weapons[currentWeapon].GetComponent<Animator>() != null)
			currentWeaponAnimator = weapons[currentWeapon].GetComponent<Animator>();

		currentWeaponNetworkAnimator = weapons[currentWeapon].GetComponent<NetworkAnimator>();
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
				Shoot(shootCamera.position, shootCamera.TransformDirection(Vector3.forward), weaponStats[currentWeapon].damage);
				Debug.Log("A tiré");
				_timeUntilNextShot = weaponStats[currentWeapon].firerate;
			}
			
			if (_input.changeWeapon)
            {
				SwitchWeapon();
            }
		}
		else
		{
			_timeUntilNextShot -= Time.deltaTime;
		}
	}

	void Shoot(Vector3 pos, Vector3 dir, float damage)
    {
		ServerFire(pos, dir, damage);
		ShootingEffects(currentWeaponNetworkAnimator);
	}

	void ShootingEffects(NetworkAnimator animator)
    {
		animator.SetTrigger("Shoot");
		EndShootingEffect(animator);
    }

	private IEnumerator EndShootingEffect(NetworkAnimator animator)
    {
		yield return new WaitForSeconds(.01f);
		animator.ResetTrigger("Shoot");
    }

	[ServerRpc]
	private void ServerFire(Vector3 firePointPosition, Vector3 firePointDirection, float damage)
	{
		if (Physics.Raycast(firePointPosition, firePointDirection, out RaycastHit hit) && hit.transform.parent.TryGetComponent(out Pawn pawn))
		{
			Debug.Log("A touché un ennemi");
			pawn.ReceiveDamage(damage);
		}
	}

	[ServerRpc]
	private void ChangeWeaponGraphics(bool newState, GameObject toChange)
    {
		toChange.SetActive(newState);
    }

	private void SetWeaponLayerRecursively(GameObject weapon, int layer)
	{
		weapon.layer = layer; // met la layer 3 qui est celle de weapon
		Debug.Log(weapon.transform.name);

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
