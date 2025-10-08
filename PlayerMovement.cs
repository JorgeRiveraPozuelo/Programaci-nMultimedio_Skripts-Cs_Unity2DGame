using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float wallJumpXForce = 10f;
    [SerializeField] private float wallJumpYForce = 15f;
    [SerializeField] private float wallJumpDiagonalX = 5f;
    [SerializeField] private float wallJumpDiagonalY = 12f;
    [SerializeField] private float wallJumpCooldownTime = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Movimiento normal cuando no está en cooldown
        if (wallJumpCooldown <= 0)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);
            
            // Si está en la pared y no en el suelo, se pega a la pared
            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.linearVelocity = new Vector2(body.linearVelocity.x, 0); // Se queda pegado
            }
            else
            {
                body.gravityScale = 3;
            }

            if (Input.GetKeyDown(KeyCode.Space))
                Jump();
        }
        else
        {
            wallJumpCooldown -= Time.deltaTime;
        }
    }

    private void Jump()
    {
        // Salto normal desde el suelo
        if(isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
        }
        // Saltos desde la pared
        else if (onWall() && !isGrounded())
        {
            float verticalInput = Input.GetAxis("Vertical");
            
            // Salto hacia arriba (vertical positivo)
            if (verticalInput > 0.01f)
            {
                body.linearVelocity = new Vector2(0, wallJumpYForce);
                // El personaje mantiene su orientación
            }
            // Salto neutral (sin input) - empuja horizontalmente lejos de la pared
            else if (Mathf.Abs(horizontalInput) < 0.01f && Mathf.Abs(verticalInput) < 0.01f)
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpXForce, jumpPower);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            // Salto diagonal (con input horizontal)
            else
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpDiagonalX, wallJumpDiagonalY);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
           
            wallJumpCooldown = wallJumpCooldownTime;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            Vector2.down,
            0.1f,
            groundLayer
        );
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            new Vector2(transform.localScale.x, 0),
            0.1f,
            wallLayer
        );
        return raycastHit.collider != null;
    }
}