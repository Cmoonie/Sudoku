using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minisudoku
{
    

public partial class Form1 : Form
    {
        private int[,] sudokuGrid = new int[4, 4];
        private Button[,] buttons = new Button[4, 4];
        private Timer playTimer;
        private int secondsElapsed = 0;

        public Form1()
        {
            InitializeComponent();
            InitializeSudokuGrid();
            InitializeButtons();
            InitializeTimer();
        }

        private void InitializeButtons()
        {
            int buttonSize = 77; // Grootte van elke knop
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    buttons[row, col] = new Button
                    {
                        Size = new Size(buttonSize, buttonSize),
                        Location = new Point(col * buttonSize + 186, row * buttonSize + 77),
                        Font = new Font("Arial", 16),
                        Text = sudokuGrid[row, col] != 0 ? sudokuGrid[row, col].ToString() : ""
                    };

                    buttons[row, col].Click += PlayerClickButton;
                    this.Controls.Add(buttons[row, col]);
                }
            }
        }


        // Willekeurige Sudoku genereren (4x4)
        private void InitializeSudokuGrid()
        {
            Random random = new Random();

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    int num;
                    int attempts = 0;
                    do
                    {
                        num = random.Next(1, 5);
                        attempts++;
                        if (attempts > 10) // Voorkom eindeloze loop
                        {
                            row = 0;
                            col = -1;
                            ClearGrid(); // Rooster resetten
                            break;
                        }
                    }
                    while (!IsValidMove(row, col, num));

                    if (col >= 0)
                    {
                        sudokuGrid[row, col] = num;
                    }
                }
            }
        }

        private void ClearGrid()
        {
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 4; col++)
                    sudokuGrid[row, col] = 0;
        }


        private bool IsValidMove(int row, int col, int num)
        {
            // Check rij en kolom
            for (int i = 0; i < 4; i++)
            {
                if (sudokuGrid[row, i] == num || sudokuGrid[i, col] == num)
                    return false;
            }

            // Check 2x2 blok
            int startRow = (row / 2) * 2;
            int startCol = (col / 2) * 2;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (sudokuGrid[startRow + i, startCol + j] == num)
                        return false;
                }
            }
            return true;
        }

        private void InitializeTimer()
        {
            playTimer = new Timer();
            playTimer.Interval = 1000; // 1 seconde
            playTimer.Tick += (sender, e) =>
            {
                secondsElapsed++;
                this.Text = $"Sudoku - Tijd: {secondsElapsed}s";
            };
            playTimer.Start();
        }


        private void PlayerClickButton(object sender, EventArgs e)
        {
            {
                Button clickedButton = sender as Button;

                // Getal invoeren
                if (clickedButton != null)
                {
                    using (var inputForm = new InputForm())
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK)
                        {
                            clickedButton.Text = inputForm.Value.ToString();
                        }
                    }
                }

                CheckGame(); // Check of sudoku klopt na invoer
            }

        }


         // Controle of de sudoku correct is ingevuld
        private void CheckGame()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    int value;
                    if (!int.TryParse(buttons[row, col].Text, out value) || !IsValidMove(row, col, value))
                    {
                        return; // Fout gevonden
                    }
                }
            }

            // Alles correct ingevuld!
            playTimer.Stop();
            MessageBox.Show($"Gefeliciteerd! Je hebt de sudoku opgelost in {secondsElapsed} seconden.", "Succes!");
        }
    }

        public class InputForm : Form
        {
            public int Value { get; private set; }

            public InputForm()
            {
                this.Text = "Voer een getal in";
                this.Size = new Size(200, 150);

                var inputBox = new TextBox() { Location = new Point(20, 20), Width = 140 };
                var okButton = new Button() { Text = "OK", Location = new Point(20, 60), Width = 60 };
                okButton.Click += (sender, e) =>
                {
                    if (int.TryParse(inputBox.Text, out int value) && value >= 1 && value <= 4)
                    {
                        Value = value;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Voer een getal tussen 1 en 4 in.");
                    }
                };

                this.Controls.Add(inputBox);
                this.Controls.Add(okButton);
            }
        }

        
    

    static class Program
    {
        [STAThread] //voorkom je threading-problemen in Windows Forms.
        static void Main()
        {
            Application.EnableVisualStyles();// start het Form1 - venster.
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());// start het Form1-venster.
        }
    }


}

