using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    private Animator animator;

    void Start()
    {
        // Karakterin üzerindeki Animator bileþenini alýyoruz
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(x, 0f, z);

        // Eðer tuþlara basýlýyorsa (hareket varsa)
        if (movement.magnitude > 0.1f)
        {
            // Karakterin hareket ettiði yöne dönmesini saðlar
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);

            // Yürüme animasyonunu baþlat
            if (animator != null) animator.SetBool("isWalking", true);
        }
        else
        {
            // Duruyorsa Idle animasyonuna dön
            if (animator != null) animator.SetBool("isWalking", false);
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}