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
using System.Collections.Generic;
using System.Xml.Serialization;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Util;

namespace DrawEngine.Renderer.Samplers
{
    /// <summary>
    /// Based on Ray Tracing From the Ground Up: http://www.raytracegroundup.com/
    /// </summary>
    [XmlInclude(typeof(RegularGridSampler)), XmlInclude(typeof(JitteredSampler)), Serializable]
    public abstract class Sampler
    {
        protected int numberOfSamples; // the number of sample points in a set
        protected int numberOfSets; // the number of sample sets

        protected List<int> shuffledSamplesIndices; // shuffled samples array indices
        //protected List<Point2D> diskSamples; // sample points on a unit disk
        protected List<Point3D> sphereOrHemisphereSamples; // sample points on a unit sphere or hemisphere
        //protected List<Point3D> sphereSamples; // sample points on a unit sphere
        protected List<Point2D> samples; // sample points on a unit square or disk
        protected int usedSamplesCount; //the current number of sample points used
        protected int jump; // random index jump


        public Sampler() : this(1, 83) { }

        public Sampler(int numberOfSamples, int numberOfSets)
        {
            this.samples = new List<Point2D>(numberOfSamples * numberOfSets);
            this.NumberOfSamples = numberOfSamples;
            this.NumberOfSets = numberOfSets;
            //this.GenerateSamples();
        }

        /// <summary>
        /// Number of Samples in a Set
        /// </summary>
        public int NumberOfSamples
        {
            get { return this.numberOfSamples; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("NumberOfSamples", value, "NumberOfSamples must be >= 1");
                }
                this.numberOfSamples = value;
                this.GenerateSamples();
                this.SetupShuffledIndices();
            }
        }

