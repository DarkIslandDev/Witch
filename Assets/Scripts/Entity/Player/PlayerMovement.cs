using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public new Rigidbody2D rigidbody;
    
    public void Movement(Vector2 movementInput, float speed) => rigidbody.velocity = movementInput * speed;
}