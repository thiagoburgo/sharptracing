//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace DrawEngine.Renderer.Mesh
//{
//    public static class MeshContentOptimizer
//    {
//        public const int AssumedVertexCacheSize = 24;
//        public const float DefaultAmbientOcclusion = 0.875f;

//        public static void Apply(MeshContent content, MeshOptimizationFlags flags)
//        {
//            if (content.Vertices != null)
//            {
//                if (flags.IsSet(MeshOptimizationFlags.RemoveRedundantVertices))
//                {
//                    int redundant = RemoveRedundantVertices(content, flags);
//                    //Console.WriteLine("Remapped " + redundant + " redundant vertices");
//                }

//                if (flags.IsSet(MeshOptimizationFlags.RemoveDegenerateTriangles))
//                {
//                    int degenerate = RemoveDegenerateTriangles(content);
//                    //Console.WriteLine("Removed " + degenerate + " degenerate triangles; leaving " + (content.Indices.Length / 3));
//                }

//                int unused = RemoveUnusedVertices(content);
//                //Console.WriteLine("Removed " + unused + " unused vertices; leaving " + content.Vertices.Length);

//                if (flags.IsSet(MeshOptimizationFlags.OptimizeForVertexCache))
//                {
//                    //float cacheMisses = (float)CheckVertexCacheMisses(c_assumedVertexCacheSize, content) / (float)content.Indices.Length;
//                    Tipsify(content, flags.IsSet(MeshOptimizationFlags.ApproximateAmbientOcclusion));
//                    //XnaOptimizeForCache(content);
//                    //float newCacheMisses = (float)CheckVertexCacheMisses(c_assumedVertexCacheSize, content) / (float)content.Indices.Length;
//                    //Console.WriteLine("Cache misses reduced from " + (int)(cacheMisses * 100.0f) + "% to " + (int)(newCacheMisses * 100.0f) + "%");

//                    int removed = ReorderVertices(content);
//                    // Console.WriteLine("Reordered vertices for linear read and removed " + removed + " unused vertices");
//                }
//                else
//                {
//                    if (flags.IsSet(MeshOptimizationFlags.ApproximateAmbientOcclusion))
//                        ApproximateAmbientOcclusion(content);
//                }

//                if (!flags.IsSet(MeshOptimizationFlags.ApproximateAmbientOcclusion))
//                    SetAmbientOcclusion(content, DefaultAmbientOcclusion);

//                int fixedZeroNormals = FixZeroLengthNormals(content);
//                //Console.WriteLine("Fixed " + fixedZeroNormals + " normals");

//                content.GenerateBounds(flags);
//            }
//            else
//            {
//                content.LocalBoundingBox = BoundingBox.Zero;
//            }

//            return;
//        }

//        /// <summary>
//        /// Also normalizes all normals.
//        /// Returns number of zero length normals fixed
//        /// </summary>
//        public static int FixZeroLengthNormals(MeshContent content)
//        {
//            MeshVertex[] vertices = content.Vertices;
//            int[] indices = content.Indices;
//            int retval = 0;

//            bool[] vertexFixNeeded = new bool[vertices.Length];
//            Vector3[] faceNormals = new Vector3[indices.Length / 3];
//            for (int i = 0; i < vertices.Length; i++)
//            {
//                float lenSqrd = vertices[i].Normal.LengthSquared();
//                if (lenSqrd == 0.0f)
//                {
//                    vertexFixNeeded[i] = true;
//                }
//                else if (Math.Abs(lenSqrd - 1.0f) > 0.001f)
//                {
//                    vertices[i].Normal.Normalize();
//                }
//            }

//            for (int i = 0; i < indices.Length; i += 3)
//            {
//                int i1 = indices[i];
//                int i2 = indices[i + 1];
//                int i3 = indices[i + 2];

//                if (!vertexFixNeeded[i1] && !vertexFixNeeded[i2] && !vertexFixNeeded[i3])
//                    continue;

//                Vector3 p1 = vertices[i1].Position;
//                Vector3 p2 = vertices[i2].Position;
//                Vector3 p3 = vertices[i3].Position;

//                Vector3 faceNormal = Vector3.NormalFrom(ref p1, ref p2, ref p3);

