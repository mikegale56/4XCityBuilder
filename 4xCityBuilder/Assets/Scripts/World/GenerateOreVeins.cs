using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerateOreVeins {

    public static void CreateVein(Vector2Int ij, int travelLength, byte[,] undergroundValue, byte toValue, float branchProbability, int N)
    {

        // Generate a new direction
        float theta = Random.Range(0, 2 * Mathf.PI);
        Vector2 aimDir = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
        CreateVein(ij, travelLength, undergroundValue, toValue, branchProbability, N, aimDir);

    }

    public static void CreateVein(Vector2Int ij, int travelLength, byte[,] undergroundValue, byte toValue, float branchProbability, int N, Vector2 aimDir)
    {
        Stack<Vector2Int> ijValues = new Stack<Vector2Int>();
        Stack<int> lenValues = new Stack<int>();
        ijValues.Push(ij);
        lenValues.Push(travelLength);
        int tmp = 0;
        while (ijValues.Count > 0)
        {

            tmp++;
            //Debug.Log("length of ijValues: " + ijValues.Count.ToString());
            PropogateVein(ijValues.Pop(), travelLength, aimDir, undergroundValue, toValue, branchProbability, N, ijValues, lenValues);
            
            // Generate a new direction
            float theta = Random.Range(0, 2 * Mathf.PI);
            aimDir = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));

            branchProbability = branchProbability / 2;
            if (tmp > 50)
                break;
        }

    }


    public static void PropogateVein(Vector2Int ij, int travelLength, Vector2 aimDir, byte[,] undergroundValue, byte toValue, 
        float branchProbability, int N, Stack<Vector2Int> ijValues, Stack<int> lenValues)
    {
        Vector2 targetIJ = new Vector2(ij.x + travelLength * aimDir.x, ij.y + travelLength * aimDir.y);
        int ind = 0;
        float roll;
        float xChance = Mathf.Abs(aimDir.x) / (Mathf.Abs(aimDir.x) + Mathf.Abs(aimDir.y));
        xChance = Mathf.Min(xChance, 0.9F);
        xChance = Mathf.Max(xChance, 0.1F);
        Vector2Int moveVec = new Vector2Int();
        while (ind < travelLength)
        {
            ind++;
            // Set this tile to be the ore value
            undergroundValue[ij.x, ij.y] = toValue;

            // Roll for x or y based on the main direction of motion
            roll = Random.Range(0F, 1F);
            moveVec.x = 0; moveVec.y = 0;
            if (roll <= xChance) // Choose to go in the x direction
                moveVec.x = (int)Mathf.Sign(aimDir.x);
            else // Choose to go in the y direction
                moveVec.y = (int)Mathf.Sign(aimDir.y);

            // Roll for right or wrong direction
            roll = Random.Range(0F, 1F);
            if (roll < 0.2) // Wrong direction
            {
                moveVec.x = -moveVec.x;
                moveVec.y = -moveVec.y;
            }

            // Travel
            ij = ij + moveVec;
            // Enforce bounds
            if (ij.x < 0) break;
            if (ij.x == N-1) break;
            if (ij.y < 0) break;
            if (ij.y == N-1) break;

            // Check if this vein branches
            float randVal = Random.Range(0F, 1F);
            //Debug.Log("Rolled " + randVal.ToString() + ", branch prob " + branchProbability.ToString());
            if (randVal < branchProbability)
            {
                // Generate a new length
                int newLength;
                if (travelLength > 1000)
                    newLength = travelLength;
                else
                    newLength = PoissonRandomGenerator.GetPoisson(travelLength - ind);
                if (newLength <= 2) continue;
                Debug.Log("New Branch of length " + newLength.ToString());
                // Branch the vein
                ijValues.Push(ij);
                lenValues.Push(newLength);
            }
        }
    }


}
