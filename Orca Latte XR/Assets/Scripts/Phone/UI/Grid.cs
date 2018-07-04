using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class Grid : Division {

        public int columnCount;
        public int rowCount;
        public int borderSize;
        public bool resizeCells;

        public List<Division> cells;

		// Called on initialisation
        protected virtual void Awake () {
            ResetGrid();
        }

        public void ResetGrid () {
			// Calculate the width of each cell
			int cellWidth = Mathf.FloorToInt((bounds.size.x - borderSize) / columnCount) - borderSize;

			// Set the starting y position
            int dY = borderSize;

			// Fill the grid until it is full
            for (int y = 0; y < rowCount; y++) {
                int rowHeight = 0;
                for (int x = 0; x < columnCount; x++) {
                    // Check position in array
                    int i = y * columnCount + x;
                    if (i >= cells.Count) {
                        break;
                    }

                    // Set cell size
                    if (resizeCells) {
                        float scale = (float)cellWidth / (float)cells[i].bounds.size.x;
                        int cellHeight = Mathf.FloorToInt(cells[i].bounds.size.y * scale);
                        cells[i].Resize(cellWidth, cellHeight);
                    }

                    // Set cell position
                    cells[i].transform.parent = transform;
                    Vector3 position = Vector3.back;
					position.x = bounds.center.x + (cellWidth - bounds.size.x) / 2 + borderSize + (cellWidth + borderSize) * x;
					position.y = bounds.center.y + bounds.extents.y - cells[i].bounds.extents.y - dY;
                    cells[i].transform.localPosition = position;

                    // Update row height
					rowHeight = Mathf.Max(rowHeight, Mathf.FloorToInt(cells[i].bounds.size.y));
                }
				dY += rowHeight + borderSize;
            }

			// Deactivate all cells that do not fit in the grid
			for (int i = rowCount * columnCount; i < cells.Count; i++) {
				cells [i].gameObject.SetActive (false);
			}

            // Update grid bounds
            if (encapsulate) {
                foreach(Division cell in cells) {
                    CheckEncapsulation(cell);
                }
            }
        }

		// Removes all cells from the grid
		public void ClearGrid () {
			cells = new List<Division> ();
			ResetGrid ();
		}
    }
}
