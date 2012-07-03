/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing é um projeto feito inicialmente para disciplina
 * Computação Gráfica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar correções e 
 * sugestões. Mantenha os créditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */
using System;
using System.Diagnostics;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using System.Collections.Generic;

namespace DrawEngine.Renderer.PhotonMapping
{
    public sealed class PhotonMap
    {
        private readonly double[] cosPhi = new double[256];
        private readonly double[] cosTheta = new double[256];
        private readonly double[] sinPhi = new double[256];
        private readonly double[] sinTheta = new double[256];

        private readonly int max_photons;
        public readonly Photon[] Photons;
        private Point3D bbox_max; // use bbox_max;
        private Point3D bbox_min; // use bbox_min;
        private int half_stored_photons;
        private int prev_scale;
        private int stored_photons;
        //OK
        public PhotonMap(int max_photons)
        {
            this.max_photons = max_photons;
            this.stored_photons = 0;
            this.prev_scale = 1;
            this.Photons = new Photon[max_photons + 1];
            this.bbox_min.X = this.bbox_min.Y = this.bbox_min.Z = double.PositiveInfinity;
            this.bbox_max.X = this.bbox_max.Y = this.bbox_max.Z = double.NegativeInfinity;
            //----------------------------------------
            // initialize direction conversion tables
            //----------------------------------------
            for (int i = 0; i < 256; i++)
            {
                double angle = i * (1.0 / 256.0) * Math.PI;
                this.cosTheta[i] = Math.Cos(angle);
                this.sinTheta[i] = Math.Sin(angle);
                this.cosPhi[i] = Math.Cos(2.0 * angle);
                this.sinPhi[i] = Math.Sin(2.0 * angle);
            }
        }
        private Vector3D photonDir(Photon p)
        {
            Vector3D pDir = new Vector3D(this.sinTheta[p.Theta] * this.cosPhi[p.Phi],
                                         this.sinTheta[p.Theta] * this.sinPhi[p.Phi],
                                         this.cosTheta[p.Theta]);
            return pDir;
        }
        //OK
        //irradiance_estimate computes an irradiance estimate
        // at a given surface position
        public RGBColor IrradianceEstimate(Point3D pos, Vector3D normal, double max_dist, int nphotons)
        {
            RGBColor irrad = RGBColor.Black;
            NearestPhotons np = new NearestPhotons();
            np.Dist2 = new double[nphotons + 1];
            np.Index = new Photon[nphotons + 1];
            np.Pos = pos;
            np.Max = nphotons;
            np.Found = 0;
            np.GotHeap = 0;
            np.Dist2[0] = max_dist * max_dist;
            // locate the nearest photons
            this.LocatePhotons(np, 1);
            // if less than 8 photons return
            if (np.Found < 8)
            {
                return irrad;
            }
            Vector3D pdir;
            // sum irradiance from all photons
            for (int i = 1; i <= np.Found; i++)
            {
                Photon p = np.Index[i];
                // the photon_dir call and following if can be omitted (for speed)
                // if the scene does not have any thin surfaces
                pdir = this.photonDir(p);
                if ((pdir * normal) < 0.0d)
                {
                    irrad += p.Power;
                }
            }
            double tmp = ((1.0d / Math.PI) / (np.Dist2[0])); // estimate of
            // density
            irrad *= tmp;
            //Use Array.Clear();
            np.Dist2 = null;
            np.Index = null;
            return irrad;
        }
        //OK
        // locate_photons finds the nearest photons in the
        // photon map given the parameters in np
        private void LocatePhotons(NearestPhotons np, int index)
        {
            Photon p = this.Photons[index];
            if (index < this.half_stored_photons)
            {
                double dist1 = np.Pos[p.Plane] - p.Position[p.Plane];
                if (dist1 > 0.0)
                {
                    // if dist1 is positive search right plane
                    this.LocatePhotons(np, 2 * index + 1);
                    if (dist1 * dist1 < np.Dist2[0])
                    {
                        this.LocatePhotons(np, 2 * index);
                    }
                }
                else
                {
                    // dist1 is negative search left first
                    this.LocatePhotons(np, 2 * index);
                    if (dist1 * dist1 < np.Dist2[0])
                    {
                        this.LocatePhotons(np, 2 * index + 1);
                    }
                }
            }
            // compute squared distance between current photon and np.pos
            //dist1 = p.pos[0] - np.pos[0];
            //double dist2 = dist1 * dist1;
            //dist1 = p.pos[1] - np.pos[1];
            //dist2 += dist1 * dist1;
            //dist1 = p.pos[2] - np.pos[2];
            //dist2 += dist1 * dist1;
            double dist2 = (p.Position - np.Pos).Length2;
            if (dist2 < np.Dist2[0])
            {
                // we found a photon :) Insert it in the candidate list
                if (np.Found < np.Max)
                {
                    // heap is not full; use array
                    np.Found++;
                    np.Dist2[np.Found] = dist2;
                    np.Index[np.Found] = p;
                }
                else
                {
                    int j, parent;
                    if (np.GotHeap == 0)
                    {
                        // Do we need to build the heap?
                        // Build heap
                        double dst2;
                        Photon phot;
                        int half_found = np.Found >> 1;
                        for (int k = half_found; k >= 1; k--)
                        {
                            parent = k;
                            phot = np.Index[k];
                            dst2 = np.Dist2[k];
                            while (parent <= half_found)
                            {
                                j = parent + parent;
                                if (j < np.Found && np.Dist2[j] < np.Dist2[j + 1])
                                {
                                    j++;
                                }
                                if (dst2 >= np.Dist2[j])
                                {
                                    break;
                                }
                                np.Dist2[parent] = np.Dist2[j];
                                np.Index[parent] = np.Index[j];
                                parent = j;
                            }
                            np.Dist2[parent] = dst2;
                            np.Index[parent] = phot;
                        }
                        np.GotHeap = 1;
                    }
                    // insert new photon into max heap
                    // delete largest element, insert new and reorder the heap
                    parent = 1;
                    j = 2;
                    while (j <= np.Found)
                    {
                        if (j < np.Found && np.Dist2[j] < np.Dist2[j + 1])
                        {
                            j++;
                        }
                        if (dist2 > np.Dist2[j])
                        {
                            break;
                        }
                        np.Dist2[parent] = np.Dist2[j];
                        np.Index[parent] = np.Index[j];
                        parent = j;
                        j += j;
                    }
                    if (dist2 < np.Dist2[parent])
                    {
                        np.Index[parent] = p;
                        np.Dist2[parent] = dist2;
                    }
                    np.Dist2[0] = np.Dist2[1];
                }
            }
        }
        //OK
        // store puts a photon into the flat array that will form
        // the final kd-tree.
        // Call this function to store a photon.
        public void Store(Photon photon)
        {
            if (this.stored_photons >= this.max_photons)
            {
                return;
            }
            this.stored_photons++;
            this.Photons[this.stored_photons] = photon;
            photon.Index = this.stored_photons;
            if (photon.Position.X < this.bbox_min.X)
            {
                this.bbox_min.X = photon.Position.X;
            }
            if (photon.Position.X > this.bbox_max.X)
            {
                this.bbox_max.X = photon.Position.X;
            }
            if (photon.Position.Y < this.bbox_min.Y)
            {
                this.bbox_min.Y = photon.Position.Y;
            }
            if (photon.Position.Y > this.bbox_max.Y)
            {
                this.bbox_max.Y = photon.Position.Y;
            }
            if (photon.Position.Z < this.bbox_min.Z)
            {
                this.bbox_min.Z = photon.Position.Z;
            }
            if (photon.Position.Z > this.bbox_max.Z)
            {
                this.bbox_max.Z = photon.Position.Z;
            }
        }
        //public void Store(RGBColor power, Point3D pos, Vector3D dir)
        //{
        //    if(this.stored_photons >= this.max_photons){
        //        return;
        //    }
        //    this.stored_photons++;
        //    Photon node = this.photons[this.stored_photons]  = new Photon();
        //    node.Position = pos;
        //    if(node.Position.X < this.bbox_min.X){
        //        this.bbox_min.X = node.Position.X;
        //    }
        //    if(node.Position.X > this.bbox_max.X){
        //        this.bbox_max.X = node.Position.X;
        //    }
        //    if(node.Position.Y < this.bbox_min.Y){
        //        this.bbox_min.Y = node.Position.Y;
        //    }
        //    if(node.Position.Y > this.bbox_max.Y){
        //        this.bbox_max.Y = node.Position.Y;
        //    }
        //    if(node.Position.Z < this.bbox_min.Z){
        //        this.bbox_min.Z = node.Position.Z;
        //    }
        //    if(node.Position.Z > this.bbox_max.Z){
        //        this.bbox_max.Z = node.Position.Z;
        //    }
        //    node.Power = power;
        //    int theta = (int)(Math.Acos(dir[2]) * (256.0 / Math.PI));
        //    if(theta > 255){
        //        node.Theta = 255;
        //    } else{
        //        node.Theta = (byte)theta;
        //    }
        //    int phi = (int)(Math.Atan2(dir[1], dir[0]) * (256.0 / (2.0 * Math.PI)));
        //    if(phi > 255){
        //        node.Phi = 255;
        //    } else if(phi < 0){
        //        node.Phi = (byte)(phi + 256);
        //    } else{
        //        node.Phi = (byte)phi;
        //    }
        //}
        // scale_photon_power is used to scale the power of all
        // photons once they have been emitted from the light
        // source. scale = 1/(#emitted photons).
        // Call this function after each light source is processed.
        public void ScalePhotonPower(double scale)
        {
            for (int i = this.prev_scale; i <= this.stored_photons; i++)
            {
                this.Photons[i].Power *= scale;
            }
            this.prev_scale = this.stored_photons + 1;
        }
        //balance creates a left balanced kd-tree from the flat photon array.
        //This function should be called before the photon map
        //is used for rendering.
        public void Balance()
        {
            if (this.stored_photons > 0)
            {
                // allocate two temporary arrays for the balancing procedure
                Photon[] pa1 = new Photon[this.stored_photons + 1];
                Photon[] pa2 = new Photon[this.stored_photons + 1];
                int i;
                for (i = 1; i <= this.stored_photons; i++)
                {
                    pa2[i] = this.Photons[i];
                }
                //this.Photons.CopyTo(pa2, 0);

                this.BalanceSegment(pa1, pa2, 1, 1, this.stored_photons);
                //Array.Clear(pa2, 0, pa2.Length); 
                pa2 = null;
                // reorganize balanced kd-tree (make a heap)
                int d, j = 1, foo = 1;
                Photon foo_photon = this.Photons[j];
                for (i = 1; i <= this.stored_photons; i++)
                {
                    //d = Array.IndexOf(this.Photons, pa1[j]); //TODO: hack, linear search =(
                    d = pa1[j].Index;//OK Eliminate linear search
                    //Debug.Assert(d == pa1[j].Index, "Merda!");
                    pa1[j] = null;
                    if (d != foo)
                    {
                        this.Photons[j] = this.Photons[d];
                        this.Photons[j].Index = j;
                    }
                    else
                    {
                        this.Photons[j] = foo_photon;
                        this.Photons[j].Index = j;
                        if (i < this.stored_photons)
                        {
                            for (; foo <= this.stored_photons; foo++)
                            {
                                if (pa1[foo] != null)
                                {
                                    break;
                                }
                            }
                            foo_photon = this.Photons[foo];
                            j = foo;
                        }
                        continue;
                    }
                    j = d;
                }
                pa1 = null;
            }
            this.half_stored_photons = this.stored_photons / 2 - 1;
        }
        //OK
        private static void swap(Photon[] ph, int a, int b)
        {
            Photon ph2 = ph[a];
            ph[a] = ph[b];
            ph[b] = ph2;

            //ph[a].Index = a;
            //ph[b].Index = b;
        }
        //OK
        // MedianSplit splits the photon array into two separate
        // pieces around the median with all photons below the
        // the median in the lower half and all photons above
        // than the median in the upper half. The comparison
        // criteria is the axis (indicated by the axis parameter)
        // (inspired by routine in "Algorithms in C++" by Sedgewick)
        private static void MedianSplit(Photon[] p, int start, int end, int median, int axis)
        {
            int left = start;
            int right = end;
            while (right > left)
            {
                double v = p[right].Position[axis];
                int i = left - 1;
                int j = right;
                while (true)
                {
                    while (p[++i].Position[axis] < v) { }
                    while (p[--j].Position[axis] > v && j > left) { }
                    if (i >= j)
                    {
                        break;
                    }
                    swap(p, i, j);
                }
                swap(p, i, right);
                if (i >= median)
                {
                    right = i - 1;
                }
                if (i <= median)
                {
                    left = i + 1;
                }
            }
        }
        //OK
        // See "Realistic image synthesis using Photon Mapping" chapter 6
        // for an explanation of this function
        private void BalanceSegment(Photon[] pbal, Photon[] porg, int index, int start, int end)
        {
            // --------------------
            // compute new median
            // --------------------
            int median = 1;
            while ((4 * median) <= (end - start + 1))
            {
                median += median;
            }
            if ((3 * median) <= (end - start + 1))
            {
                median += median;
                median += start - 1;
            }
            else
            {
                median = end - median + 1;
            }
            // --------------------------
            // find axis to split along
            // --------------------------
            int axis = 2;
            if ((this.bbox_max.X - this.bbox_min.X) > (this.bbox_max.Y - this.bbox_min.Y)
               && (this.bbox_max.X - this.bbox_min.X) > (this.bbox_max.Z - this.bbox_min.Z))
            {
                axis = 0;
            }
            else if ((this.bbox_max.Y - this.bbox_min.Y) > (this.bbox_max.Z - this.bbox_min.Z))
            {
                axis = 1;
            }
            // ------------------------------------------
            // partition photon block around the median
            // ------------------------------------------
            MedianSplit(porg, start, end, median, axis);
            pbal[index] = porg[median];
            //pbal[index].Index = index;
            pbal[index].Plane = (short)axis;
            // ----------------------------------------------
            // recursively balance the left and right block
            // ----------------------------------------------
            if (median > start)
            {
                // balance left segment
                if (start < median - 1)
                {
                    double tmp = this.bbox_max[axis];
                    this.bbox_max[axis] = pbal[index].Position[axis];
                    this.BalanceSegment(pbal, porg, 2 * index, start, median - 1);
                    this.bbox_max[axis] = tmp;
                }
                else
                {
                    pbal[2 * index] = porg[start];
                    //pbal[2 * index].Index = 2 * index;
                }
            }
            if (median < end)
            {
                // balance right segment
                if (median + 1 < end)
                {
                    double tmp = this.bbox_min[axis];
                    this.bbox_min[axis] = pbal[index].Position[axis];
                    this.BalanceSegment(pbal, porg, 2 * index + 1, median + 1, end);
                    this.bbox_min[axis] = tmp;
                }
                else
                {
                    pbal[2 * index + 1] = porg[end];
                    //pbal[2 * index + 1].Index = 2 * index + 1;
                }
            }
        }
    }
}