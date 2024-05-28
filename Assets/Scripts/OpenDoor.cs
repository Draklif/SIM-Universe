using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject DoorOpen;
    [SerializeField] GameObject DoorClosed;
    private bool isOpen = true;
    public void Interact()
    {
        if (DoorOpen) DoorOpen.SetActive(isOpen);
        if (DoorClosed) DoorClosed.SetActive(!isOpen);

        isOpen = !isOpen;
    }
}
