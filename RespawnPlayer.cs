using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private float fallThreshold = -10f; // Altura Y donde se activa el respawn
    [SerializeField] private Vector3 respawnPosition = new Vector3(0, 0, 0); // Posición inicial
    
    [Header("Optional: Set Respawn Point")]
    [SerializeField] private Transform respawnPoint; // Opcional: usa un GameObject vacío como punto de respawn
    
    private Vector3 startPosition;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        
        // Guarda la posición inicial del jugador
        startPosition = transform.position;
        
        // Si no se asignó una posición de respawn, usa la inicial
        if (respawnPosition == Vector3.zero)
            respawnPosition = startPosition;
    }

    private void Update()
    {
        // Verifica si el jugador cayó por debajo del límite
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        // Si hay un punto de respawn asignado, úsalo
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            transform.position = respawnPosition;
        }
        
        // Resetea la velocidad del Rigidbody2D
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
        }
    }

    // Método público para establecer un nuevo punto de respawn (útil para checkpoints)
    public void SetRespawnPosition(Vector3 newPosition)
    {
        respawnPosition = newPosition;
    }

    // Método para establecer el punto de respawn desde un Transform
    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
}