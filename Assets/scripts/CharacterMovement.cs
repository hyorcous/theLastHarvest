using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 1200f; // <-- Bu deðeri artýrdýk (Örn: 720 yerine 1200 yaptýk)
    private CharacterController controller;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float turn = Input.GetAxisRaw("Horizontal"); // A ve D tuþlarý (Saða/Sola dönme)
        float move = Input.GetAxisRaw("Vertical");   // W ve S tuþlarý (Ýleri/Geri gitme)

        // 1. Karakteri saða veya sola döndür (A ve D tuþlarý)
        if (turn != 0)
        {
            float rotationAmount = turn * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotationAmount, 0);
        }

        // 2. W veya S tuþuna basýldýðýnda karakterin baktýðý yöne doðru hareket et
        Vector3 moveDir = transform.forward * move;

        if (moveDir.magnitude >= 0.1f)
        {
            // Karakter Controller ile hareket ettiriyoruz (duvarlardan geçmez, yere basar)
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // Yürüme animasyonunu baþlat
            if (animator != null) animator.SetBool("isWalking", true);
        }
        else
        {
            // Duruyorsa Idle animasyonuna dön
            if (animator != null) animator.SetBool("isWalking", false);
        }
    }
}