using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartSimulation : MonoBehaviour, IInteractable
{
    [SerializeField] Chamber Chamber;
    [SerializeField] TMP_Text Text;
    [SerializeField] string textStringPre;
    [SerializeField] string textStringPost;
    [SerializeField] GameObject DoorOpen;

    public bool isToggle = false;
    bool isOn = false;
    private void Start()
    {
        Text.text = textStringPre;
    }

    public void Interact()
    {
        if (Chamber != null)
        {
            Chamber.Initialize();
        } 
        else
        {
            DoorOpen.SetActive(isOn);
        }
        if (isToggle)
        {
            isOn = !isOn;
            Text.text = isOn ? textStringPost : textStringPre;
            return;
        }
        Text.text = textStringPost;
    }
}
