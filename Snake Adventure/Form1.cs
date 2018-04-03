using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake_Adventure
{
    public partial class Form1 : Form
    {
        private Snake game;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            game = new Snake(this);

        }

        

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.DoubleBuffered = true;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            game.paint(e.Graphics);
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(game.getState()))
            {
                if (e.KeyCode == Keys.Y)
                {
                    game = null;
                    game = new Snake(this);
                }
                else if (e.KeyCode == Keys.N)
                {
                    this.Close();
                }
            }
            else
            {
                game.keyDown(e);
            }
        }
    }

    public class Snake
    {
        private const int WIDTH = 300;
        private const int HEIGHT = 330;
        private const int DOT_SIZE = 10;
        private const int ALL_DOTS = 900;
        private const int RAND_POS = 29;
        private const int DELAY = 140;

        private int[] x = new int[ALL_DOTS];
        private int[] y = new int[ALL_DOTS];

        private int dots;
        private int newDots;
        private int score;
        private int apples = 0;
        /*private int speed = 140;*/
        private int apple_x;
        private int apple_y;
        private int bonus_x;
        private int bonus_y;
        private int bonusPoints;

        private bool left = false;
        private bool right = true;
        private bool up = false;
        private bool down = false;
        private bool inGame = true;
        private bool showApple = true;

        private Form parent;
        private Timer timer;
        private Image ball;
        private Image apple;
        private Image head;
        private Image headRight;
        private Image headLeft;
        private Image headUp;
        private Image headDown;
        private Image bonus;

        public Snake(Form form)
        {
            //get the images and initiate the game
            this.parent = form;
            ball = Properties.Resources.dot;
            apple = Properties.Resources.apple;
            bonus = Properties.Resources.bonus1;
            headRight = Properties.Resources.headRight;
            headLeft = Properties.Resources.headLeft;
            headUp = Properties.Resources.headUp;
            headDown = Properties.Resources.headDown;

            initGame();
        }

        public void initGame()
        {

            dots = 3;//start with 3 dots

            //set the initial x and y positions of the three dots next to each other
            for (int z = 0; z < dots; z++)
            {
                x[z] = 100 - (z * 10);
                y[z] = 100;
            }

            //generate the initial x and y positions of the apple
            locateApple();
            timer = new Timer();
            timer.Interval = DELAY;
            timer.Tick += updateGame;

        }

        public bool getState()
        {
            return this.inGame;
        }

        public void move()
        {
            //change the x and y of each dot to the x and y of the dot before it
            for (int z = dots; z > 0; z--)
            {
                x[z] = x[(z - 1)];
                y[z] = y[(z - 1)];
            }

            //change the x and y positions of the head by the dot size as it moves left, right, up or down
            if (left)
            {
                x[0] -= DOT_SIZE;
            }

            if (right)
            {
                x[0] += DOT_SIZE;
            }

            if (up)
            {
                y[0] -= DOT_SIZE;
            }

            if (down)
            {
                y[0] += DOT_SIZE;
            }
        }

        public void checkCollision()
        {

            for (int z = dots; z > 0; z--)
            {
                //check if snake has eaten itself (i.e the head is touching one of its own dots)
                if ((z > 4) && (x[0] == x[z]) && (y[0] == y[z]))
                {
                    inGame = false;
                }
            }
            //check if snake goes beyond the bottom, top, right and left of the screen
            if (y[0] > HEIGHT)
            {
                inGame = false;
            }

            if (y[0] < 30)
            {
                inGame = false;
            }

            if (x[0] > WIDTH)
            {
                inGame = false;
            }

            if (x[0] < 0)
            {
                inGame = false;
            }
        }

        public void checkApple()
        {
            //if apple is eaten create new one
            if ((x[0] == apple_x) && (y[0] == apple_y))
            {
                dots++;
                apples++;
                score += 5;
                SoundPlayer player = new SoundPlayer(Properties.Resources.gobble);
                player.Load();
                player.Play();
                if (apples == 5)
                {
                    locateBonus();
                    apples = 0;
                }
                else
                    locateApple();
            }
        }

        public void checkBonus()
        {
            //if apple is eaten create new one
            if ((x[0] == bonus_x) && (y[0] == bonus_y))
            {
                SoundPlayer player = new SoundPlayer(Properties.Resources.gobble);
                player.Load();
                player.Play();
                newDots = (int)bonusPoints / 5;

                for (int i = 1; i <= newDots; i++)
                {
                    x[(dots + 1)] = x[dots];
                    y[(dots + 1)] = y[dots];
                    dots++;
                }

                score += newDots * 5;
                locateApple();
            }
        }

        public void locateApple()
        {
            //generate random x and y position for the apple
            Random rnd = new Random();
            int r = (int)(rnd.NextDouble() * RAND_POS);
            apple_x = ((r * DOT_SIZE));
            apple_y = 0;
            while (apple_y < 30)
            {
                r = (int)(rnd.NextDouble() * RAND_POS);
                apple_y = ((r * DOT_SIZE));
            }

            showApple = true;
        }

        public void locateBonus()
        {
            //generate random x and y position for the apple
            Random rnd = new Random();
            int r = (int)(rnd.NextDouble() * RAND_POS);
            bonus_x = ((r * DOT_SIZE));
            bonus_y = 0;
            while (bonus_y < 30)
            {
                r = (int)(rnd.NextDouble() * RAND_POS);
                bonus_y = ((r * DOT_SIZE));
            }
            showApple = false;
            bonusPoints = 50;
            SoundPlayer player = new SoundPlayer(Properties.Resources.bonus);
            player.Load();
            player.Play();
        }

        public void updateGame(object sender, EventArgs e)
        {
            //repaint the component after checking collisions and moving
            if (inGame)
            {
                if (!(showApple))
                {
                    //decrement bonus points by 1 every tick, stop if points is less than zero
                    bonusPoints--;
                    if (bonusPoints < 0)
                    {
                        locateApple();
                    }

                    checkBonus();
                }
                else
                {
                    checkApple();
                }

                checkCollision();
                move();
            }

            parent.Invalidate();
        }

        public void paint(Graphics g)
        {
            if (inGame)
            {
                //draw the apple/bonus using the image and the x and y generated by locateApple/Bonus()
                if (showApple)
                    g.DrawImage(apple, new Point(apple_x, apple_y));
                else
                    g.DrawImage(bonus, new Point(bonus_x, bonus_y));

                for (int z = 0; z < dots; z++)
                {
                    if (z == 0)
                    {
                        if (right)
                            head = headRight;
                        else if (left)
                            head = headLeft;
                        else if (up)
                            head = headUp;
                        else
                            head = headDown;
                        //draw the head of the snake using the image and the x and y positions
                        g.DrawImage(head, new Point(x[z], y[z]));
                    }
                    else
                        //draw the dots of the snake using the image and the x and y positions
                        g.DrawImage(ball, new Point(x[z], y[z]));
                }

                //draw the score on the screen
                SolidBrush brush = new SolidBrush(Color.White);
                Font font = new Font(new FontFamily("Arial"), 12, FontStyle.Bold);
                g.DrawString("Score: " + score, font, brush, new PointF(5, 5));
                if (!(showApple))
                    g.DrawString("Bonus: " + bonusPoints, font, brush, new PointF(100, 5));
                //draw line to seperate score section with game area
                Pen pen = new Pen(brush, 2);
                g.DrawLine(pen, new PointF(0, 28), new PointF(320, 28));

            }
            else
            {
                gameOver(g);
            }
        }

        public void gameOver(Graphics g)
        {
            //draw the string Game Over! on the screen
            SolidBrush brush = new SolidBrush(Color.White);
            Font font = new Font(new FontFamily("Arial"), 12, FontStyle.Bold);
            g.DrawString("Game Over!", font, brush, new PointF((WIDTH / 2) - 40, (HEIGHT / 2) - 5));
            g.DrawString("Play again? Enter Y/N", font, brush, new PointF((WIDTH / 2) - 80, (HEIGHT / 2) + 10));

        }

        public void keyDown(KeyEventArgs e)
        {
            //change direction depending on pressed key
            Keys key = e.KeyCode;

            if ((key == Keys.NumPad4) && (!right))
            {
                left = true;
                up = false;
                down = false;
            }

            if ((key == Keys.NumPad6) && (!left))
            {
                if (!timer.Enabled)
                {
                    timer.Start();
                }
                right = true;
                up = false;
                down = false;
            }

            if ((key == Keys.NumPad8) && (!down))
            {
                up = true;
                right = false;
                left = false;
            }

            if ((key == Keys.NumPad2) && (!up))
            {
                down = true;
                right = false;
                left = false;
            }
        }
    }
}
