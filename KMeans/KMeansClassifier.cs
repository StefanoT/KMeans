/*
Project: K Means demo
Author: Stefano Tommesani
Website: http://www.tommesani.com
Microsoft Public License (MS-PL) [OSI Approved License]
This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.
2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
3. Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMeans
{
    /// <summary>
    /// Classifier of the K Means algorithm
    /// </summary>
    public class KMeansClassifier
    {
        /// <summary>
        /// classify array of points into numCluster clusters
        /// </summary>
        /// <param name="points"></param>
        /// <param name="numCluster">number of clusters to divide </param>
        /// <returns>array of clusters</returns>
        public KMeansCluster[] Run(int[] points, int numClusters)
        {            
            KMeansCluster[] currentClusters = pickStartingClusters(points, numClusters);
            // iteratively improve the clusters by moving points to the cluster with the nearby centroid
            while (true)
            {
                KMeansCluster[] newClusters = new KMeansCluster[numClusters];
                for (int i = 0; i < numClusters; i++)
                    newClusters[i] = new KMeansCluster();

                assingPointsToClusters(points, currentClusters, newClusters);
                dumpClustersToConsole(newClusters);
                if (isStable(currentClusters, newClusters))
                    return newClusters;                    
                currentClusters = newClusters;
            }
        }

        /// <summary>
        /// initialize first iteration by choosing random mean points within the given array of points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        virtual protected KMeansCluster[] pickStartingClusters(int[] points, int numClusters)
        {
            Random rnd = new Random();
            KMeansCluster[] currentClusters = new KMeansCluster[numClusters];
            for (int i = 0; i < numClusters; i++)
            {
                currentClusters[i] = new KMeansCluster(points[rnd.Next(points.Length)]);
            }
            return currentClusters;
        }

        /// <summary>
        /// assign points of the incoming array to the cluster with the nearest centroid
        /// </summary>
        /// <param name="points"></param>
        /// <param name="prevClusters"></param>
        /// <param name="nextClusters"></param>
        private void assingPointsToClusters(int[] points, KMeansCluster[] prevClusters, KMeansCluster[] nextClusters)
        {
            foreach (int currentPoint in points)
            {
                float bestDistance = prevClusters[0].distanceFromMean(currentPoint);
                int bestIndex = 0;
                for (int i = 1; i < prevClusters.Length; i++)
                {
                    float currentDistance = prevClusters[i].distanceFromMean(currentPoint);
                    if (currentDistance < bestDistance)
                    {
                        bestDistance = currentDistance;
                        bestIndex = i;
                    }
                }
                nextClusters[bestIndex].addPoint(currentPoint);
            }
        }

        /// <summary>
        /// dump the array to clusters to the console
        /// </summary>
        /// <param name="clusters"></param>
        private void dumpClustersToConsole(KMeansCluster[] clusters)
        {
            for (int i = 0; i < clusters.Length; i++)
            {
                Console.WriteLine(@"Cluster #" + (i+1).ToString());

                StringBuilder sb = new StringBuilder();
                foreach (int currentPoint in clusters[i].points)
                {
                    sb.Append(currentPoint.ToString() + " ");
                }
                Console.WriteLine(sb.ToString());
            }
            Console.WriteLine();
        }

        /// <summary>
        /// verify if the classification has converged by comparing the clusters
        /// </summary>
        /// <param name="prevClusters"></param>
        /// <param name="nextClusters"></param>
        /// <returns>true if converged, false otherwise</returns>
        private bool isStable(KMeansCluster[] prevClusters, KMeansCluster[] nextClusters)
        {
            // check number of items in each cluster
            for (int i = 0; i < prevClusters.Length; i++)
            {
                if (prevClusters[i].points.Count != nextClusters[i].points.Count)
                    return false;
            }
            // check if mean has not changed significantly (epsilon = 0.1)
            float EPSILON = 0.1f;
            for (int i = 0; i < prevClusters.Length; i++)
            {
                if (Math.Abs(prevClusters[i].mean - nextClusters[i].mean) > EPSILON)
                    return false;
            }
            return true;
        }
    }
}