//                if (vertexFixNeeded[i1])
//                {
//                    MeshVertex v = vertices[i1];
//                    v.Normal = faceNormal;
//                    vertices[i1] = v;
//                    retval++;
//                }

//                if (vertexFixNeeded[i2])
//                {
//                    MeshVertex v = vertices[i2];
//                    v.Normal = faceNormal;
//                    vertices[i2] = v;
//                    retval++;
//                }

//                if (vertexFixNeeded[i3])
//                {
//                    MeshVertex v = vertices[i3];
//                    v.Normal = faceNormal;
//                    vertices[i3] = v;
//                    retval++;
//                }
//            }

//            return retval;
//        }

//        /// <summary>
//        /// Returns number of unused vertices removed
//        /// </summary>
//        public static int RemoveUnusedVertices(MeshContent content)
//        {
//            int wasNumVertices = content.Vertices.Length;

//            // determine which vertices are used
//            bool[] used = new bool[wasNumVertices];
//            int numUsed = 0;
//            foreach (int idx in content.Indices)
//            {
//                if (used[idx] == false)
//                {
//                    numUsed++;
//                    used[idx] = true;
//                }
//            }

//            if (numUsed == content.Vertices.Length)
//                return 0; // no unused vertices

//            // copy to new vertices
//            MeshVertex[] newVertices = new MeshVertex[numUsed];
//            int ptr = 0;
//            int[] map = new int[content.Vertices.Length];
//            for (int i = 0; i < content.Vertices.Length; i++)
//            {
//                if (used[i])
//                {
//                    map[i] = ptr;
//                    newVertices[ptr++] = content.Vertices[i];
//                }
//            }
//            content.Vertices = newVertices;

//            // update indices
//            for (int i = 0; i < content.Indices.Length; i++)
//                content.Indices[i] = map[content.Indices[i]];

//            return (wasNumVertices - content.Vertices.Length);
//        }

//        /// <summary>
//        /// Returns number of redundant vertices removed
//        /// </summary>
//        public static int RemoveRedundantVertices(MeshContent content, MeshOptimizationFlags flags)
//        {
//            MeshVertex[] contentVertices = content.Vertices;
//            int[] contentIndices = content.Indices;

//            Dictionary<long, int> existing = new Dictionary<long, int>(contentVertices.Length);
//            int[] map = new int[contentVertices.Length];

//            int numWasRemapped = 0;

//            for (int i = 0; i < contentVertices.Length; i++)
//            {
//                MeshVertex v = contentVertices[i];

//                long hash = v.GetLongHashCode();

//                int idx;
//                if (existing.TryGetValue(hash, out idx))
//                {
//                    numWasRemapped++;
//                    map[i] = idx;
//                    continue;
//                }

//                map[i] = i;
//                existing[hash] = i;
//            }

//            // map is now set up
//            existing = null;

//            for (int i = 0; i < contentIndices.Length; i++)
//            {
//                int idx = contentIndices[i];
//                int mapsTo = map[idx];
//                if (idx == mapsTo)
//                    continue;
//                contentIndices[i] = mapsTo;
//            }

//            return numWasRemapped;
//        }


//        /// <summary>
//        /// Returns number of degenerate triangles removed
//        /// </summary>
//        public static int RemoveDegenerateTriangles(MeshContent content)
//        {
//            double start = GlobalServices.TimeService.Now;
//            List<int> newIndices = new List<int>(content.Indices.Length);
//            for (int i = 0; i < content.Indices.Length; i += 3)
//            {
//                if (content.Indices[i] == content.Indices[i + 1] ||
//                        content.Indices[i] == content.Indices[i + 2] ||
//                        content.Indices[i + 1] == content.Indices[i + 2])
//                {
//                    // degenerate, remove it
//                    continue;
//                }

//                // add it
//                newIndices.Add(content.Indices[i]);
//                newIndices.Add(content.Indices[i + 1]);
//                newIndices.Add(content.Indices[i + 2]);
//            }

//            int ms = (int)((GlobalServices.TimeService.Now - start) * 1000.0);
//            int numRemovedFaces = ((content.Indices.Length - newIndices.Count) / 3);
//            if (numRemovedFaces > 0)
//                content.Indices = newIndices.ToArray();
//            return numRemovedFaces;
//        }

