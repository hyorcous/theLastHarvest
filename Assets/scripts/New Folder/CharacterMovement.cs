using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    private Animator animator;
    private Transform camTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (Camera.main != null) camTransform = Camera.main.transform;
    }

    void Update()
    {
        if (NPCDialogue.isDialogueActive)
        {
            if (animator != null) animator.SetBool("isWalking", false);
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Kameranýn baktýðý yöne göre hareket vektörü hesapla
        Vector3 forward = camTransform != null ? camTransform.forward : Vector3.forward;
        Vector3 right = camTransform != null ? camTransform.right : Vector3.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * z + right * x).normalized;

        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);

            if (animator != null) animator.SetBool("isWalking", true);
        }
        else
        {
            if (animator != null) animator.SetBool("isWalking", false);
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}