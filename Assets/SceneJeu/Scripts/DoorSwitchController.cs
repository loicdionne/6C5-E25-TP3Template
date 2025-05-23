using UnityEngine;

namespace SceneJeu.Scripts
{
    public class DoorSwitchController : MonoBehaviour
    {
        public GameObject door;
        public Material doorClosedMaterial;
        public Material doorOpenMaterial;
        public Renderer doorRenderer;

        private bool doorIsOpen = false;

        void Update()
        {
            // Déplacement de base WASD
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            transform.Translate(new Vector3(h, 0, v) * Time.deltaTime * 5);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Switch"))
            {
                ToggleDoor();
            }
        }

        private void ToggleDoor()
        {
            doorIsOpen = !doorIsOpen;
            doorRenderer.material = doorIsOpen ? doorOpenMaterial : doorClosedMaterial;
        }
    }
}
