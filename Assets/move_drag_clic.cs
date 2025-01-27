using UnityEngine;

public class move_drag_clic : MonoBehaviour
{
	// Vitesse de d�placement de la cam�ra
	public float moveSpeed = 5f;

	// Bool�en pour suivre si le clic droit est enfonc�
	private bool isDragging = false;

	// Position initiale de la souris
	private Vector3 lastMousePosition;
	void Update()
	{
		// Gestion du clic droit de la souris
		HandleMouseInput();
	}

	void HandleMouseInput()
	{
		// V�rifie si le bouton droit de la souris est enfonc�
		if (Input.GetMouseButtonDown(1)) // 1 correspond au bouton droit
		{
			// Commence le drag
			isDragging = true;
			// M�morise la position initiale de la souris
			lastMousePosition = Input.mousePosition;
		}

		// V�rifie si le bouton droit de la souris est rel�ch�
		if (Input.GetMouseButtonUp(1))
		{
			// Arr�te le drag
			isDragging = false;
		}

		// Si on est en train de dragger
		if (isDragging)
		{
			// Calcule le d�placement de la souris
			Vector3 delta = Input.mousePosition - lastMousePosition;

			// Convertit le d�placement en unit�s du monde
			Vector3 move = Camera.main.ScreenToWorldPoint(Input.mousePosition) -
						   Camera.main.ScreenToWorldPoint(lastMousePosition);

			// Ajuste le d�placement (inversez le signe si n�cessaire)
			move.z = 0; // Assurez-vous que le mouvement est en 2D

			// D�place la cam�ra
			transform.position -= move;

			// Met � jour la derni�re position de la souris
			lastMousePosition = Input.mousePosition;
		}
	}
}
