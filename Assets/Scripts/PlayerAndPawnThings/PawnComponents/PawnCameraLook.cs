using FishNet.Object;
using UnityEngine;

public sealed class PawnCameraLook : NetworkBehaviour
{
	private PawnInput _input;

	[SerializeField]
	private Transform myCamera;
	private Camera myCameraCamera;
	private Pawn pawnScript;

	[SerializeField]
	private float xMin;

	[SerializeField]
	private float xMax;

	[SerializeField]
	private int defaultFOV;
	[SerializeField]
	private int sniperFOV;

	private Vector3 _eulerAngles;

	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		_input = GetComponent<PawnInput>();
	}

	public override void OnStartClient()
	{
		base.OnStartClient();

		myCamera.GetComponent<Camera>().enabled = IsOwner;

		myCamera.GetComponent<AudioListener>().enabled = IsOwner;

        if (IsOwner)
        {
			myCameraCamera = myCamera.GetComponent<Camera>();
			pawnScript = GetComponent<Pawn>();
        }
	}

	private void Update()
	{
		if (!IsOwner)
		{
			return;
		}

		_eulerAngles.x -= _input.mouseY;

		_eulerAngles.x = Mathf.Clamp(_eulerAngles.x, xMin, xMax);

		myCamera.localEulerAngles = _eulerAngles;

		transform.Rotate(0.0f, _input.mouseX, 0.0f, Space.World);

		if (pawnScript.weaponScript.currentWeapon == 1)
		{
			if (Input.GetKeyDown(KeyCode.Mouse1))
			{
				myCameraCamera.fieldOfView = sniperFOV;
			}
			if (Input.GetKeyUp(KeyCode.Mouse1))
			{
				myCameraCamera.fieldOfView = defaultFOV;
			}
		}
	}
}
