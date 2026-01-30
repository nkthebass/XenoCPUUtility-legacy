using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace XenoCPUUtilityLegacy
{
    /// <summary>
    /// Path tracing benchmark - CPU-intensive ray tracing workload.
    /// Simplified version for .NET 4.0 compatibility (removed Vector3 dependency).
    /// </summary>
    public class PathTracerBenchmark
    {
        private const int MAX_BOUNCES = 6;
        private const float RAY_EPSILON = 0.001f;
        private const int SAMPLES_PER_PIXEL = 32;

        public struct Vector3
        {
            public float X, Y, Z;

            public Vector3(float x = 0, float y = 0, float z = 0)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            public static Vector3 operator *(Vector3 a, float s) => new Vector3(a.X * s, a.Y * s, a.Z * s);
            public static Vector3 operator *(float s, Vector3 a) => a * s;
            public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
            public static float Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            public static Vector3 Cross(Vector3 a, Vector3 b) => new Vector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
            public float Length() => (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            public Vector3 Normalized() { float len = Length(); return len > 0 ? new Vector3(X / len, Y / len, Z / len) : this; }
            public static Vector3 Zero => new Vector3(0, 0, 0);
        }

        public enum MaterialType { Diffuse, Metal, Glass }

        public struct Sphere
        {
            public Vector3 Center;
            public float Radius;
            public Vector3 Color;
            public bool IsLight;
            public float Emission;
            public MaterialType Material;
            public float Roughness;
            public float IOR;
        }

        public struct Plane
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector3 Color;
        }

        public struct Ray
        {
            public Vector3 Origin;
            public Vector3 Direction;
        }

        public struct HitRecord
        {
            public bool Hit;
            public float Distance;
            public Vector3 Point;
            public Vector3 Normal;
            public Vector3 Color;
            public bool IsLight;
            public float Emission;
            public MaterialType Material;
            public float Roughness;
            public float IOR;
        }

        private List<Sphere> spheres;
        private List<Plane> planes;
        private Random random = new Random(42);

        public PathTracerBenchmark()
        {
            InitializeScene();
        }

        private void InitializeScene()
        {
            spheres = new List<Sphere>
            {
                // Lights
                new Sphere { Center = new Vector3(0, 3.5f, 1), Radius = 0.8f, Color = new Vector3(1, 1, 1), IsLight = true, Emission = 2.5f, Material = MaterialType.Diffuse, Roughness = 1, IOR = 1 },
                new Sphere { Center = new Vector3(-2, 2.5f, 1), Radius = 0.3f, Color = new Vector3(1, 0.9f, 0.7f), IsLight = true, Emission = 1.0f, Material = MaterialType.Diffuse, Roughness = 1, IOR = 1 },
                
                // Glass sphere
                new Sphere { Center = new Vector3(0, 1.2f, 2.5f), Radius = 1.0f, Color = new Vector3(1, 1, 1), IsLight = false, Emission = 0, Material = MaterialType.Glass, Roughness = 0, IOR = 1.5f },
                
                // Metal spheres
                new Sphere { Center = new Vector3(-1.8f, 0.6f, 1.5f), Radius = 0.6f, Color = new Vector3(0.8f, 0.8f, 0.9f), IsLight = false, Emission = 0, Material = MaterialType.Metal, Roughness = 0.1f, IOR = 1 },
                new Sphere { Center = new Vector3(1.8f, 0.6f, 1.5f), Radius = 0.6f, Color = new Vector3(1, 0.84f, 0), IsLight = false, Emission = 0, Material = MaterialType.Metal, Roughness = 0.05f, IOR = 1 },
                new Sphere { Center = new Vector3(0, 0.5f, 0.8f), Radius = 0.5f, Color = new Vector3(0.9f, 0.9f, 1), IsLight = false, Emission = 0, Material = MaterialType.Glass, Roughness = 0.15f, IOR = 1.4f },
                
                // Colored spheres
                new Sphere { Center = new Vector3(-1, 1.5f, 3.5f), Radius = 0.4f, Color = new Vector3(1, 0.3f, 0.3f), IsLight = false, Emission = 0, Material = MaterialType.Diffuse, Roughness = 1, IOR = 1 },
                new Sphere { Center = new Vector3(1.5f, 1.8f, 4), Radius = 0.5f, Color = new Vector3(0.3f, 1, 0.3f), IsLight = false, Emission = 0, Material = MaterialType.Glass, Roughness = 0.1f, IOR = 1.5f },
                new Sphere { Center = new Vector3(-0.5f, 0.7f, 4.5f), Radius = 0.4f, Color = new Vector3(0.3f, 0.5f, 1), IsLight = false, Emission = 0, Material = MaterialType.Metal, Roughness = 0.3f, IOR = 1 }
            };

            planes = new List<Plane>
            {
                new Plane { Position = new Vector3(0, 0, 0), Normal = new Vector3(0, 1, 0), Color = new Vector3(0.75f, 0.75f, 0.75f) },
                new Plane { Position = new Vector3(0, 0, 6), Normal = new Vector3(0, 0, -1), Color = new Vector3(0.9f, 0.9f, 0.9f) },
                new Plane { Position = new Vector3(-4, 0, 0), Normal = new Vector3(1, 0, 0), Color = new Vector3(0.9f, 0.2f, 0.2f) },
                new Plane { Position = new Vector3(4, 0, 0), Normal = new Vector3(-1, 0, 0), Color = new Vector3(0.2f, 0.9f, 0.9f) },
                new Plane { Position = new Vector3(0, 4, 0), Normal = new Vector3(0, -1, 0), Color = new Vector3(0.85f, 0.85f, 0.85f) }
            };
        }

        public double RunBenchmark(int width = 320, int height = 240, int threadCount = -1)
        {
            if (threadCount <= 0)
                threadCount = Math.Max(1, Environment.ProcessorCount);

            Stopwatch sw = Stopwatch.StartNew();

            // Simple parallel path tracing
            int[] pixelCount = new int[1];
            object lockObj = new object();

            Parallel.For(0, height, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, y =>
            {
                Random threadRandom = new Random(y * 12345);
                for (int x = 0; x < width; x++)
                {
                    Vector3 color = Vector3.Zero;
                    for (int s = 0; s < SAMPLES_PER_PIXEL; s++)
                    {
                        float px = (x + (float)threadRandom.NextDouble()) / width;
                        float py = (y + (float)threadRandom.NextDouble()) / height;
                        
                        Vector3 rayDir = new Vector3(px - 0.5f, py - 0.5f, 1).Normalized();
                        Ray ray = new Ray { Origin = new Vector3(0, 2, -3), Direction = rayDir };
                        
                        color = color + TraceRay(ray, threadRandom);
                    }
                }

                lock (lockObj)
                {
                    pixelCount[0]++;
                }
            });

            sw.Stop();
            double pixelsPerSecond = (width * height) / (sw.Elapsed.TotalSeconds + 0.001);
            return pixelsPerSecond;
        }

        private Vector3 TraceRay(Ray ray, Random threadRandom)
        {
            Vector3 color = Vector3.Zero;
            Vector3 throughput = new Vector3(1, 1, 1);

            for (int bounce = 0; bounce < MAX_BOUNCES; bounce++)
            {
                HitRecord hit = TraceRaySimple(ray);
                if (!hit.Hit)
                    break;

                if (hit.IsLight)
                {
                    color = color + throughput * hit.Color * hit.Emission;
                    break;
                }

                // Simple material handling
                Vector3 newDir = RandomHemisphereDirection(hit.Normal, threadRandom);
                ray = new Ray { Origin = hit.Point + hit.Normal * RAY_EPSILON, Direction = newDir };
                throughput = throughput * hit.Color * 0.9f;
            }

            return color;
        }

        private HitRecord TraceRaySimple(Ray ray)
        {
            HitRecord closest = new HitRecord { Hit = false, Distance = float.MaxValue };

            // Test spheres
            foreach (var sphere in spheres)
            {
                HitRecord hit = RaySphereIntersect(ray, sphere);
                if (hit.Hit && hit.Distance < closest.Distance && hit.Distance > RAY_EPSILON)
                {
                    closest = hit;
                }
            }

            // Test planes
            foreach (var plane in planes)
            {
                HitRecord hit = RayPlaneIntersect(ray, plane);
                if (hit.Hit && hit.Distance < closest.Distance && hit.Distance > RAY_EPSILON)
                {
                    closest = hit;
                }
            }

            return closest;
        }

        private HitRecord RaySphereIntersect(Ray ray, Sphere sphere)
        {
            HitRecord result = new HitRecord { Hit = false };
            Vector3 oc = ray.Origin - sphere.Center;
            float a = Vector3.Dot(ray.Direction, ray.Direction);
            float b = 2.0f * Vector3.Dot(oc, ray.Direction);
            float c = Vector3.Dot(oc, oc) - sphere.Radius * sphere.Radius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0) return result;

            float t = (-b - (float)Math.Sqrt(discriminant)) / (2 * a);
            if (t <= RAY_EPSILON) t = (-b + (float)Math.Sqrt(discriminant)) / (2 * a);

            if (t > RAY_EPSILON)
            {
                result.Hit = true;
                result.Distance = t;
                result.Point = ray.Origin + ray.Direction * t;
                result.Normal = (result.Point - sphere.Center).Normalized();
                result.Color = sphere.Color;
                result.IsLight = sphere.IsLight;
                result.Emission = sphere.Emission;
                result.Material = sphere.Material;
                result.Roughness = sphere.Roughness;
                result.IOR = sphere.IOR;
            }

            return result;
        }

        private HitRecord RayPlaneIntersect(Ray ray, Plane plane)
        {
            HitRecord result = new HitRecord { Hit = false };
            float denom = Vector3.Dot(plane.Normal, ray.Direction);
            if (Math.Abs(denom) < 1e-6f) return result;

            float t = Vector3.Dot(plane.Position - ray.Origin, plane.Normal) / denom;
            if (t > RAY_EPSILON)
            {
                result.Hit = true;
                result.Distance = t;
                result.Point = ray.Origin + ray.Direction * t;
                result.Normal = plane.Normal;
                result.Color = plane.Color;
                result.IsLight = false;
                result.Emission = 0;
            }

            return result;
        }

        private Vector3 RandomHemisphereDirection(Vector3 normal, Random rand)
        {
            float theta = (float)(2.0 * Math.PI * rand.NextDouble());
            float u = (float)rand.NextDouble();
            float sinTheta = (float)Math.Sqrt(1 - u * u);
            
            Vector3 tangent = Math.Abs(normal.X) < 0.9f ? new Vector3(1, 0, 0) : new Vector3(0, 1, 0);
            Vector3 bitangent = Vector3.Cross(normal, tangent).Normalized();
            tangent = Vector3.Cross(bitangent, normal).Normalized();

            return tangent * ((float)Math.Cos(theta) * sinTheta) + bitangent * ((float)Math.Sin(theta) * sinTheta) + normal * u;
        }
    }
}
