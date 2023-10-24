using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace Activity3
{
    public partial class Form1 : Form
    {
        int side;
        int numX = 20;
        int numY = 16;
        Square[,] grid;
        Square highlighted = new Square();
        Square selected = new Square(0, 0);
        LinkedList<PathNode> tree;
        Queue<PathNode> search;
        int exploreLimit;
        int exploreCount;
        PathNode goalNode;
        private List<List<PathNode>> paths = new List<List<PathNode>>();

        public Form1()
        {
            InitializeComponent();
            grid = new Square[numX, numY];
            exploreLimit = numX * numY;
            resetGrid();
            side = Convert.ToInt16(pictureBox1.Width / numX);
            tree = new LinkedList<PathNode>();
            tree.AddFirst(new PathNode(selected));
            search = new Queue<PathNode>();
            search.Enqueue(tree.First.Value);
        }

        public void resetGrid()
        {
            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    grid[x, y] = new Square(x, y);
                }
            }
        }

        private bool isValid(int x, int y)
        {
            return x >= 0 && x < numX && y >= 0 && y < numY;
        }

        private bool hasArrived(PathNode target)
        {
            return target.location.X == highlighted.X && target.location.Y == highlighted.Y;
        }

        private void exploreNode(PathNode target)
        {
            Point[] directions = {
            // Directions ni nako sila....
            new Point(0, -1),
            new Point(-1, 0),
            new Point(1, 0),
            new Point(0, 1)
            };

            foreach (Point dir in directions)
            {
                Point nextPoint = new Point(target.location.X + dir.X, target.location.Y + dir.Y);

                if (isValid(nextPoint.X, nextPoint.Y))
                {
                    Square check = grid[nextPoint.X, nextPoint.Y];
                    if (!check.explored)
                    {
                        PathNode newNode = new PathNode(check, target);
                        tree.AddLast(newNode);
                        search.Enqueue(newNode);
                        check.explored = true;
                    }
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            resetGrid();

            highlighted.X = e.X / side;
            highlighted.Y = e.Y / side;

            tree = new LinkedList<PathNode>();
            tree.AddFirst(new PathNode(selected));
            search = new Queue<PathNode>();
            search.Enqueue(tree.First.Value);

            bool foundGoal = false;

            while (!foundGoal && search.Count > 0)
            {
                PathNode target = search.Dequeue();
                foundGoal = hasArrived(target);

                if (foundGoal)
                {
                    goalNode = target;
                    lblPath.Text = "Path Found";
                    break;
                }
                else
                {
                    exploreNode(target);
                }
            }

            if (!foundGoal)
            {
                lblPath.Text = "No Path Found";
            }

            pictureBox1.Refresh();

            lblMouse.Text = "X: " + e.X + " Y: " + e.Y;
            lblSquare.Text = "( " + highlighted.X + " , " + highlighted.Y + " ) ";
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Color selectedColor = Color.SlateBlue;
            Color highlightedColor = Color.Red;
            Color defaultColor = Color.Silver;

            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    Rectangle cell = new Rectangle(x * side, y * side, side, side);
                    Color cellColor;

                    if (x == selected.X && y == selected.Y)
                        cellColor = selectedColor;
                    else if (x == highlighted.X && y == highlighted.Y)
                        cellColor = highlightedColor;
                    else
                        cellColor = defaultColor;

                    e.Graphics.FillRectangle(new SolidBrush(cellColor), cell);
                    e.Graphics.DrawRectangle(Pens.Black, cell);
                }

                PathNode currGoalNode = goalNode;
                while (currGoalNode != null)
                {
                    if ((selected.X == currGoalNode.location.X && selected.Y == currGoalNode.location.Y) ||
                        (highlighted.X == currGoalNode.location.X && highlighted.Y == currGoalNode.location.Y))
                    {
                        currGoalNode = currGoalNode.origin;
                        continue;
                    }

                    Rectangle goalCell = new Rectangle(currGoalNode.location.X * side, currGoalNode.location.Y * side, side, side);
                    e.Graphics.FillRectangle(Brushes.IndianRed, goalCell);
                    currGoalNode = currGoalNode.origin;
                }
            }
        }


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            selected.X = e.X / side;
            selected.Y = e.Y / side;

            pictureBox1.Refresh();

            lblSelected.Text = "( " + selected.X + " , " + selected.Y + " ) ";
        }
    }
}