using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScreen : MonoBehaviour
{
    [SerializeField]
    private Canvas settingsCanvas;
    [SerializeField]
    private Canvas lobbyCanvas;
    [SerializeField]
    private Canvas hathoraSDKCanvas;

    private void Start()
    {
        settingsCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(true);
        hathoraSDKCanvas.gameObject.SetActive(true);
    }

    public void ToggleSettingsScreen()
    {
        settingsCanvas.gameObject.SetActive(!settingsCanvas.gameObject.activeSelf);
        lobbyCanvas.gameObject.SetActive(!lobbyCanvas.gameObject.activeSelf);
        hathoraSDKCanvas.gameObject.SetActive(!hathoraSDKCanvas.gameObject.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
