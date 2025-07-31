using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemaphoreSystem : MonoBehaviour
{
    [Header("Semaphore Settings")]
    public int number;
    public Transform keyCrystalPoint;
    public GameObject keyCrystalPrefab;

    [Header("Particle Settings")]
    public ParticleSystem particleSystem1;
    public ParticleSystem particleSystem2;

    [Header("Line Setting")]
    public Material beamMaterial;
    public LineRenderer lineRenderer;

    public bool isUnlocked = false;
    private GameManager gameManager;
    private void Awake()
    {
        lineRenderer.material = beamMaterial; // 빛나는 머티리얼

        // 위치 설정
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector3.zero);      // 오브젝트 중심
        lineRenderer.SetPosition(1, Vector3.up * 50f);  // 위로 50m
        
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void OnEnable()
    {
        if (number >= GameState.currentSemaphoreNumber)
        {
            particleSystem1.Stop();
            particleSystem2.Stop();
        }
        else
        {
            SpawnKeyCrystal();
        }

        if (number >= 4 && GameState.currentSemaphoreNumber >= 4)
        {
            GameState.IsOperate = true;
            gameManager.SetOperateSemaphore();
        }
    }
    public void SpawnKeyCrystal()
    {
        if (keyCrystalPrefab && keyCrystalPoint)
        {
            GameObject keyCrystal = Instantiate(keyCrystalPrefab, keyCrystalPoint.position, keyCrystalPoint.rotation);
            keyCrystal.transform.SetParent(keyCrystalPoint);
            keyCrystal.transform.localPosition = Vector3.zero;
            keyCrystal.transform.localRotation = Quaternion.identity;

            Invoke("ShowPartical", 2f);   
        }
    }

    void ShowPartical()
    {
        particleSystem1.Play();
        particleSystem2.Play();

        if (number >= 4)
        {
            GameState.IsOperate = true;
            gameManager.SetOperateSemaphore();
        }
    }
}
