using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject crosshair;

    public InputField usernameField;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }


    /// <summary>Attempts to connect to the server.</summary>
    public void ConnectToServer()
    {
        Client.instance.ConnectToServer();
        startMenu.SetActive(false);
        crosshair.SetActive(true);
        usernameField.interactable = false;
       
    }



    public void TogglePauseMenu()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            pauseMenu.SetActive(false);
            crosshair.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(true);
            crosshair.SetActive(false);
        }
    }
}
