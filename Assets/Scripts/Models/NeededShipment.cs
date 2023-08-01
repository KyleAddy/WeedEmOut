using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeededShipment
{
    public int[] boxNeeded = new int[5];

    public NeededShipment(int box0, int box1, int box2, int box3, int box4)
    {
        boxNeeded[0] = box0;
        boxNeeded[1] = box1;
        boxNeeded[2] = box2;
        boxNeeded[3] = box3;
        boxNeeded[4] = box4;
    }
}
