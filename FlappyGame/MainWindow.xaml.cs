using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;
using System.IO;


namespace FlappyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        MediaPlayer backgroundMusic = new MediaPlayer();


        double score;
        int gravity = 8;
        bool gameOver;
        Rect flappyBirdHitBox;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += MainEventTimer;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            // Find the MP3 inside the Music folder
            string musicPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Sounds",
                "menumusic.mp3");

            // Load the music
            backgroundMusic.Open(new Uri(musicPath));

            // Volume: 0.0 = silent, 1.0 = maximum. .10 is quiet, .25 is background music, .50 is loud and 1.0 is max
            backgroundMusic.Volume = 0.45;

            // When the song finishes, start it again
            backgroundMusic.MediaEnded += BackgroundMusic_MediaEnded;

            // Start playing
            backgroundMusic.Play();


            StartGame();
        }

        private void BackgroundMusic_MediaEnded(object? sender, EventArgs e)
        {
            // Go back to the beginning
            backgroundMusic.Position = TimeSpan.Zero;

            // Play again
            backgroundMusic.Play();
        }

        private void MainEventTimer(object? sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;

            // Create hitbox around the player
            flappyBirdHitBox = new Rect(
                Canvas.GetLeft(flappyBird) +20,
                Canvas.GetTop(flappyBird) +10,
                flappyBird.Width - 40,
                flappyBird.Height -20
            );

            // Apply gravity
            Canvas.SetTop(
                flappyBird,
                Canvas.GetTop(flappyBird) + gravity
            );

            // Check if player went above or below the screen
            if (Canvas.GetTop(flappyBird) < -10 ||
                Canvas.GetTop(flappyBird) > 458)
            {
                EndGame();
            }

            foreach (var x in MyCanvas.Children.OfType<Image>())
            {
                // MOVE ALL RADARS
                if ((string)x.Tag == "obs1" ||
                    (string)x.Tag == "obs2" ||
                    (string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x,Canvas.GetLeft(x) - 5);

                    // Hitbox around the CURRENT RADAR
                    Rect pipeHitBox = new Rect(
                        Canvas.GetLeft(x) +8,
                        Canvas.GetTop(x) +4,
                        x.Width -16,
                        x.Height - 16
                    );

                    // Give score when the radar passes the player
                    if (Canvas.GetLeft(x) <= 25 &&
                        Canvas.GetLeft(x) > 20)
                    {
                        score += .5;
                    }

                    // Check collision
                    if (flappyBirdHitBox.IntersectsWith(pipeHitBox))
                    {
                        EndGame();
                    }
                }

                // RESET OBSTACLE 1
                if ((string)x.Tag == "obs1" &&
                    Canvas.GetLeft(x) < -100)
                {
                    Canvas.SetLeft(x, 800);

                 
                }

                // RESET OBSTACLE 2
                if ((string)x.Tag == "obs2" &&
                    Canvas.GetLeft(x) < -200)
                {
                    Canvas.SetLeft(x, 800);

                
                }

                // RESET OBSTACLE 3
                if ((string)x.Tag == "obs3" &&
                    Canvas.GetLeft(x) < -250)
                {
                    Canvas.SetLeft(x, 800);

                
                }

                // MOVE CLOUDS
                if ((string)x.Tag == "cloud")
                {
                    Canvas.SetLeft(
                        x,
                        Canvas.GetLeft(x) - 2
                    );

                    if (Canvas.GetLeft(x) < -250)
                    {
                        Canvas.SetLeft(x, 550);
                    }
                }
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                flappyBird.RenderTransform =
                    new RotateTransform(
                        -20,
                        flappyBird.Width / 2,
                        flappyBird.Height / 2
                    );

                gravity = -8;
            }

            if (e.Key == Key.R && gameOver == true)
            {
                StartGame();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                flappyBird.RenderTransform =
                    new RotateTransform(
                        5,
                        flappyBird.Width / 2,
                        flappyBird.Height / 2
                    );

                gravity = 8;
            }
        }

        private void StartGame()
        {
            MyCanvas.Focus();

            int temp = 300;

            score = 0;
            gameOver = false;

            gravity = 8;

            // Reset player
            Canvas.SetTop(flappyBird, 155);

            // Reset all objects
            foreach (var x in MyCanvas.Children.OfType<Image>())
            {
                if ((string)x.Tag == "obs1")
                {
                    Canvas.SetLeft(x, 500);
                }

                if ((string)x.Tag == "obs2")
                {
                    Canvas.SetLeft(x, 800);
                }

                if ((string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x, 1000);
                }

                if ((string)x.Tag == "cloud")
                {
                    Canvas.SetLeft(x, 300 + temp);

                    temp = 800;
                }
            }

            gameTimer.Start();
        }

        private void EndGame()
        {
            gameTimer.Stop();

            gameOver = true;

            txtScore.Content += " Game Over! Click R to retry.";
        }
    }
}