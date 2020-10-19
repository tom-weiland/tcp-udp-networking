using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public GameObject pauseMenu;

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

    public void ConnectToMap()
    {
        SceneManager.LoadScene("Map1");
        ConnectToServer();
    }

    /// <summary>Attempts to connect to the server.</summary>
    public void ConnectToServer()
    {
        SceneManager.LoadScene("Map1");
        //startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
    }


    public void TogglePauseMenu()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            pauseMenu.SetActive(false);
        }
        else
        {
            pauseMenu.SetActive(true);
        }
    }
}