//        /// <summary>
//        /// Unused
//        /// </summary>
//        public static void CalculateNormals(MeshContent content)
//        {
//            int i, o;
//            for (i = 0; i < content.Vertices.Length; i++)
//            {
//                Vector3 normal;
//                normal.X = 0;
//                normal.Y = 0;
//                normal.Z = 0;
//                int numAdds = 0;

//                // determine which triangle(s) uses this vertex
//                for (o = 0; o < content.Indices.Length; o++)
//                {
//                    if (content.Indices[o] == i)
//                    {
//                        // which triangle is this?
//                        int triIdx = o / 3;

//                        Vector3 p1 = content.Vertices[triIdx * 3].Position;
//                        Vector3 p2 = content.Vertices[triIdx * 3 + 1].Position;
//                        Vector3 p3 = content.Vertices[triIdx * 3 + 2].Position;
//                        normal += Vector3.NormalFrom(ref p1, ref p2, ref p3);
//                        numAdds++;
//                    }
//                }

//                if (numAdds > 1)
//                    normal *= (1.0f / numAdds);

//                content.Vertices[i].Normal = normal;
//            }
//        }

//        /// <summary>
//        /// Reorders vertices for linear access and removed unused vertices; returns number of vertices removed
//        /// </summary>
//        public static int ReorderVertices(MeshContent content)
//        {
//            List<MeshVertex> newVertices = new List<MeshVertex>(content.Vertices.Length);
//            int[] oldToNew = new int[content.Vertices.Length];
//            int[] indices = content.Indices;

//            for (int i = 0; i < oldToNew.Length; i++)
//                oldToNew[i] = -1;

//            for (int i = 0; i < indices.Length; i++)
//            {
//                int idx = indices[i];
//                int np = oldToNew[idx];
//                if (np == -1)
//                {
//                    // not yet written
//                    int ptr = newVertices.Count;
//                    oldToNew[idx] = ptr;
//                    np = ptr;
//                    newVertices.Add(content.Vertices[idx]);
//                }
//                indices[i] = np;
//            }

//            int removed = content.Vertices.Length - newVertices.Count;

//            // swap vertices
//            content.Vertices = newVertices.ToArray();

//            return removed;
//        }

//        public static string SetDefaultAmbientOcclusion(MeshContent content)
//        {
//            return SetAmbientOcclusion(content, DefaultAmbientOcclusion);
//        }

//        public static string SetAmbientOcclusion(MeshContent content, float ambientOcclusion)
//        {
//            for (int i = 0; i < content.Vertices.Length; i++)
//                content.Vertices[i].AmbientOcclusion = ambientOcclusion;
//            return "";
//        }

//        public static void CalculateAmbientOcclusion2(MeshContent content)
//        {
//            double startTime = GlobalServices.TimeService.Now;

//            float rayBegin = content.LocalBoundingSphere.Radius * 2;

//            MeshContent tmpRayContent = MeshContentGenerator.CreateUnitSphere(2);

//            int[] triHits = new int[content.Indices.Length / 3];

//            foreach (MeshVertex mv in tmpRayContent.Vertices)
//            {
//                Vector3 nrm = mv.Position;
//                nrm.Normalize();

//                Ray ray = new Ray(nrm * rayBegin, -nrm);

//                int tri;
//                float dist;
//                if (content.Intersect(ray, out dist, out tri))
//                    triHits[tri]++;
//            }

//            SetAmbientOcclusion(content, 0.0f);

//            for (int i = 0; i < triHits.Length; i++)
//            {
//                int hits = triHits[i];
//                if (hits <= 0)
//                    continue;

//                int idx1 = content.Indices[i];
//                int idx2 = content.Indices[i + 1];
//                int idx3 = content.Indices[i + 2];

//                content.Vertices[idx1].AmbientOcclusion += (0.25f * (float)hits);
//                content.Vertices[idx2].AmbientOcclusion += (0.25f * (float)hits);
//                content.Vertices[idx3].AmbientOcclusion += (0.25f * (float)hits);
//            }
//        }

