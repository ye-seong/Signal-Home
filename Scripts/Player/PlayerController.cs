using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private GroundChecker groundChecker;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Step Climbing")]
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepDistance = 0.6f;
    [SerializeField] private LayerMask groundLayer = 1;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform cameraTransform;

    [Header("Managers")]
    [SerializeField] private UIManager uiManager;

    [Header("Renderer")]
    [SerializeField] private SkinnedMeshRenderer playerRenderer;

    [HideInInspector] public Animator animator;

    [HideInInspector] public ArmorSkill currentSkill;

    private Rigidbody rb;
    private float xRotation = 0f;
    private Inventory inventory;
    private InteractionDetector interactionDetector;
    private QuickPanel quickPanel;
    private PlayerItemHandler playerItemHandler;
    private UseItem useItem;
    private PlayerState playerState;
    [HideInInspector] public bool isPaused = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inventory = GetComponent<Inventory>();
        interactionDetector = GetComponent<InteractionDetector>();

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
        playerState = GetComponent<PlayerState>();
        useItem = GetComponent<UseItem>();
        animator = GetComponent<Animator>();

        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        
    }

    void Update()
    {
        HandleEsc();
        if (isPaused) return;
        if (!GameState.IsUIOpen)
        {
            HandleMouseLook();
            HandleMovement();
            HandleJump();
            HandleQuickSlot();
            HandleUseItem();
            HandleModuleSkill();
        }
        
        if (GameState.IsUIOpen && inventory.isHovering)
        {
            HandleSetQuickSlot();
        }
        HandleItemPickup();
        SkillTimerController();
    }

    void HandleEsc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiManager.SetEscMenu();
        }
    }
    void HandleMouseLook()
    {
        if (cameraTransform == null) return;

        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Y�� ȸ�� (�¿� - �÷��̾� ��ü)
        transform.Rotate(Vector3.up * mouseX);

        // X�� ȸ�� (���Ʒ� - ī�޶�)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleUseItem()
    {
        if (playerItemHandler && playerItemHandler.currentItem)
        {
            playerItemHandler.UseItem();
        }
    }
    void HandleMovement()
    {
        // �Է� �ޱ� (WASD)
        float horizontal = Input.GetAxis("Horizontal"); // A, D
        float vertical = Input.GetAxis("Vertical");     // W, S

        // �̵� ���� ��� (�÷��̾� ����)
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        direction = direction.normalized;

        // ��� üũ (�̵� ���� ����)
        if (direction.magnitude > 0.1f)
        {
            CheckForStep(direction);
        }

        // ������ �̵� (Y�� �ӵ��� ����)
        Vector3 moveVelocity = direction * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    void CheckForStep(Vector3 moveDirection)
    {
        // ���� ���� ���� ���� ��� üũ
        if (!groundChecker.IsGrounded()) return;

        // �÷��̾� �� ��ġ���� �������� ����ĳ��Ʈ
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        // ���ʿ� ��ֹ��� �ִ��� üũ
        if (Physics.Raycast(rayOrigin, moveDirection, out RaycastHit hit, stepDistance, groundLayer))
        {
            // ��� ���� üũ
            Vector3 stepTopPosition = hit.point + Vector3.up * stepHeight + moveDirection * 0.1f;

            // ��� ���� ������ �ִ��� üũ
            if (!Physics.Raycast(stepTopPosition, Vector3.down, stepHeight + 0.1f, groundLayer))
            {
                // ��� ���� �ε巴�� �ö󰡱�
                float stepUp = Mathf.Min(stepHeight, hit.point.y - transform.position.y + stepHeight);
                if (stepUp > 0.05f) // �ּ� �������� ���� ����
                {
                    transform.position += Vector3.up * stepUp;
                }
            }
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && groundChecker.IsGrounded())
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

    public void TryPickupItem()
    {
        if (!interactionDetector) return;   

        ItemInstance item = interactionDetector.GetCurrentTarget();

        if (item != null)
        {
            bool success = inventory.AddItem(item);
            uiManager.UpdateItemUI();

            if (success)
            {
                item.isWorldItem = false;
                item.gameObject.SetActive(false);
            }
        }
    }

    void HandleQuickSlot()
    {
        for (int i = 0; i < quickPanel.maxSlots; i++)
        {
            int key = i + 1;
            if (Input.GetKeyDown(key.ToString()) && !useItem.chargeUIOpen)
            {
                quickPanel.currentSlotIndex = i;
                quickPanel.HoldingItem();
            }
        }
    }

    void SkillTimerController()
    {
        if (!currentSkill) return;
        if (currentSkill.isSkill)
        {
            currentSkill.skillDurationTimer -= Time.deltaTime;
            if (currentSkill.skillDurationTimer <= 0)
            {
                currentSkill.isSkill = false;
                currentSkill.isCooldown = true;
                currentSkill.ResetSkill();
                currentSkill.skillDurationTimer = currentSkill.skillDuration;
            }
        }

        if (currentSkill.isCooldown)
        {
            currentSkill.skillCooldownTimer -= Time.deltaTime;
            if (currentSkill.skillCooldownTimer <= 0)
            {
                currentSkill.isCooldown = false;
                currentSkill.skillCooldownTimer = currentSkill.skillCooldown;
            }
        }
    }
    void HandleModuleSkill()
    {
        if (!playerState.equipmentItems[1]) return;
        ItemInstance moduleManager = playerState.equipmentItems[1];
        ItemInstance skillModule = moduleManager.Get<ItemInstance[]>("armorModules")[2];
        if (!skillModule) return;
        ArmorSkill skill = skillModule.GetComponent<ArmorSkill>();
        if (!skill) return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            skill.SetSkill(this);
            currentSkill = skill;
        }
    }
    void HandleSetQuickSlot()
    {
        quickPanel.SwitchQuickSlotItem();
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetPlayerInvisibility(bool invisible)
    {
        playerState.isInvisible = invisible;
        playerRenderer.enabled = !invisible;
        uiManager.invisibilityBar.SetActive(invisible);
    }
}