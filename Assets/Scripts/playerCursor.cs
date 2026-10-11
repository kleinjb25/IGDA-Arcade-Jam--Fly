using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class playerCursor : MonoBehaviour
{
    public RectTransform cursor;
    public Camera gameCamera;
    public float cursorSpeed = 420;
    [SerializeField] private int pNum;

    private Vector2 moveInput;
    void Update()
    {
        moveCursor();
    }

    public void OnMove(InputAction.CallbackContext c)
    {
        moveInput = c.ReadValue<Vector2>();
    }
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        Debug.Log("select has been pushed");
        clickWithCursor();
    }

    private void moveCursor()
    {
        cursor.position += (Vector3)(moveInput * cursorSpeed * Time.deltaTime);
        cursor.position = new Vector3(Mathf.Clamp(cursor.position.x, 0f, Screen.width), Mathf.Clamp(cursor.position.y, 0f, Screen.height), cursor.position.z);
    }

    private void clickWithCursor()
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, cursor.position);
        Ray r = gameCamera.ScreenPointToRay(screenPos);
        int layerMask = LayerMask.GetMask("problemSpot");
        if (Physics.Raycast(r, out RaycastHit h, Mathf.Infinity, layerMask))
        {
            Debug.Log("hit" + h.collider.gameObject.name);
            problemSpot p = h.collider.GetComponentInParent<problemSpot>();
            if (p != null)
            {
                Debug.Log("found ps");
                if (p.select(pNum)) gamemanager.instance.useAttempt(pNum, p);
            }
        }
    }
}
