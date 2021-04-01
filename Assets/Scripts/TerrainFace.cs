using UnityEngine;

public class TerrainFace
{
    private readonly ShapeGenerator shapeGenerator;
    private readonly Mesh mesh;
    private readonly int resolution;
    private Vector3 localUp;
    private Vector3 axisA;
    private Vector3 axisB;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 2 * 3]; // number of squares multiplied by two triangles in a square, by 3 vertices in a triangle
        int triIndex = 0;
        Vector2[] uv = mesh.uv;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + (y * resolution);
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnCube = localUp + ((percent.x - .5f) * 2 * axisA) + ((percent.y - .5f) * 2 * axisB);
                Vector3 pointOnSphere = pointOnCube.normalized;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnSphere);
                //todo

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;

                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
    }

    public void UpdateUV(ColorGenerator colorGenerator)
    {
        Vector2[] uv = new Vector2[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + (y * resolution);
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnCube = localUp + ((percent.x - .5f) * 2 * axisA) + ((percent.y - .5f) * 2 * axisB);
                Vector3 pointOnSphere = pointOnCube.normalized;
                uv[i] = new Vector2(colorGenerator.BiomePercentFromPoint(pointOnSphere), 0);
            }
        }

        mesh.uv = uv;
    }
}
