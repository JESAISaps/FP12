using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public sealed class MainView : View
{
	[SerializeField]
	private TextMeshProUGUI healthText;

	[SerializeField]
	private GameObject pauseMenu;

	[SerializeField]
	private GameObject gamingMenu;

	[SerializeField]
	private Slider sensitivitySlider;

	private void Update()
	{
		if (!Initialized)
		{
			return;
		}

		Player player = Player.Instance;

		if (player == null || player.controlledPawn == null) return;

		healthText.text = $"Health {player.controlledPawn.health}";

		if (Input.GetKeyDown(KeyCode.Escape))
        {
			TogglePauseMenu();
        }
	}

	void TogglePauseMenu()
    {
		pauseMenu.SetActive(!pauseMenu.activeSelf);
		gamingMenu.SetActive(!gamingMenu.activeSelf);

		// si lockstate == 1, 1 -1 = 0, sinon 1-0 = 1
		Cursor.lockState = 1 - CursorLockMode.Locked;
		Cursor.visible = !Cursor.visible;
		// ces actions se passent au lancement de la partie dans Player.TargetPawnSpawned()
	}

	public void QuitApp()
    {
		Application.Quit();
    }

	public void ChangeSensitivity(float sensitivity)
    {
		Player player = Player.Instance;
		PawnInput pawnInput = player.controlledPawn.GetComponent<PawnInput>();

		pawnInput.sensitivity = sensitivity;
		sensitivitySlider.value = pawnInput.sensitivity;
	}
}