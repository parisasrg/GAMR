using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PCPlayerController controller;
    public static Action shootInput;
    public static Action reloadInput;

    private void Awake() {
        controller = new PCPlayerController();

        controller.Player.Shoot.performed += cntxt => shootInput?.Invoke();
        controller.Player.Reload.performed += cntxt => reloadInput?.Invoke();
    }

    private void OnEnable() {
        controller.Player.Enable();
    }

    private void OnDisable() {
        controller.Player.Disable();
    }
}
