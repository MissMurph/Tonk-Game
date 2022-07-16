using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    [SerializeField]
    private int sizeX, sizeY;

    private Node[,] array;

    private void Start() {
        array = new Node[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeX; y++) {
                array[x, y] = new Node();
            }
        }
    }

    private struct Node {
        private Vector2Int pos;

        private Node (int _x, int _y) {
            pos = new Vector2Int(_x, _y);
        }
    }
}