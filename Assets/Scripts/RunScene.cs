using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunScene : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene("Planets", LoadSceneMode.Single);
    }
}