//        private static object m_rayCreationLockObject = new object();
//        private static Vector3[] m_tmpRays;
//        private static Vector3[] m_tmpRaysOffset;
//        public static void CalculateAmbientOcclusion(MeshContent content)
//        {
//            double startTime = GlobalServices.TimeService.Now;

//            if (m_tmpRays == null)
//            {
//                lock (m_rayCreationLockObject)
//                {
//                    if (m_tmpRays == null)
//                    {
//                        MeshContent tmpRayContent = MeshContentGenerator.CreateUnitSphere(1);
//                        m_tmpRays = new Vector3[tmpRayContent.Vertices.Length];
//                        m_tmpRaysOffset = new Vector3[tmpRayContent.Vertices.Length];

//                        for (int i = 0; i < m_tmpRays.Length; i++)
//                        {
//                            m_tmpRays[i] = tmpRayContent.Vertices[i].Position;
//                            m_tmpRays[i].Normalize();
//                            m_tmpRaysOffset[i] = m_tmpRays[i] * 0.000000001f;
//                        }
//                        tmpRayContent = null;
//                    }
//                }
//            }

//            //if (false)
//            //{
//            //      // Run several threads
//            //      Parallel<MeshVertex> p = new Parallel<MeshVertex>();
//            //      p.Run(m_tmpContent.Vertices, CalculateAORange);
//            //}
//            //else
//            //{

//            // Old singlethreaded code
//            float maxDist = (content.LocalBoundingSphere.Radius * 2);
//            float totalDist = maxDist * m_tmpRays.Length;

//            Dictionary<int, float> store = new Dictionary<int, float>(content.Vertices.Length);

//            for (int i = 0; i < content.Vertices.Length; i++)
//            {
//                MeshVertex vert = content.Vertices[i];

//                int hash = vert.Position.X.GetHashCode() ^ vert.Position.Y.GetHashCode() ^ vert.Position.Z.GetHashCode();

//                float tmp;
//                if (store.TryGetValue(hash, out tmp))
//                {
//                    content.Vertices[i].AmbientOcclusion = tmp;
//                    continue;
//                }

//                // send rays
//                float accumulatedDist = 0;
//                for (int r = 0; r < m_tmpRays.Length; r++)
//                {
//                    Ray ray;
//                    ray.Position.X = vert.Position.X - m_tmpRaysOffset[r].X;
//                    ray.Position.Y = vert.Position.Y - m_tmpRaysOffset[r].Y;
//                    ray.Position.Z = vert.Position.Z - m_tmpRaysOffset[r].Z;
//                    ray.Direction = m_tmpRays[r];

//                    float dist;
//                    int triIdx;
//                    if (content.Intersect(ray, out dist, out triIdx))
//                    {
//                        //if (dist > maxDist || dist < 0.0f)
//                        //      Console.WriteLine("Ouch; maxdist is " + maxDist + " dist is " + dist);
//                        accumulatedDist += dist;
//                    }
//                    else
//                    {
//                        accumulatedDist += maxDist;
//                    }
//                }

//                float ao = accumulatedDist / totalDist;
//                store[hash] = ao;

//                content.Vertices[i].AmbientOcclusion = ao;

//                //if (i % 100 == 0)
//                //      Console.WriteLine("AO: " + i + " of " + content.Vertices.Length);
//            }

//            return;
//        }

//        /// <summary>
//        /// Count number of vertex cache misses
//        /// </summary>
//        public static int CheckVertexCacheMisses(int cacheSize, MeshContent content)
//        {
//            int[] cache = new int[cacheSize];
//            for (int i = 0; i < cache.Length; i++)
//                cache[i] = -1;

//            int misses = 0;
//            int cacheWritePtr = 0;

//            for (int a = 0; a < content.Indices.Length; a += 3)
//            {
//                int v1 = content.Indices[a];

//                int i;
//                for (i = 0; i < cache.Length; i++) { if (v1 == cache[i]) break; }
//                if (i >= cache.Length)
//                {
//                    // miss
//                    misses++;
//                    cache[cacheWritePtr++] = v1;
//                    if (cacheWritePtr >= cache.Length)
//                        cacheWritePtr = 0;
//                }

