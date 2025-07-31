using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackController : MonoBehaviour
{
    [Header("Jetpack Settings")]
    public float jetpackForce = 15f;        // ��Ʈ�� ������
    public float maxAltitude = 50f;         // �ִ� ��
    public float gravityScale = 1f;         // �߷� ����

    private ItemInstance jetPack;
    private Rigidbody rb;
    private float groundLevel;              // ���� ����
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
        groundLevel = player.transform.position.y;  // ���� ������ �������� ����
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
            Debug.Log("���� �������� �������� �ʽ��ϴ�!");
            return false;
        }
        float rate = fuel.Get<float>("fuelUsageRate");
        if (rate <= 0)
        {
            Debug.Log("���ᰡ ��� �����Ǿ����ϴ�!");
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
