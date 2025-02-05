using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerClickHandler
{
    public Image borderImage;  // Reference to the image that will serve as the border.
    public Color borderColor = Color.red; // The color of the border when active
    public float borderWidth = 5f; // Width of the border

    private bool isBorderActive = false; // Tracks whether the border is currently shown

    void Start()
    {
        if (borderImage != null)
        {
            borderImage.gameObject.SetActive(false);  // Ensure the border is hidden initially
        }
    }

    // Called when the button/toggle is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleBorder();
        
    }

    // Toggle the border visibility on and off
    private void ToggleBorder()
    {
        if (borderImage != null)
        {
            isBorderActive = !isBorderActive;

            if (isBorderActive)
            {
                // Show the border with the specified color and width
                borderImage.gameObject.SetActive(true);
                borderImage.color = borderColor;
                // Set the border width here (make sure the borderImage is set up properly to allow this)
                // Example: Change the image's RectTransform size based on borderWidth (if desired)
                RectTransform rt = borderImage.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x + borderWidth, rt.sizeDelta.y + borderWidth);
            }
            else
            {
                // Hide the border
                borderImage.gameObject.SetActive(false);
            }
        }
    }
}
