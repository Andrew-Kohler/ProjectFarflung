using UnityEngine;

public class InverseLight : MonoBehaviour
{
    public float radius = 5f; // Radius inverse light effect
    public float strength = 1f; // Strength of darkness effect
    public Color darknessColor = Color.black; // Color of the darkness (default to black)

    private void OnDrawGizmosSelected()
    {
        // Draw the radius in the Scene view for debugging
        Gizmos.color = new Color(darknessColor.r, darknessColor.g, darknessColor.b, 0.5f);
        Gizmos.DrawSphere(transform.position, radius);
    }

    void Update()
    {
        // Find all objects within the radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            // Check if the object has a Renderer
            Renderer renderer = collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Apply a fake "darkness" effect by blending the material color
                Material material = renderer.material;

                // Calculate distance to the object
                float distance = Vector3.Distance(transform.position, collider.transform.position);

                // Scale the effect by distance (closer objects are darker)
                float darknessFactor = Mathf.Clamp01(1f - (distance / radius)) * strength;

                // Set the object's material color to simulate darkness
                material.color = Color.Lerp(material.color, darknessColor, darknessFactor);
            }
        }
    }
}
