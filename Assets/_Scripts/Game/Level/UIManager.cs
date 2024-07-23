using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager uimanager;

    public Canvas gameCanvas;
    public GameObject deathPanel;

    int numSiblings;

    //Game contols
    GameControls controls;

    private void Awake() {

        if(uimanager == null)
        {
            uimanager = this;
        }
        
        controls = new GameControls();

        //Input Actions
        controls.Game.Restart.performed += cntxt => RestartGame();
    }

    public void ToggleGameCanvas()
    {
        gameCanvas.enabled = !gameCanvas.enabled;
    }

    public void ToggleDeathPanel()
    {
        //Setting index of Death Panel as the last sibling to show the panel in fron of all elements
        numSiblings = deathPanel.transform.parent.transform.childCount;
        deathPanel.transform.SetSiblingIndex(numSiblings - 1);

        //Showing Death Panel
        deathPanel.SetActive(!deathPanel.activeSelf);
    }

    private void OnEnable() {
        controls.Game.Enable();
    }

    private void OnDisable() {
        controls.Game.Disable();
    }

    public void RestartGame()
    {
        if(deathPanel.activeSelf)
        {
            PlayerManager.instance.KillPlayer();
        }   
    }
}
