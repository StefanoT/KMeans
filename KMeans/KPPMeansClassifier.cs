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
    /// Classifier of the K Means++ algorithm
    /// </summary>
    public class KPPMeansClassifier : KMeansClassifier
    {
        /// <summary>
        /// initialize first iteration by choosing first mean point at random and next ones with the KMeans++ algorithm
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <remarks>
        /// The exact algorithm is as follows:
        /// 1 - Choose one center uniformly at random from among the data points.
        /// 2 - For each data point x, compute D(x), the distance between x and the nearest center that has already been chosen.
        /// 3 - Choose one new data point at random as a new center, using a weighted probability distribution where a point x is chosen with probability proportional to D(x)2.
        /// 4 - Repeat Steps 2 and 3 until k centers have been chosen.
        /// 5 - Now that the initial centers have been chosen, proceed using standard k-means clustering.
        /// </remarks>
        override protected KMeansCluster[] pickStartingClusters(int[] points, int numClusters)
        {
            Random rnd = new Random();
            KMeansCluster[] currentClusters = new KMeansCluster[numClusters];
            // first item is chosen randomly
            currentClusters[0] = new KMeansCluster(points[rnd.Next(points.Length)]);

            for (int i = 1; i < numClusters; i++)
            {
                // compute the total of squared distances of each point compared to existing clusters
                float accumulatedDistances = 0.0f;
                // store results of the first loop into this array
                float[] accDistances = new float[points.Length];
                for (int pointIdx = 0; pointIdx < points.Length; pointIdx++)
                {
                    // find the minimum distance between the current point and all existing clusters
                    float minDistance = currentClusters[0].distanceFromMean(points[pointIdx]);
                    for (int clusterIdx = 1; clusterIdx < i; clusterIdx++)
                    {
                        float currentDistance = currentClusters[clusterIdx].distanceFromMean(points[pointIdx]);
                        if (currentDistance < minDistance)
                            minDistance = currentDistance;
                    }
                    // accumulate squared min distance                   
                    // note: points already used in previous clusters will have zero distance, so they will not be picked in
                    // the following loop as they have the same accDistances value as the previous point
                    accumulatedDistances += minDistance * minDistance;
                    accDistances[pointIdx] = accumulatedDistances;
                }
                // pick a random point in the distribution of squared min distances
                float targetPoint = (float)rnd.NextDouble() * accumulatedDistances;
                // create new cluster using this point as mean                
                for (int pointIdx = 0; pointIdx < points.Length; pointIdx++)
                {                   
                    if (accDistances[pointIdx] >= targetPoint)
                    {
                        currentClusters[i] = new KMeansCluster(points[pointIdx]);
                        break;
                    }
                }                
            }
            return currentClusters;
        }
    }
}