        /// <summary>
        /// Number of Sets of Samples
        /// </summary>
        public int NumberOfSets
        {
            get { return this.numberOfSets; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("NumberOfSets", value, "NumberOfSets must be >= 1");
                }
                this.numberOfSets = value;
                this.GenerateSamples();
                this.SetupShuffledIndices();
            }
        }

        protected abstract void GenerateSamples();
        public IEnumerable<Point2D> GetSamplesFor(float x, float y)
        {
            const int pixelSize = 1;
            for (int j = 0; j < numberOfSamples; j++)
            {
                Point2D sample = this.SampleUnitSquareOrDisk();

                sample.X = pixelSize * (x - 0.5f + sample.X);
                sample.Y = pixelSize * (y - 0.5f + sample.Y);

                yield return sample;
            }

        }


        private static void Swap<T>(ref T src, ref T dest) where T : struct
        {
            T temp = src;
            src = dest;
            dest = temp;
        }
        // shuffle the x coordinates of the points within each set
        public void ShuffleXCoordinates()
        {
            Random random = new Random(83);
            for (int p = 0; p < numberOfSets; p++)
                for (int i = 0; i < numberOfSamples - 1; i++)
                {
                    int sourceIndex = i + p * numberOfSamples + 1;
                    int targetIndex = random.Next() % numberOfSamples + p * numberOfSamples;
                    Point2D source = samples[sourceIndex];
                    Point2D target = samples[targetIndex];
                    Swap(ref source.X, ref target.X);
                    samples[sourceIndex] = source;
                    samples[targetIndex] = target;

                }
        }
        // shuffle the y coordinates of the points within set
        public void ShuffleYCoordinates()
        {
            Random random = new Random();
            for (int p = 0; p < numberOfSets; p++)
                for (int i = 0; i < numberOfSamples - 1; i++)
                {
                    int targetIndex = random.Next() % numberOfSamples + p * numberOfSamples;
                    int sourceIndex = i + p * numberOfSamples + 1;
                    Point2D source = samples[sourceIndex];
                    Point2D target = samples[targetIndex];
                    Swap(ref source.Y, ref target.Y);
                    samples[sourceIndex] = source;
                    samples[targetIndex] = target;
                }
        }

        // sets up randomly shuffled indices for the samples array
        private void SetupShuffledIndices()
        {
            shuffledSamplesIndices = new List<int>(numberOfSamples * numberOfSets);
            List<int> indices = new List<int>(numberOfSamples);

            for (int j = 0; j < numberOfSamples; j++)
                indices.Add(j);

            for (int p = 0; p < numberOfSets; p++)
            {
                indices.Shuffle();
                for (int j = 0; j < numberOfSamples; j++)
                    shuffledSamplesIndices.Add(indices[j]);
            }
        }

        // Maps the 2D sample points in the square [-1,1] X [-1,1] to a unit disk, using Peter Shirley's
        // concentric map function
        public void MapSamplesToUnitDisk()
        {
            List<Point2D> samplesCopy = new List<Point2D>(samples);
            float r, phi;		// polar coordinates
            Point2D sp; 		// sample point on unit disk
            for (int j = 0; j < samplesCopy.Count; j++)
            {
                // map sample point to [-1, 1] X [-1,1]

                sp.X = 2.0f * samplesCopy[j].X - 1.0f;
                sp.Y = 2.0f * samplesCopy[j].Y - 1.0f;

                if (sp.X > -sp.Y)
                {			// sectors 1 and 2
                    if (sp.X > sp.Y)
                    {		// sector 1
                        r = sp.X;
                        phi = sp.Y / sp.X;
                    }
                    else
                    {					// sector 2
                        r = sp.Y;
                        phi = 2 - sp.X / sp.Y;
                    }
                }
                else
                {						// sectors 3 and 4
                    if (sp.X < sp.Y)
                    {		// sector 3
                        r = -sp.X;
                        phi = 4 + sp.Y / sp.X;
                    }
                    else
                    {					// sector 4
                        r = -sp.Y;
                        if (sp.Y != 0)	// avoid division by zero at origin
                            phi = 6 - sp.X / sp.Y;
                        else
                            phi = 0.0f;
                    }
                }

                phi *= (float)(Math.PI / 4.0);
                Point2D sample = new Point2D((float)(r * Math.Cos(phi)), (float)(r * Math.Sin(phi)));
                samples[j] = sample;
            }
        }
        /// <summary>
        /// Maps the 2D sample points to 3D points on a unit hemisphere with a cosine power
        /// density distribution in the polar angle
        /// </summary>
        /// <param name="exp">Power</param>
        public void MapSamplesToHemisphere(float exp)
        {
            this.sphereOrHemisphereSamples = new List<Point3D>(samples.Count);

            for (int j = 0; j < samples.Count; j++)
            {
                float cosPhi = (float)Math.Cos(2.0 * Math.PI * samples[j].X);
                float sinPhi = (float)Math.Sin(2.0 * Math.PI * samples[j].X);
                float cosTheta = (float)Math.Pow((1.0 - samples[j].Y), 1.0 / (exp + 1.0));
                float sinTheta = (float)Math.Sqrt(1.0 - cosTheta * cosTheta);
                float pu = sinTheta * cosPhi;
                float pv = sinTheta * sinPhi;
                float pw = cosTheta;
                this.sphereOrHemisphereSamples.Add(new Point3D(pu, pv, pw));
            }
        }

        // Maps the 2D sample points to 3D points on a unit sphere with a uniform density 
        // distribution over the surface
        // this is used for modelling a spherical light
        public void MapSamplesToSphere()
        {
            float r1, r2;
            float x, y, z;
            float r, phi;
            float twoPI = (float)(Math.PI + Math.PI);
            sphereOrHemisphereSamples = new List<Point3D>(samples.Count);

            for (int j = 0; j < samples.Count; j++)
            {
                r1 = samples[j].X;
                r2 = samples[j].Y;
                z = 1.0f - 2.0f * r1;
                r = (float)Math.Sqrt(1.0 - z * z);
                phi = twoPI * r2;
                x = (float)(r * Math.Cos(phi));
                y = (float)(r * Math.Sin(phi));
                sphereOrHemisphereSamples.Add(new Point3D(x, y, z));
            }
        }



        public Point2D SampleUnitSquareOrDisk()
        {
            Random random = new Random();
            if (usedSamplesCount % numberOfSamples == 0)  									// start of a new pixel
                jump = (random.Next() % numberOfSets) * numberOfSamples;				// random index jump initialised to zero in constructor

            return (samples[jump + shuffledSamplesIndices[jump + usedSamplesCount++ % numberOfSamples]]);
        }

        // get next sample on unit hemisphere
        public Point3D SampleSphereOrHemisphere()
        {
            Random random = new Random();
            if (usedSamplesCount % numberOfSamples == 0)  									// start of a new pixel
                jump = (random.Next() % numberOfSets) * numberOfSamples;

            return (this.sphereOrHemisphereSamples[jump + shuffledSamplesIndices[jump + usedSamplesCount++ % numberOfSamples]]);
        }


        // only used to set up a vector noise table
        // this is not discussed in the book, but see the
        // file LatticeNoise.cpp in Chapter 31
        // This is a specialised function called in LatticeNoise::init_vector_table
        // It doesn't shuffle the indices
        public Point2D SampleOneSet()
        {
            return (samples[usedSamplesCount++ % numberOfSamples]);
        }

    }
}