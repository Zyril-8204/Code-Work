using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace orion
{
    class Circles
    {
        const float PI = 3.14159265f;
        public Vectors vertex;
        public float radius;

        public Circles()
        {
            vertex = new Vectors();
            radius = 0f;
        }

        public Circles(Vectors position, float radiusLength)
        {
            vertex = position;
            radius = radiusLength;
        }

        public Circles(float x, float y, float z, float radiusLength)
        {
            vertex = new Vectors(x, y, z);
            radius = radiusLength;
        }

        public float distanceTo(Circles c1)
        {
            return distanceTo(c1.vertex);
        }

        public float distanceTo(Vectors c1)
        {
            return (float)Math.Sqrt(Math.Pow((c1.x - vertex.x), 2) +
                                     Math.Pow((c1.y - vertex.y), 2) +
                                     Math.Pow((c1.z - vertex.z), 2));
        }

        public static bool overlap(Circles c1, Circles c2)
        {
            if (c1.distanceTo(c2) < c1.radius + c2.radius)
                return true;
            else
                return false;
        }

        public bool contains(Vectors vect)
        {
            if (distanceTo(vect) < radius)
                return true;
            else
                return false;
        }

        public static bool areEqual(Circles c1, Circles c2)
        {
            if (Vectors.areEqual(c1.vertex, c2.vertex) && c1.radius == c2.radius)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This returns a position along a circle's arc given degrees.
        /// </summary>
        /// <param name="degrees">Degrees from the positive x axis</param>
        /// <returns></returns>
        public Vectors getArcPosition(float degrees)
        {
            Vectors temp = new Vectors();
            temp.setPolar(radius, degrees);
            return vertex + temp;
        }

        public float getArea()
        {
            return PI * radius * radius;
        }

        public float getCirc()
        {
            return PI * radius * 2;
        }

    }
}