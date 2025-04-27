using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    private Rect bounds;
    private int capacity;
    private List<Vector2> points;
    private bool divided;
    private QuadTree northEast, northWest, southEast, southWest;

    public QuadTree(Rect _bounds, int _capacity)
    {
        bounds = _bounds;
        capacity = _capacity;
        points = new List<Vector2>();
        divided = false;
    }

    public bool Insert(Vector2 point)
    {
        if (bounds.Contains(point)) return false;

        if (points.Count < capacity)
        {
            points.Add(point);
            return true;
        }
        else
        {
            if (!divided)
            {
                Subdivide();
            }

            if (northEast.Insert(point) || northWest.Insert(point) ||
                southEast.Insert(point) || southWest.Insert(point))
                return true;
        }

        return false;
    }

    public List<Vector2> Query(Rect range)
    {
        List<Vector2> found = new List<Vector2>();
        if (!bounds.Overlaps(range))
        {
            return found;
        }

        foreach (Vector2 p in points)
        {
            if (range.Contains(p))
            {
                found.Add(p);
            }
        }

        if (divided)
        {
            found.AddRange(northEast.Query(range));
            found.AddRange(northWest.Query(range));
            found.AddRange(southEast.Query(range));
            found.AddRange(southWest.Query(range));
        }

        return found;
    }

    private void Subdivide()
    {
        float x = bounds.x;
        float y = bounds.y;
        float w = bounds.width / 2;
        float h = bounds.height / 2;

        northEast = new QuadTree(new Rect(x + w, y, w, h), capacity);
        northWest = new QuadTree(new Rect(x, y, w, h), capacity);
        southEast = new QuadTree(new Rect(x + w, y + h, w, h), capacity);
        southWest = new QuadTree(new Rect(x, y + h, w, h), capacity);

        divided = true;
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector3(bounds.x + bounds.width / 2, 0, bounds.y + bounds.height / 2), // Center of the rectangle
            // new Vector3(bounds.x, 0, bounds.y), // Center of the rectangle
            new Vector3(bounds.width, 0, bounds.height) // Size of the rectangle
        );
        
        Gizmos.color = Color.red;
        foreach (var point in points)
        {
            Gizmos.DrawSphere(new Vector3(point.x, 0, point.y), 2f); // Ensure points are at (x, y, 0)
        }
        
        if (divided)
        {
            northEast.DrawGizmos();
            northWest.DrawGizmos();
            southEast.DrawGizmos();
            southWest.DrawGizmos();
        }
    }
}
