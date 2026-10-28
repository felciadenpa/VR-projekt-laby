using UnityEngine;
using UnityEngine.InputSystem; // Dla nowej obsługi klawiatury i myszy
using UnityEngine.XR; // Dla sprawdzania statusu gogli VR

public class VRKeyboardMovement : MonoBehaviour
{
    public float speed = 3.0f;

    [Tooltip("Przeciągnij tutaj Main Camera ze środka XR Origin")]
    public Transform headCamera;

    [Header("Ustawienia myszki (działają tylko bez VR)")]
    public float mouseSensitivity = 15f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // Jeśli nie wykryto gogli VR na starcie, blokujemy kursor na środku i ukrywamy go
        if (!XRSettings.isDeviceActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // 1. OBRÓT KAMERĄ (MYSZKA) - tylko gdy nie ma gogli VR
        if (!XRSettings.isDeviceActive && Mouse.current != null)
        {
            // Odczytanie przesunięcia myszy (delta)
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * Time.deltaTime;

            yRotation += mouseDelta.x;
            xRotation -= mouseDelta.y;

            // Zablokowanie obrotu góra/dół, żeby kamera nie zrobiła "fikołka" (max 90 stopni)
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Aplikowanie obrotu do kamery
            headCamera.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }

        // 2. RUCH KLAWIATURĄ (WASD)
        if (Keyboard.current == null) return;

        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Keyboard.current.wKey.isPressed) moveVertical += 1f;
        if (Keyboard.current.sKey.isPressed) moveVertical -= 1f;
        if (Keyboard.current.dKey.isPressed) moveHorizontal += 1f;
        if (Keyboard.current.aKey.isPressed) moveHorizontal -= 1f;

        // Obliczanie kierunku ruchu na podstawie tego, gdzie patrzy kamera
        Vector3 forward = headCamera.forward;
        Vector3 right = headCamera.right;

        // Zerowanie osi Y
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized;

        // Przesuwanie głównego obiektu (XR Origin)
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}