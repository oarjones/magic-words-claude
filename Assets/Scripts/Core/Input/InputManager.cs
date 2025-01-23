// InputManager.cs
using UnityEngine;
using UnityEngine.EventSystems;
using MagicWords.Core.Board;

namespace MagicWords.Core.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private float dragThreshold = 0.1f;

        private Vector2 touchStartPos;
        private bool isDragging;
        private ITile lastSelectedTile;
        private IBoard board;

        private void Awake()
        {
            board = GetComponent<IBoard>();
            if (gameCamera == null) gameCamera = Camera.main;
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                HandleTouchStart(UnityEngine.Input.mousePosition);
            }
            else if (UnityEngine.Input.GetMouseButton(0))
            {
                HandleTouchMove(UnityEngine.Input.mousePosition);
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                HandleTouchEnd();
            }
        }

        private void HandleTouchStart(Vector2 position)
        {
            touchStartPos = position;
            isDragging = false;

            var hit = Physics2D.Raycast(gameCamera.ScreenToWorldPoint(position), Vector2.zero);
            if (hit.collider != null)
            {
                var tile = hit.collider.GetComponent<ITile>();
                if (tile != null)
                {
                    TrySelectTile(tile);
                }
            }
        }

        private void HandleTouchMove(Vector2 position)
        {
            if ((position - touchStartPos).magnitude > dragThreshold)
            {
                isDragging = true;
                var hit = Physics2D.Raycast(gameCamera.ScreenToWorldPoint(position), Vector2.zero);
                if (hit.collider != null)
                {
                    var tile = hit.collider.GetComponent<ITile>();
                    if (tile != null && tile != lastSelectedTile)
                    {
                        TrySelectTile(tile);
                    }
                }
            }
        }

        private void HandleTouchEnd()
        {
            if (!isDragging)
            {
                // Handle tap logic if needed
            }

            isDragging = false;
            lastSelectedTile = null;
        }

        private void TrySelectTile(ITile tile)
        {
            if (board.ValidateMove(lastSelectedTile, tile))
            {
                board.TrySelectTile(tile);
                lastSelectedTile = tile;
            }
        }
    }
}