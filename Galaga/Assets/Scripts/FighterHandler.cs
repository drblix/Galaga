using TreeEditor;
using UnityEngine;

namespace Galaga
{
    public class FighterHandler : MonoBehaviour
    {
        [SerializeField, Tooltip("Position where player shots are created; relative to player's position")]
        private Vector2 _shootPos;

        [SerializeField, Tooltip("Speed the player moves")]
        private float _moveSpeed = 5f;

        [SerializeField, Tooltip("How far the player can be to the sides of the screen")]
        private float _xBorder = 8.3f;

        private float _moveAxis = 0f;

        private void Start()
        {
            PlayerInputEvents.OnPlayerMove += OnMove;
            PlayerInputEvents.OnPlayerShoot += OnShoot;
        }

        private void Update()
        {
            float moveAmount = _moveAxis * _moveSpeed * Time.deltaTime;
            float newX = Mathf.Clamp(transform.position.x + moveAmount, -_xBorder, _xBorder);

            transform.position = new Vector2(newX, transform.position.y);
        }

        private void OnShoot()
        {
            
        }

        private void OnMove(float x) => _moveAxis = x;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)transform.position + _shootPos, Vector3.one * .05f);
        }
    }
}
