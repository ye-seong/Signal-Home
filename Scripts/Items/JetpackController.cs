using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackController : MonoBehaviour
{
    [Header("Jetpack Settings")]
    public float jetpackForce = 15f;        // 제트팩 추진력
    public float maxAltitude = 50f;         // 최대 고도
    public float gravityScale = 1f;         // 중력 영향

    private ItemInstance jetPack;
    private Rigidbody rb;
    private float groundLevel;              // 지면 높이
    private bool isUsingJetpack;
    private GameObject player;
    private UIManager uiManager;
    private PlayerItemHandler playerItemHandler;

    [HideInInspector] public float foolPowerTime = 5f;
    private float currentPowerTime;
    private bool isPower;

    void Start()
    {
        player = GameObject.Find("Player");
        jetPack = GetComponent<ItemInstance>(); 
        rb = player.GetComponent<Rigidbody>();
        groundLevel = player.transform.position.y;  // 시작 지점을 지면으로 설정
        currentPowerTime = foolPowerTime;
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        playerItemHandler = player.GetComponent<PlayerItemHandler>();
    }
    private void OnEnable()
    {
        currentPowerTime = foolPowerTime;
        isPower = true;
    }
    private void OnDisable()
    {
        if (uiManager.jetpackBar)
        {
            uiManager.jetpackBar.SetActive(false);
        }
    }
    void Update()
    {
        if (!playerItemHandler.backSocketItem) return;
        if (GetComponent<ItemInstance>() != playerItemHandler.backSocketItem) return;

        if (currentPowerTime <= 0)
        {
            isPower = false;
        }

        if (Input.GetKey(KeyCode.Space) && isPower)
        {
            isUsingJetpack = CanUseFuelProduct(jetPack);
            if (isUsingJetpack)
            {
                currentPowerTime -= Time.deltaTime;
            }
        }
        else if (!Input.GetKey(KeyCode.Space))
        {
            isUsingJetpack = false;
            if (currentPowerTime < foolPowerTime)
            {
                currentPowerTime += Time.deltaTime / 3;
            }
        }
        else if (!isPower)
        {
            isUsingJetpack = false;
            currentPowerTime += Time.deltaTime / 3;
            if (currentPowerTime >= foolPowerTime)
            {
                isPower = true;
            }
        }

        if (uiManager)
        {
            uiManager.UpdateJetpackUI(jetPack, currentPowerTime, foolPowerTime);
        }
    }

    private void FixedUpdate()
    {
        if (isUsingJetpack)
        {
            UseJetpack();
        }
    }

    private bool CanUseFuelProduct(ItemInstance jetPack)
    {
        ItemInstance fuel = jetPack.Get<ItemInstance>("Fuel");
        if (!fuel)
        {
            Debug.Log("현재 연료통이 존재하지 않습니다!");
            return false;
        }
        float rate = fuel.Get<float>("fuelUsageRate");
        if (rate <= 0)
        {
            Debug.Log("연료가 모두 소진되었습니다!");
            return false;
        }
        if (isPower)
        {
            rate -= Time.deltaTime;
            fuel.Set<float>("fuelUsageRate", rate);
        }

        return true;
    }

    void UseJetpack()
    {
        float currentAltitude = player.transform.position.y - groundLevel;

        if (currentAltitude < maxAltitude)
        {
            rb.AddForce(Vector3.up * jetpackForce, ForceMode.Force);
        }
        else
        {
            rb.AddForce(Vector3.up * (jetpackForce * 0.3f), ForceMode.Acceleration);
        }
    }
}
