using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerGFXTransform;

    public bool isLeft;
    
    public void UpdateAnimatorValues(Vector2 movementInput)
    {
        float h = movementInput.x;
        float v = movementInput.y;

        if (h == 0) h = 0;
        if (v == 0) v = 0;

        switch (h)
        {
            case <= -0.1f:
                EntityAngleRotation.AngleRotation(playerGFXTransform, 180);
                isLeft = true;
                animator.SetFloat("Horizontal", 1, 0.1f, Time.deltaTime);
                break;
            case >= 0.1f:
                EntityAngleRotation.AngleRotation(playerGFXTransform, 0);
                isLeft = false;
                animator.SetFloat("Horizontal", 1, 0.1f, Time.deltaTime);
                break;
            default:
                animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                break;
        }

        switch (v)
        {
            case <= -0.1f:
                animator.SetFloat("Vertical", -1, 0.1f, Time.deltaTime);
                break;
            case >= 0.1f:
                animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                break;
            default:
                animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                break;
        }
    }
    
    
}