//                v1 = content.Indices[a + 1];
//                for (i = 0; i < cache.Length; i++) { if (v1 == cache[i]) break; }
//                if (i >= cache.Length)
//                {
//                    // miss
//                    misses++;
//                    cache[cacheWritePtr++] = v1;
//                    if (cacheWritePtr >= cache.Length)
//                        cacheWritePtr = 0;
//                }

//                v1 = content.Indices[a + 2];
//                for (i = 0; i < cache.Length; i++) { if (v1 == cache[i]) break; }
//                if (i >= cache.Length)
//                {
//                    // miss
//                    misses++;
//                    cache[cacheWritePtr++] = v1;
//                    if (cacheWritePtr >= cache.Length)
//                        cacheWritePtr = 0;
//                }
//            }

//            return misses;
//        }

//        /*
//        public static void XnaOptimizeForCache(MeshContent content)
//        {
//                Microsoft.Xna.Framework.Content.Pipeline.Graphics.MeshContent xnaContent = new Microsoft.Xna.Framework.Content.Pipeline.Graphics.MeshContent();
//                Microsoft.Xna.Framework.Content.Pipeline.Graphics.GeometryContent gcnt = new Microsoft.Xna.Framework.Content.Pipeline.Graphics.GeometryContent();
//                xnaContent.Geometry.Add(gcnt);

//                gcnt.Indices.AddRange(content.Indices);
//                for (int i = 0; i < content.Vertices.Length; i++)
//                        gcnt.Vertices.Add(i);

//                Microsoft.Xna.Framework.Content.Pipeline.Graphics.MeshHelper.OptimizeForCache(xnaContent);

//                if (content.Indices.Length != gcnt.Indices.Count)
//                        throw new Exception("ouch");

//                // copy indices back to content
//                for (int i = 0; i < gcnt.Indices.Count; i++)
//                        content.Indices[i] = gcnt.Indices[i];

//                // reorder vertices according to xnacontent
//                MeshVertex[] newVertices = new MeshVertex[content.Vertices.Length];
//                for (int i = 0; i < newVertices.Length; i++)
//                        newVertices[i] = content.Vertices[gcnt.Vertices.PositionIndices[i]];
//                content.Vertices = newVertices;

//                return;
//        }
//        */

//        private struct TriangleCluster
//        {
//            public int IndexStart;
//            public int IndexCount;
//            public float Occlusion;
//            public TriangleCluster(int idxStart, int idxCount, float occlusion)
//            {
//                IndexStart = idxStart;
//                IndexCount = idxCount;
//                Occlusion = occlusion;
//            }
//        }

//        private static MergeSorter<TriangleCluster> s_clusterSorter = new MergeSorter<TriangleCluster>();
//        private static TriangleClusterComparer s_clusterComparer = new TriangleClusterComparer();

//        private class TriangleClusterComparer : IComparer<TriangleCluster>
//        {
//            public int Compare(TriangleCluster x, TriangleCluster y)
//            {
//                if (x.Occlusion == y.Occlusion)
//                    return 0;
//                if (x.Occlusion > y.Occlusion)
//                    return -1;
//                return 1;
//            }
//        }

//        public static void Tipsify(MeshContent content, bool doApproximateAmbientOcclusion)
//        {
//            int[] indices = content.Indices;
//            int numVertices = content.Vertices.Length;

//            //
//            // Generate triangle adjacency
//            //
//            List<int>[] trisForVertex = new List<int>[numVertices];
//            for (int i = 0; i < trisForVertex.Length; i++)
//                trisForVertex[i] = new List<int>(6);

//            int numTriangles = indices.Length / 3;

//            int[] livecount = new int[numVertices];

//            List<int> clusterBoundary = new List<int>();

//            int av;
//            int t = 0;
//            for (int i = 0; i < indices.Length; t++)
//            {
//                av = indices[i++];
//                trisForVertex[av].Add(t);
//                livecount[av]++;
//                av = indices[i++];
//                trisForVertex[av].Add(t);
//                livecount[av]++;
//                av = indices[i++];
//                trisForVertex[av].Add(t);
//                livecount[av]++;
//            }

//            int[] timestamps = new int[numVertices];

