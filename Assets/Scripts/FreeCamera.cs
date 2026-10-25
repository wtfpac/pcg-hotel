using UnityEngine;

// camera livre de debug. atravessa paredes de proposito,
// serve so pra inspecionar a planta gerada.
public class FreeCamera : MonoBehaviour
{
    public float speed = 12f;
    public float boostMultiplier = 3f;   // segurar shift anda mais rapido
    public float mouseSensitivity = 2f;

    private float yaw;
    private float pitch;
    private bool looking;

    void Start()
    {
        // comeca com a rotacao que a camera ja tem na cena
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void Update()
    {
        HandleLook();
        HandleMove();
    }

    void HandleLook()
    {
        // so gira enquanto o botao direito estiver pressionado.
        // assim o mouse continua livre pra mexer no editor.
        if (Input.GetMouseButtonDown(1))
            looking = true;

        if (Input.GetMouseButtonUp(1))
            looking = false;

        if (!looking)
            return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // trava pra nao virar de cabeca pra baixo
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleMove()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += transform.forward;
        if (Input.GetKey(KeyCode.S)) direction -= transform.forward;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;
        if (Input.GetKey(KeyCode.A)) direction -= transform.right;

        // sobe e desce no eixo do mundo, nao no da camera
        if (Input.GetKey(KeyCode.E)) direction += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) direction -= Vector3.up;

        float current = speed;

        if (Input.GetKey(KeyCode.LeftShift))
            current *= boostMultiplier;

        transform.position += direction.normalized * current * Time.deltaTime;
    }
}