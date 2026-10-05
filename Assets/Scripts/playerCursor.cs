using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class playerCursor : MonoBehaviour
{
    public RectTransform cursor;
    public Camera gameCamera;
    public float cursorSpeed = 420;

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
        if (Physics.Raycast(r, out RaycastHit h))
        {

        }
    }
}
