using UnityEngine;

namespace LSA.Arena
{
    /// <summary>
    /// Compact flat arena with solid perimeter walls.
    /// </summary>
    public sealed class ArenaBounds : MonoBehaviour
    {
        [SerializeField] float halfExtent = 10f;
        [SerializeField] float wallHeight = 2.5f;
        [SerializeField] float wallThickness = 0.5f;

        public float HalfExtent => halfExtent;

        public void Build(float sizeHalfExtent = 10f)
        {
            halfExtent = sizeHalfExtent;
            // Floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(transform, false);
            floor.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            floor.transform.localScale = new Vector3(halfExtent * 2f, 0.2f, halfExtent * 2f);
            Tint(floor, new Color(0.22f, 0.26f, 0.32f));

            // Subtle center mark
            var mark = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mark.name = "CenterMark";
            mark.transform.SetParent(transform, false);
            mark.transform.localPosition = new Vector3(0f, 0.01f, 0f);
            mark.transform.localScale = new Vector3(2.2f, 0.01f, 2.2f);
            Object.Destroy(mark.GetComponent<Collider>());
            Tint(mark, new Color(0.35f, 0.4f, 0.48f));

            PlaceWall("WallNorth", new Vector3(0f, wallHeight * 0.5f, halfExtent + wallThickness * 0.5f),
                new Vector3(halfExtent * 2f + wallThickness * 2f, wallHeight, wallThickness));
            PlaceWall("WallSouth", new Vector3(0f, wallHeight * 0.5f, -halfExtent - wallThickness * 0.5f),
                new Vector3(halfExtent * 2f + wallThickness * 2f, wallHeight, wallThickness));
            PlaceWall("WallEast", new Vector3(halfExtent + wallThickness * 0.5f, wallHeight * 0.5f, 0f),
                new Vector3(wallThickness, wallHeight, halfExtent * 2f));
            PlaceWall("WallWest", new Vector3(-halfExtent - wallThickness * 0.5f, wallHeight * 0.5f, 0f),
                new Vector3(wallThickness, wallHeight, halfExtent * 2f));
        }

        void PlaceWall(string name, Vector3 pos, Vector3 scale)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(transform, false);
            wall.transform.localPosition = pos;
            wall.transform.localScale = scale;
            Tint(wall, new Color(0.16f, 0.18f, 0.22f));
        }

        static void Tint(GameObject go, Color color)
        {
            var rend = go.GetComponent<MeshRenderer>();
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            rend.material = new Material(shader) { color = color };
        }
    }
}