//            //
//            // main loop
//            //
//            int[] output = new int[indices.Length];
//            int outptr = 0;
//            Stack<int> deadend = new Stack<int>();
//            bool[] emitted = new bool[numTriangles];
//            int f = 0;
//            int s = AssumedVertexCacheSize + 1;
//            int j = 1;

//            List<int> candidates = new List<int>(16);
//            while (f >= 0)
//            {
//                foreach (int tri in trisForVertex[f])
//                {
//                    if (!emitted[tri])
//                    {
//                        int idxbase = tri * 3;

//                        // for every vertex in triangle t
//                        int v = indices[idxbase++];
//                        output[outptr++] = v;
//                        deadend.Push(v);
//                        candidates.Add(v);
//                        livecount[v]--;
//                        if (s - timestamps[v] > AssumedVertexCacheSize) // if not in cache
//                            timestamps[v] = s++;

//                        v = indices[idxbase++];
//                        output[outptr++] = v;
//                        deadend.Push(v);
//                        candidates.Add(v);
//                        livecount[v]--;
//                        if (s - timestamps[v] > AssumedVertexCacheSize) // if not in cache
//                            timestamps[v] = s++;

//                        v = indices[idxbase];
//                        output[outptr++] = v;
//                        deadend.Push(v);
//                        candidates.Add(v);
//                        livecount[v]--;
//                        if (s - timestamps[v] > AssumedVertexCacheSize) // if not in cache
//                            timestamps[v] = s++;

//                        emitted[tri] = true;
//                    }
//                }

//                //f = Get-Next-Vertex(I,i,k,N,C,s,L,D)
//                f = -1;
//                int m = 0;
//                foreach (int vn in candidates)
//                {
//                    if (livecount[vn] > 0)
//                    {
//                        int p = 0; // initial priority
//                        if (s - timestamps[vn] + 2 * livecount[vn] <= AssumedVertexCacheSize) // in cache even after fanning?
//                            p = s - timestamps[vn]; // Priority is position in cache
//                        if (p > m) // Keep best candidate
//                        {
//                            m = p;
//                            f = vn;
//                        }
//                    }
//                }

//                if (f == -1) // reached a dead end?
//                {
//                    clusterBoundary.Add(outptr);
//                    f = SkipDeadEnd(numVertices, deadend, livecount, ref j);
//                }

//                candidates.Clear();
//            }

//            // calculate model centroid
//            Vector3 modelCentroid = Vector3.Zero;
//            for (int i = 0; i < content.Vertices.Length; i++)
//                modelCentroid += content.Vertices[i].Position;
//            modelCentroid *= (1.0f / content.Vertices.Length);

//            float[] occlusionSum = null;
//            int[] occlusionCount = null;

//            if (doApproximateAmbientOcclusion)
//            {
//                occlusionCount = new int[content.Vertices.Length];
//                occlusionSum = new float[content.Vertices.Length];
//            }

//            // average normals for each cluster
//            TriangleCluster[] clusters = new TriangleCluster[clusterBoundary.Count];
//            int currentCluster = 0;
//            Vector3 normalSum = Vector3.Zero;
//            Vector3 positionSum = Vector3.Zero;

//            for (int i = 0; i < output.Length; i += 3)
//            {
//                if (i >= clusterBoundary[currentCluster])
//                {
//                    // grade cluster
//                    int clusterStart = (currentCluster == 0 ? 0 : clusterBoundary[currentCluster - 1]);
//                    int clusterIndexCount = (clusterBoundary[currentCluster] - clusterStart);
//                    int clusterFaceCount = clusterIndexCount / 3;

//                    Vector3 clusterCenter = positionSum * (1.0f / (float)clusterIndexCount);
//                    Vector3 averageNormal = Vector3.Normalize(normalSum);
//                    Vector3 a = clusterCenter - modelCentroid;
//                    a.Normalize();
//                    float occlusion = Vector3.Dot(averageNormal, a);
//                    occlusion += 1.0f;
//                    occlusion *= 0.5f;
//                    if (occlusion > 1.0f)
//                        occlusion = 1.0f;

//                    clusters[currentCluster++] = new TriangleCluster(clusterStart, clusterIndexCount, occlusion);

//                    positionSum = Vector3.Zero;
//                    normalSum = Vector3.Zero;
//                }

