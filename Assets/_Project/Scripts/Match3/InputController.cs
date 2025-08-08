using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private BoardController board;
    [SerializeField] private float minSwipeDistance = 0.2f;

    private TileView selected;
    private Vector3 pressWorld;

    private void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pressWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            var tile = RaycastTile(Input.mousePosition);
            if (tile != null) selected = tile;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (selected == null) return;
            Vector3 releaseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = releaseWorld - pressWorld;
            TrySwipe(delta);
            selected = null;
        }
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 0) return;
        var t = Input.GetTouch(0);
        if (t.phase == TouchPhase.Began)
        {
            pressWorld = cam.ScreenToWorldPoint(t.position);
            var tile = RaycastTile(t.position);
            if (tile != null) selected = tile;
        }
        else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
        {
            if (selected == null) return;
            Vector3 releaseWorld = cam.ScreenToWorldPoint(t.position);
            Vector3 delta = releaseWorld - pressWorld;
            TrySwipe(delta);
            selected = null;
        }
    }

    private TileView RaycastTile(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            if (board.TryGetTileFromCollider(hit.collider, out var tile)) return tile;
        }
        return null;
    }

    private void TrySwipe(Vector3 delta)
    {
        if (selected == null) return;
        delta.z = 0f;
        if (delta.magnitude < minSwipeDistance) return;

        int dr = 0, dc = 0;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            dc = delta.x > 0 ? 1 : -1;
        else
            dr = delta.y > 0 ? -1 : 1;

        int r2 = selected.Row + dr;
        int c2 = selected.Col + dc;
        board.TrySwap(selected.Row, selected.Col, r2, c2);
    }
}
