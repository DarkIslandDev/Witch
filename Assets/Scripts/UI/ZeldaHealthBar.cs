// Canvas Setup:
// Create a Canvas in World Space to hold the health bar UI elements.
//     Add two Image components as the background and foreground of the health bar.
//     Heart Prefab:
// Create a heart prefab (representing a single heart container).
// Each heart prefab will form a linked list.
//     Algorithm:
// Write an algorithm that recurses through each heart.
//     Update the fill amount of the heart image based on the player’s current and maximum health.
//     Script:
// Create a C# script called Health.
//     Declare variables for:
//     Health value
// Number of hearts
//     Array of images for the hearts
// Sprites for full and empty hearts
// In the Update function, check the index of the heart image and set the sprite and visibility accordingly.
//     Limit the health to the number of hearts.
//     Here’s a simplified example of how you might structure your C# script for the health bar:


using UnityEngine;
using UnityEngine.UI;

public class ZeldaHealthBar
{
    public int maxHealth = 10; // Set the maximum health
    public Image heartPrefab; // Reference to the heart prefab
    public Transform heartsParent; // Parent transform for heart containers

    private Image[] hearts; // Array to store heart images
    private int currentHealth; // Current health value

    void Start()
    {
        currentHealth = maxHealth;
        hearts = new Image[maxHealth];

        // Instantiate heart containers
        for (int i = 0; i < maxHealth; i++)
        {
            // hearts[i] = Instantiate(heartPrefab, heartsParent);
            // Set heart position based on index (e.g., arrange them horizontally)
        }
    }

    // Function to update health display
    public void UpdateHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);

        for (int i = 0; i < maxHealth; i++)
        {
            hearts[i].enabled = i < currentHealth; // Show/hide hearts based on health
        }
    }
}