//                int idx1 = output[i];
//                int idx2 = output[i + 1];
//                int idx3 = output[i + 2];

//                Vector3 p1 = content.Vertices[idx1].Position;
//                Vector3 p2 = content.Vertices[idx2].Position;
//                Vector3 p3 = content.Vertices[idx3].Position;

//                positionSum = positionSum + p1 + p2 + p3;

//                Vector3 nrm = Vector3.NormalFrom(p1, p2, p3);
//                normalSum += nrm;

//                if (doApproximateAmbientOcclusion)
//                {
//                    Vector3 ia = p1 - modelCentroid;
//                    ia.Normalize();

//                    float iocclusion = ((Vector3.Dot(nrm, ia) + 1.25f) * 0.5f);
//                    if (iocclusion > 1.0f)
//                        iocclusion = 1.0f;

//                    occlusionSum[idx1] += iocclusion;
//                    occlusionCount[idx1]++;

//                    occlusionSum[idx2] += iocclusion;
//                    occlusionCount[idx2]++;

//                    occlusionSum[idx3] += iocclusion;
//                    occlusionCount[idx3]++;
//                }
//            }

//            // sort clusters
//            s_clusterSorter.Sort(clusters, s_clusterComparer);

//            // reorder by cluster
//            int ptr = 0;
//            foreach (TriangleCluster cluster in clusters)
//            {
//                for (int i = 0; i < cluster.IndexCount; i++)
//                    indices[ptr++] = output[cluster.IndexStart + i];
//            }
//            //content.Indices = output;

//            if (doApproximateAmbientOcclusion)
//            {
//                for (int i = 0; i < content.Vertices.Length; i++)
//                {
//                    float ao = 0.35f + (occlusionSum[i] / (float)occlusionCount[i]);
//                    if (ao > 1.0f)
//                        ao = 1.0f;
//                    content.Vertices[i].AmbientOcclusion = ao;
//                }
//            }

//            return;
//        }

//        private static int SkipDeadEnd(int numVertices, Stack<int> deadend, int[] livecount, ref int j)
//        {
//            while (deadend.Count > 0)
//            {
//                int d = deadend.Pop();
//                if (livecount[d] > 0)
//                    return d;
//            }

//            while (j < numVertices)
//            {
//                if (livecount[j] > 0)
//                    return j;
//                j++; // cursor sweeps list only once
//            }

//            return -1; // done!
//        }

//        public static void ApproximateAmbientOcclusion(MeshContent content)
//        {
//            // calculate model centroid
//            Vector3 modelCentroid = Vector3.Zero;
//            for (int i = 0; i < content.Vertices.Length; i++)
//                modelCentroid += content.Vertices[i].Position;
//            modelCentroid *= (1.0f / content.Vertices.Length);

//            int[] indices = content.Indices;

//            float[] occlusionSum = new float[content.Vertices.Length];
//            int[] occlusionCount = new int[content.Vertices.Length];

//            // calculate normal per triangle
//            for (int i = 0; i < indices.Length; i += 3)
//            {
//                Vector3 p1 = content.Vertices[indices[i]].Position;
//                Vector3 p2 = content.Vertices[indices[i + 1]].Position;
//                Vector3 p3 = content.Vertices[indices[i + 2]].Position;

//                Vector3 nrm = Vector3.NormalFrom(p1, p2, p3);

//                Vector3 a = modelCentroid - p1;
//                a.Normalize();

//                float occlusion = (Vector3.Dot(a, nrm) + 1) * 0.5f;
//                //occlusion = Math.Max(0, occlusion);

//                // color all vertices
//                int idx1 = indices[i];
//                int idx2 = indices[i + 1];
//                int idx3 = indices[i + 2];

//                occlusionSum[idx1] += occlusion;
//                occlusionCount[idx1]++;

//                occlusionSum[idx2] += occlusion;
//                occlusionCount[idx2]++;

//                occlusionSum[idx3] += occlusion;
//                occlusionCount[idx3]++;
//            }

//            for (int i = 0; i < content.Vertices.Length; i++)
//                content.Vertices[i].AmbientOcclusion = occlusionSum[i] / (float)occlusionCount[i];

//            return;
//        }
//    }
//}

