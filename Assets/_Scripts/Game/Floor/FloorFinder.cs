using System.Runtime.CompilerServices;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTKExtensions.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class FloorFinder : MonoBehaviour
{
    AnalyticPCControls controls;

    private float _delayMoment;
        
    private Vector3? foundPosition = null;

    [SerializeField]
    [Tooltip("Maximum distance to look for the floor")]
    private float maxDistance = 3.0f;

    [SerializeField]
    [Tooltip("Prompt to ask the user if this is indeed the floor")]
    private GameObject confirmPrompt;
    
    [SerializeField]
    [Tooltip("Triggered once when the location is accepted.")]
    private UnityEvent<Vector3> locationFound = new UnityEvent<Vector3>();

    private void Awake() {
        controls = new AnalyticPCControls();

        controls.Analytic.Accept.performed += cntxt => Accept();
        controls.Analytic.Reset.performed += cntxt => Reset();
    }

    private void OnEnable()
    {
        Reset();
        controls.Analytic.Enable();
    }

    private void OnDisable() {
        controls.Analytic.Disable();
    }

    private void Update()
    {
        CheckLocationOnSpatialMap();
    }

    public void Reset()
    {
        _delayMoment = Time.time + 2;
        foundPosition = null;
        confirmPrompt.SetActive(false);
    }

    public void Accept()
    {
        if (foundPosition != null)
        {
            locationFound?.Invoke(foundPosition.Value);
            confirmPrompt.SetActive(false);
        }
    }
    
    private void CheckLocationOnSpatialMap()
    {
        if (foundPosition == null && Time.time > _delayMoment)
        {
            foundPosition = LookingDirectionHelpers.GetPositionOnSpatialMap(maxDistance);
            if (foundPosition != null)
            {
                if (CameraCache.Main.transform.position.y - foundPosition.Value.y > 1f)
                { 
                    confirmPrompt.transform.position = foundPosition.Value;
                    confirmPrompt.SetActive(true);
                }
                else
                {
                    foundPosition = null;
                }
            }
        }
    }
}
