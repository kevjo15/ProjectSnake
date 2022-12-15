using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace Snake
{
    public partial class Form2 : Form
    {
        //skapar en lista av circle som kallas snake
        private List<Circle> Snake = new List<Circle>();
        //skapar en instans av klassen circle
        private Circle food = new Circle();

        float maxWidth;
        float maxHeight;

        int score;
        public int highScore;

        bool left, right, up, down;

        float velocity = 0.3f;
        float colorTimer = 0f;

        Random rand = new Random();


        public Form2()
        {
            InitializeComponent();
            new Settings();

            /*string path = @"C:\Users\SteamBox\Documents\GitHub\H-stTerminProjectX\Snake\text.txt";
            StreamReader stream = new StreamReader(path);
            string fileData = stream.ReadLine();
            txtHighScore.Text = "HighScore: " + fileData;
            stream.Close();

            StreamWriter streamWriter = new StreamWriter(path);*/

            using (StreamReader reader = new StreamReader(@"C:\Users\SteamBox\Documents\GitHub\H-stTerminProjectX\Snake\text.txt"))
            {
                txtHighScore.Text = reader.ReadLine();
            }



        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            //om key left är nertryckt och direction inte är höger, så svänger den vänster
            if (e.KeyCode == Keys.Left && Settings.directions != "right") left = true;
            if (e.KeyCode == Keys.Right && Settings.directions != "left") right = true;
            if (e.KeyCode == Keys.Up && Settings.directions != "down") up = true;
            if (e.KeyCode == Keys.Down && Settings.directions != "up") down = true;
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            //samma sak fast tvärtom, om left key is up är left key också false
            if (e.KeyCode == Keys.Left) left = false;
            if (e.KeyCode == Keys.Right) right = false;
            if (e.KeyCode == Keys.Up) up = false;
            if (e.KeyCode == Keys.Down) down = false;
        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            //Setting the directions
            if (left)
            {
                Settings.directions = "left";
            }
            if (right)
            {
                Settings.directions = "right";
            }
            if (up)
            {
                Settings.directions = "up";
            }
            if (down)
            {
                Settings.directions = "down";
            }
            //end of directions

            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.directions)
                    {
                        case "left": //när ormen rör sig åt vänster tar vi bort en cirkel i X led
                            Snake[i].x -= 1 * velocity;
                            break;
                        case "right": //när ormen rör sig åt höger lägger vi till en cirkel i X led
                            Snake[i].x += 1 * velocity;
                            break;
                        case "up":
                            Snake[i].y -= 1 * velocity;
                            break;
                        case "down":
                            Snake[i].y += 1 * velocity;
                            break;
                    }
                    if (Snake[i].x < 0)
                    {
                        Snake[i].x = maxWidth;
                    }
                    if (Snake[i].x > maxWidth)
                    {
                        Snake[i].x = 0;
                    }
                    if (Snake[i].y < 0)
                    {
                        Snake[i].y = maxHeight;
                    }
                    if (Snake[i].y > maxHeight)
                    {
                        Snake[i].y = 0;
                    }

                    if (Collision(Snake[i].x, food.x, 1f) && Collision(Snake[i].y, food.y, 1f))
                    {
                        Eat();
                    }

                    //kollar om head kolliderar med någon cirkel i body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        //om snake head har samma position som snake body i x och y led kallar vi på GameOver()
                        if (Snake[i].x == Snake[j].x && Snake[i].y == Snake[j].y)
                        {
                            GameOver();
                        }
                    }

                }
                //får varje cirkel i kroppen att följa varandra.
                else
                {
                    Snake[i].x = Snake[i - 1].x;
                    Snake[i].y = Snake[i - 1].y;
                }
            }
            picCanvas.Invalidate();
        }

        static bool Collision(float value1, float value2, float acceptableDifference)
        {
            return Math.Abs(value1 - value2) <= acceptableDifference;
        }

        private void UpdateMapGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColour;

            //går igenom snake litsan
            for (int i = 0; i < Snake.Count; i++)
            {
                //första indexen i snake listan är huvudet och den blir då svart
                if (i == 0)
                {
                    snakeColour = Brushes.Black;
                }
                else
                {
                    snakeColour = Brushes.Purple; // resten av kroppen blir lila
                }
                //definera x och y, alltså bredden och höjden till snake
                canvas.FillEllipse(snakeColour, new Rectangle((int)(Snake[i].x * Settings.width), (int)(Snake[i].y * Settings.height), (int)Settings.width, (int)Settings.height));
            }
            colorTimer += 0.1f;
            //definera x och y, alltså bredden och höjden till food
            var newBrush = Brushes.Red;
            if (colorTimer >= 8f)
                newBrush = Brushes.Orange;
            if (colorTimer >= 16f)
                newBrush = Brushes.Brown;
            if (colorTimer >= 32f) GameOver();
            canvas.FillEllipse(newBrush, new Rectangle((int)(food.x * Settings.width), (int)(food.y * Settings.height), (int)Settings.width, (int)Settings.height));
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            //avslutar program
            Environment.Exit(1);
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void txtScore_Load(object sender, EventArgs e)
        {
            lblGameOver.Parent = picCanvas;
            lblGameOver.BackColor = Color.Transparent;
        }

        private void Takesnapshot(object sender, EventArgs e)
        {
            Label caption = new Label();
            caption.Text = "I Scored: " + score + "and my Highscore is " + highScore;
            caption.Font = new Font("Ariel", 12, FontStyle.Bold);
            caption.ForeColor = Color.DarkBlue;
            caption.AutoSize = false;
            caption.Width = picCanvas.Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(caption);

            //Spara snapshot till en jpeg fil 
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Snake game snapshot";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image File | *.jpg";
            dialog.ValidateNames = true;



            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(picCanvas.Width);
                int height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(width, height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                picCanvas.Controls.Remove(caption);


            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void RestartGame()
        {
            lblGameOver.Text = "";
            colorTimer = 0f;
            //defalt values vi vill ha när spelet börjar
            maxWidth = picCanvas.Width / Settings.width - 1; // -1 är padding för ormen så den inte kommer för nära väggarna
            maxHeight = picCanvas.Height / Settings.height - 1;
            //rensar a child objekt om de finns från snake list
            Snake.Clear();
            //om starbutton är Enable när spelet startar går de inte att använda up, ner, höger, vänster pilarna
            StartButton.Enabled = false;
            SnapButton.Enabled = false;
            score = 0;
            txtscore.Text = "Score: " + score;


            //skapar head av instansen circle och ger han en startposition
            Circle head = new Circle { x = 10, y = 5 };
            Snake.Add(head); //lägger till head i listan snake

            for (int i = 0; i < 10; i++)
            {
                //skapar objekt body av instansen circle
                Circle body = new Circle();
                Snake.Add(body); //lägger till body till listan snake
            }
            //skapar ny object food av instansen circle, och ger den en random spanpoint.
            food = new Circle { x = rand.Next(2, (int)maxWidth), y = rand.Next(2, (int)maxHeight) }; //  2an här är padding från väggen

            gameTimer.Start(); // Startar speltimer

        }

        private void Eat()
        {
            colorTimer = 0f; // resets timer för äpplen att röttna
            score += 1; // lägger till poäng för varje äpple som äts
            txtscore.Text = "Score: " + score;
            Circle body = new Circle
            {
                x = Snake[Snake.Count - 1].x,
                y = Snake[Snake.Count - 1].y,
            };
            Snake.Add(body);
            //när du har ätit en circel så skapas en ny
            food = new Circle { x = rand.Next(2, (int)maxWidth), y = rand.Next(2, (int)maxHeight) };

            //varje food du äter ökar spelets hastighet
            if (gameTimer.Interval == 1) return;
            gameTimer.Interval = gameTimer.Interval - 1;
        }
        private void GameOver()
        {
            lblGameOver.Text = "Game over";
            colorTimer = 0f; // resets timer för äpplen att röttna
            gameTimer.Interval = 40; //återställer spelhastiheten
            gameTimer.Stop(); //stannar speltimern när spelet är över
            StartButton.Enabled = true; //enable start knappen igen så man kan starta ett nytt spel efter man har förlorat
            SnapButton.Enabled = true;

            //skriver ut din highscore
            if (score > highScore)
            {
                highScore = score;
                txtHighScore.Text = Convert.ToString(highScore);
                txtHighScore.ForeColor = Color.Red;

                using (StreamWriter writer = new StreamWriter(@"C:\Users\SteamBox\Documents\GitHub\H-stTerminProjectX\Snake\text.txt"))
                {
                    writer.WriteLine(highScore);
                }

            }
        }
    }
}
