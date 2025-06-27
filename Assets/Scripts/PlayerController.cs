using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform cameraTransform;

    [Header("Managers")]
    [SerializeField] private GameObject UIManager;

    private Rigidbody rb;
    private bool isGrounded;
    private float xRotation = 0f;
    private Inventory inventory;
    private InteractionDetector interactionDetector;
    private UIManager uiManager;
    private QuickPanel quickPanel;
    private PlayerItemHandler playerItemHandler;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inventory = GetComponent<Inventory>();
        interactionDetector = GetComponent<InteractionDetector>();
        uiManager = UIManager.GetComponent<UIManager>();

        if (cameraTransform == null)
        {
            Camera playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera != null)
                cameraTransform = playerCamera.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        quickPanel = uiManager.quickSlotPanel.GetComponent<QuickPanel>();
        playerItemHandler = GetComponent<PlayerItemHandler>();
    }

    void Update()
    {
        if (!GameState.IsUIOpen)
        {
            HandleMouseLook();
            HandleMovement();
            HandleJump();
            HandleQuickSlot();
            HandleUseItem();
        }
        
        if (GameState.IsUIOpen && inventory.isHovering)
        {
            HandleSetQuickSlot();
        }
        HandleItemPickup();
    }

    void HandleMouseLook()
    {
        if (cameraTransform == null) return;

        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Y축 회전 (좌우 - 플레이어 몸체)
        transform.Rotate(Vector3.up * mouseX);

        // X축 회전 (위아래 - 카메라만)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleUseItem()
    {
        if (Input.GetMouseButtonDown(1) && playerItemHandler && playerItemHandler.currentItem)
        {
            playerItemHandler.UseItem();
        }
    }
    void HandleMovement()
    {
        // 입력 받기 (WASD)
        float horizontal = Input.GetAxis("Horizontal"); // A, D
        float vertical = Input.GetAxis("Vertical");     // W, S

        // 이동 방향 계산 (플레이어 기준)
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        direction = direction.normalized;

        // 물리적 이동 (Y축 속도는 유지)
        Vector3 moveVelocity = direction * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleItemPickup()
    {
        if (Input.GetKeyDown(KeyCode.F) && !uiManager.isBenchOpen)
        {
            TryPickupItem();
        }
    }

    void TryPickupItem()
    {
        if (!interactionDetector) return;   

        ItemInstance item = interactionDetector.GetCurrentTarget();

        if (item != null)
        {
            bool success = inventory.AddItem(item);
            uiManager.UpdateItemUI();

            if (success)
            {
                //Destroy(item.gameObject);
                item.gameObject.SetActive(false);
            }
        }
    }

    void HandleQuickSlot()
    {
        for (int i = 0; i < quickPanel.maxSlots; i++)
        {
            int key = i + 1;
            if (Input.GetKeyDown(key.ToString()))
            {
                quickPanel.currentSlotIndex = i;
                quickPanel.HoldingItem();
            }
        }
    }

    void HandleSetQuickSlot()
    {
        quickPanel.SwitchQuickSlotItem();
    }

    // 바닥 감지
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}