using UnityEngine;

public class ParticleSimulator : MonoBehaviour
{
    public float speed = 1f; // Speed of particle motion
    public float gravity = 9.8f; // Gravity acceleration

    private Vector2 velocity; // Current velocity of the particle
    private Vector2 startPosition; // Initial position of the particle

    void Start()
    {
        // Set the initial position of the particle
        startPosition = transform.position;

        // Calculate an initial velocity based on the desired behavior (e.g., rising, jumping, falling)
        // Example: Rising up
        velocity = new Vector2(0f, speed);
    }

    void Update()
    {
        // Update the particle's position based on the velocity
        transform.position += (Vector3)velocity * Time.deltaTime;

        // Update the velocity to simulate gravity
        velocity.y -= gravity * Time.deltaTime;

        // Check if the particle has fallen below its initial position (or any other condition for destroying the particle)
        if (transform.position.y < startPosition.y)
        {
            Destroy(gameObject); // Destroy the particle object
        }
    }
}