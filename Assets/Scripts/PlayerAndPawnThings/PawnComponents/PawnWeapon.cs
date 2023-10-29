using FishNet.Object;
using UnityEngine;

public sealed class PawnWeapon : NetworkBehaviour
{
	private PawnInput _input;

	[SerializeField]
	private WeaponData[] weaponStats;
	// important que l'arme et sont ScriptableObject soient en meme indice
	[SerializeField]
	private GameObject[] weapons;

	[HideInInspector]
	public WeaponData currentWeaponData;

	[SerializeField]
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
		currentWeaponData = weaponStats[0];
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
				ServerFire(shootCamera.position, shootCamera.TransformDirection(Vector3.forward));
				Debug.Log("A tiré");
				_timeUntilNextShot = currentWeaponData.firerate;
			}
		}
		else
		{
			_timeUntilNextShot -= Time.deltaTime;
		}
	}

	[ServerRpc]
	private void ServerFire(Vector3 firePointPosition, Vector3 firePointDirection)
	{
		if (Physics.Raycast(firePointPosition, firePointDirection, out RaycastHit hit) && hit.transform.parent.TryGetComponent(out Pawn pawn))
		{
			Debug.Log("A touché un ennemi");
			pawn.ReceiveDamage(currentWeaponData.damage);
		}
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
