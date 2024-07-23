using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad : MonoBehaviour
{
    GameControls gameController;

    public static Quad instance;

    public GameObject heatmap;
    public GameObject heatmapLegacy;

    Material mMaterial;
    MeshRenderer mMeshRenderer;

    float[] mPoints;
    int mHitCount;

    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }

        gameController = new GameControls();
        gameController.Game.Heatmap.performed += cntxt => ToggleHeatmap();
    }

    private void OnEnable() {
        gameController.Game.Enable();
    }

    private void OnDisable() {
        gameController.Game.Disable();
    }

    void Start()
    {
        mMeshRenderer = heatmap.GetComponent<MeshRenderer>();
        mMaterial = mMeshRenderer.material;
    
        mPoints = new float[32 * 3]; //32 point 

        heatmap.SetActive(false);
  
    }

    public void ToggleHeatmap()
    {
        heatmap.SetActive(!heatmap.activeSelf);
        UIManager.uimanager.ToggleGameCanvas();
        heatmapLegacy.SetActive(!heatmapLegacy.activeSelf);
    }
  
    public void addHitPoint(float xp,float yp)
    {
        mPoints[mHitCount * 3] = xp;
        mPoints[mHitCount * 3 + 1] = yp;
        mPoints[mHitCount * 3 + 2] = Random.Range(1f, 3f);
  
        mHitCount++;
        mHitCount %= 32;
  
        mMaterial.SetFloatArray("_Hits", mPoints);
        mMaterial.SetInt("_HitCount", mHitCount);
    }
}
