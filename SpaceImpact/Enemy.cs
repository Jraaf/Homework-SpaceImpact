using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceImpact
{
    internal class Enemy
    {
        public int xPosition { get; set; }
        public int yPosition { get; set; }
        public Enemy(int xPosition, int yPosition)
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
        }

        public Enemy() { }

        public string[] Model=
        {
            "  ╱│ ",
            "<══╡ ",
            " \\│ " 
        };
        public void Draw(int xPos,int yPos)
        {
            Console.SetCursorPosition(xPos, yPos);
            foreach (var line in Model)
            {
                Console.Write(line);
                Console.SetCursorPosition(xPos, ++yPos);
            }
        }
        public void MoveAndRespawn()
        {
            Random random = new Random();

        }

    }
}
