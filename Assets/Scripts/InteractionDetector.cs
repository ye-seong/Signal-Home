using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Transform bodyCenter;

    
    private RaycastHit hit;
    private KeyCode InteractionKey = KeyCode.F;

    private ItemInstance highlightedItem;
    void Start()
    {
       
    }
    void Update()
    {
        SetHighlight();
        if (Input.GetKeyDown(InteractionKey))
        {
            CheckInteraction();
        }
    }

    private void CheckInteraction()
    {
        if (CheckIsUIOpen()) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (hit.collider.CompareTag("Bench"))
            {
                uiManager.ToggleBench();
            }
        }
    }

    private bool CheckIsUIOpen()
    {
        if (uiManager.isBenchOpen)
        {
            GameObject Tooltip = GameObject.Find("Tooltip");
            if (Tooltip) uiManager.ClearTooltip(Tooltip);
            uiManager.ToggleBench();
            return true;
        }
        return false;
    }

    public ItemInstance GetCurrentTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out hit, 50f))
        {
            float distanceFromBody = Vector3.Distance(bodyCenter.position, hit.collider.transform.position);

            ItemInstance item = hit.collider.GetComponent<ItemInstance>();
            if (item != null && distanceFromBody <= 8f)
            {
                return item;
            }
        }
        return null;
    }

    public LivingEntity GetLivingEntityTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, 50f))
        {
            float distanceFromBody = Vector3.Distance(bodyCenter.position, hit.collider.transform.position);
            LivingEntity target = hit.collider.GetComponent<LivingEntity>();
            if (target && distanceFromBody <= 3f)
            {
                return target;
            }
        }
        return null;
    }

    private void SetHighlight()
    {
        ItemInstance item = GetCurrentTarget();
        if (highlightedItem)
        {
            if (!item)
            {
                highlightedItem.HideOutLine();
                highlightedItem = null;
            }
            else if (item && item != highlightedItem)
            {
                highlightedItem.HideOutLine();
                highlightedItem = item;
            }
        }

        if (item)
        {
            item.ShowOutLine();
            highlightedItem = item;
        }
    }
}
