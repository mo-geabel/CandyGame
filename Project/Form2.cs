using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public partial class Form2 : Form
    {
        private Timer gameTimer;
        private int timeLeft = 200; // Game time limit
        private Button selectedTile = null; // First selected tile
        private bool isFilling = true; // Tracks if the grid is being filled

        // Property for TimeLeft
        public int TimeLeft
        {
            get => timeLeft;
            private set
            {
                timeLeft = value;
                UpdateTimerLabel();
            }
        }

        // Property for Score (Encupsulation)
        private int _score;
        public int score
        {
            get => _score;
            private set
            {
                _score = value;
                UpdateScoreLabel();
            }
        }


        // Property for Player
        private string _playerName;

        public string Player
        {
            get => _playerName;
            private set
            {
                _playerName = value;
                PlayerName.Text = $"Player Name: {value}";
            }
        }



        public Form2(string playerName)
        {
            InitializeComponent();
            Player = playerName;
            PlayerName.Text = $"Player Name: {Player}";
        }
 

        private async void Form2_Load(object sender, EventArgs e)
        {
            DisableUserInput(); // Disable input at game start
            await FillGridWithAnimation(gameGrid); // Wait for the grid to fill
            EnableUserInput(); // Re-enable input after the initial fill
            StartGameTimer(); // Start the game timer
            this.KeyPreview = true; // Enable keyboard events
            this.KeyDown += Form2_KeyDown;
        }




        private void StartGameTimer()
        {
            gameTimer = new Timer
            {
                Interval = 1000 // 1 second interval
            };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            UpdateTimerLabel();
            UpdateScoreLabel();
        }


        private void GameTimer_Tick(object sender, EventArgs e)
        {
            TimeLeft--; // Update time left using the property

            if (TimeLeft <= 0)
                EndGame();
        }

        private void UpdateTimerLabel()
        {
            Timer.Text = $"Time: {timeLeft}";
        }

        private void UpdateScoreLabel()
        {
            Score.Text = $"Score: {score}";
        }

        public static List<KeyValuePair<string, int>> HighScores = new List<KeyValuePair<string, int>>();
    

        private void EndGame()
        {
            gameTimer.Stop();

            // Load existing high scores
            var highScores = HighScoreManager.LoadHighScores();

            // Add the new score
            highScores.Add(new KeyValuePair<string, int>(Player, score));

            // Sort and keep only the top 5 scores
            highScores = highScores
                .OrderByDescending(entry => entry.Value)
                .Take(5)
                .ToList();

            // Save updated high scores
            HighScoreManager.SaveHighScores(highScores);

            // Show game over message and offer to restart
            var result = MessageBox.Show(
                $"Game Over! Your score: {score}\nDo you want to restart the game?",
                "Game Over",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                RestartGame(); // Restart the game if the player chooses "Yes"
            }
            else
            {
                // Return to Form1 (Menu) if the player chooses "No"
               
                this.Close();
            }
        }

        private async void RestartGame()
        {
            gameTimer.Stop(); // Stop the timer
            isPaused = true;

            TimeLeft = 200; // Reset time using the property
            score = 0;      // Reset score using the property

            gameGrid.Controls.Clear();
            UpdateTimerLabel();
            UpdateScoreLabel();

            await FillGridWithAnimation(gameGrid); // Fill the grid with animation

            gameTimer.Start(); // Restart the timer
            isPaused = false;
        }







   



        private const int GridRows = 8; // Define grid rows
        private const int GridColumns = 8; // Define grid columns
        private static readonly string[] SpecialItems = { "jocker", "verticalRocket", "horizontalRocket", "rainbowBall", "copter", "dynamit" };
        private static readonly Color[] Colors = { Color.Red, Color.Blue, Color.Green, Color.Yellow }; // Four colors for items
        private Random random = new Random();

        public async Task FillGridWithAnimation(Panel gamePanel)
        {
            isFilling = true;
            gamePanel.Controls.Clear(); // Clear previous items
            var cells = new List<GameTile>();
            int specialItemCount = 0; // Track the number of special items

            var specialItemImages = new Dictionary<string, Image>
    {
        { "jocker", Properties.Resources.Jocker },
        { "verticalRocket", Properties.Resources.VRocket },
        { "horizontalRocket", Properties.Resources.HRocket },
        { "rainbowBall", Properties.Resources.Rainbow },
        { "copter", Properties.Resources.copter },
        { "dynamit", Properties.Resources.dynamite }
    };

            int tileWidth = gamePanel.Width / GridColumns;
            int tileHeight = gamePanel.Height / GridRows;

            // Create grid
            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns; col++)
                {
                    GameTile tile;

                    // Ensure exactly 4 special items
                    if (specialItemCount < 4 && random.NextDouble() < 0.2)
                    {
                        string specialType = SpecialItems[random.Next(SpecialItems.Length)];
                        tile = new SpecialItemTile(row, col, tileWidth, tileHeight, specialType, specialItemImages[specialType]);
                        tile.Tag = specialType; // Set the Tag to the type of the special item
                        specialItemCount++;
                    }
                    else
                    {
                        Color color = Colors[random.Next(Colors.Length)];
                        tile = new ColorTile(row, col, tileWidth, tileHeight, color);
                        tile.Tag = color; // Set the Tag to the color
                    }

                    tile.Location = new Point(col * tileWidth, row * tileHeight);
                    tile.Visible = false;

                    // Attach click event
                    tile.Click += Item_Click;
                    Debug.WriteLine($"Click event attached to tile at Row: {tile.Row}, Column: {tile.Column}, Tag: {tile.Tag}");

                    cells.Add(tile);
                }
            }

            foreach (var tile in cells)
            {
                gamePanel.Controls.Add(tile);
            }

            // Animate grid filling
            Timer animationTimer = new Timer { Interval = 50 };
            int currentRow = 0;
            var animationCompletion = new TaskCompletionSource<bool>();

            animationTimer.Tick += (sender, e) =>
            {
                if (currentRow >= GridRows)
                {
                    animationTimer.Stop();
                    animationCompletion.SetResult(true);
                    return;
                }

                foreach (var tile in cells.Where(t => t.Row == currentRow))
                {
                    tile.Visible = true;
                }

                currentRow++;
            };

            animationTimer.Start();
            await animationCompletion.Task;

            // Check for matches after filling
            isFilling = false;
            HandleGameCycle(gamePanel);
        }






        private void DisableUserInput()
        {
            this.KeyPreview = false; // Disable keyboard input
            gameGrid.Enabled = false; // Disable mouse interactions on the grid
        }

        private void EnableUserInput()
        {
            this.KeyPreview = true; // Re-enable keyboard input
            gameGrid.Enabled = true; // Re-enable mouse interactions on the grid
        }










        private bool DestroyMatches(Panel gameGrid)
        {
            bool hasMatches = false;
            var matchedCells = new HashSet<GameTile>();

            // Create a dictionary for fast lookup of tiles by position
            var cellMap = new Dictionary<Point, GameTile>();

            foreach (var tile in gameGrid.Controls.OfType<GameTile>())
            {
                var position = new Point(tile.Column, tile.Row);

                if (!cellMap.ContainsKey(position))
                {
                    cellMap[position] = tile; // Add the tile to the map
                }
                else
                {
                    Debug.WriteLine($"Duplicate tile found at {position}. Skipping tile: {tile}");
                }
            }

            // Check for horizontal matches
            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns - 2; col++) // At least 3 tiles for a match
                {
                    if (cellMap.TryGetValue(new Point(col, row), out var cell1) &&
                        cellMap.TryGetValue(new Point(col + 1, row), out var cell2) &&
                        cellMap.TryGetValue(new Point(col + 2, row), out var cell3) &&
                        cell1.Tag != null && cell1.Tag.Equals(cell2.Tag) && cell1.Tag.Equals(cell3.Tag))
                    {
                        matchedCells.UnionWith(new[] { cell1, cell2, cell3 });
                        hasMatches = true;
                    }
                }
            }

            // Check for vertical matches
            for (int col = 0; col < GridColumns; col++)
            {
                for (int row = 0; row < GridRows - 2; row++) // At least 3 tiles for a match
                {
                    if (cellMap.TryGetValue(new Point(col, row), out var cell1) &&
                        cellMap.TryGetValue(new Point(col, row + 1), out var cell2) &&
                        cellMap.TryGetValue(new Point(col, row + 2), out var cell3) &&
                        cell1.Tag != null && cell1.Tag.Equals(cell2.Tag) && cell1.Tag.Equals(cell3.Tag))
                    {
                        matchedCells.UnionWith(new[] { cell1, cell2, cell3 });
                        hasMatches = true;
                    }
                }
            }

            // Remove matched tiles and update score
            foreach (var cell in matchedCells)
            {
                Debug.WriteLine($"Removing matched tile at Row: {cell.Row}, Column: {cell.Column}, Tag: {cell.Tag}");
                gameGrid.Controls.Remove(cell); // Remove from the grid
                score += 10;                    // Increment score
            }

            // Update UI elements
            UpdateScoreLabel();

            // Return whether any matches were found
            return hasMatches;
        }














        private async void FillGrid(Panel gamePanel)
        {
            isFilling = true; // Prevent user interaction during filling

            var columns = Enumerable.Range(0, GridColumns).ToList(); // Columns to process
            int tileWidth = gamePanel.Width / GridColumns;
            int tileHeight = gamePanel.Height / GridRows;

            foreach (int col in columns)
            {
                var columnTiles = gamePanel.Controls.OfType<GameTile>()
                    .Where(tile => tile.Column == col)
                    .OrderByDescending(tile => tile.Row)
                    .ToList();

                int emptyRow = GridRows - 1;

                // Move existing tiles down to fill gaps
                foreach (var tile in columnTiles)
                {
                    if (tile.Row != emptyRow)
                    {
                        Point targetLocation = new Point(tile.Column * tileWidth, emptyRow * tileHeight);
                        await AnimateCellMovement(tile, targetLocation);
                        tile.Row = emptyRow;
                    }
                    emptyRow--;
                }

                // Add new tiles from the top
                while (emptyRow >= 0)
                {
                    // Create a new ColorTile
                    Color randomColor = Colors[random.Next(Colors.Length)];
                    GameTile newTile = new ColorTile(
                        emptyRow,
                        col,
                        tileWidth,
                        tileHeight,
                        randomColor
                    );

                    // Set its location and tag
                    newTile.Location = new Point(col * tileWidth, 0); // Start animation from the top
                    newTile.Tag = randomColor; // Ensure Tag is set for color matching
                    newTile.Click += Item_Click; // Attach the click event
                    gamePanel.Controls.Add(newTile);

                    // Animate its movement
                    Point targetLocation = new Point(col * tileWidth, emptyRow * tileHeight);
                    await AnimateCellMovement(newTile, targetLocation);

                    emptyRow--;
                }
            }


            isFilling = false; // Allow user interaction after filling
            // Check for matches after filling is complete
            HandleGameCycle(gamePanel);

        }



        private async Task AnimateCellMovement(PictureBox cell, Point targetLocation)
        {
            int step = 5; // Number of pixels to move per frame for smooth animation
            int delay = 10; // Delay between frames in milliseconds

            while (cell.Location.Y != targetLocation.Y)
            {
                // Calculate the next Y position
                int nextY = cell.Location.Y;

                if (cell.Location.Y < targetLocation.Y)
                {
                    nextY += step;
                    if (nextY > targetLocation.Y) // Ensure we don't overshoot
                    {
                        nextY = targetLocation.Y;
                    }
                }
                else if (cell.Location.Y > targetLocation.Y)
                {
                    nextY -= step;
                    if (nextY < targetLocation.Y) // Ensure we don't overshoot
                    {
                        nextY = targetLocation.Y;
                    }
                }

                // Update the cell's position
                cell.Location = new Point(cell.Location.X, nextY);

                // Wait for the next frame
                await Task.Delay(delay);
            }

            // Ensure cell snaps to final target location
            cell.Location = targetLocation;
        }










        private PictureBox selectedItem = null; // To track the first selected item


        private void HandleGameCycle(Panel gameGrid)
        {
            bool hasMatches;

            do
            {
                // Step 1: Destroy all matches
                hasMatches = DestroyMatches(gameGrid);

                // Step 2: Refill the grid if matches were found
                if (hasMatches)
                {
                    FillGrid(gameGrid);
                }
            } while (hasMatches); // Repeat until no more matches are found
        }











        private void OnPlayerMove() // Example trigger for a player's move
        {
            HandleGameCycle(gameGrid);
        }


        private void Item_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Item clicked!");

            if (isFilling) return; // Ignore clicks during animation

            if (sender is GameTile clickedTile)
            {
                Debug.WriteLine($"Tile clicked: Row {clickedTile.Row}, Column {clickedTile.Column}");

                // Handle the first click
                if (selectedItem == null)
                {
                    selectedItem = clickedTile;
                    clickedTile.BorderStyle = BorderStyle.Fixed3D; // Highlight the selected tile

                    // Activate the special item immediately if it's a special tile
                    if (clickedTile is SpecialItemTile specialTile)
                    {
                        specialTile.ActivateEffect();
                        ActivateSpecialItem(specialTile, gameGrid); // Trigger special effect
                        selectedItem = null; // Reset selection after activation
                    }
                }
                else
                {
                    // Handle the second click
                    if (clickedTile == selectedItem)
                    {
                        // If the same tile is clicked again, deselect it
                        selectedItem.BorderStyle = BorderStyle.FixedSingle; // Reset border style
                        selectedItem = null;
                    }
                    else
                    {
                        // Perform the swap if it's a different tile
                        SwapItems(selectedItem, clickedTile, gameGrid);
                        selectedItem.BorderStyle = BorderStyle.FixedSingle; // Reset border style
                        selectedItem = null;
                    }
                }
            }
        }







        private void SwapItems(PictureBox firstItem, PictureBox secondItem, Panel gamePanel)
        {
            // Step 1: Validate adjacency
            Point firstPosition = new Point(
                firstItem.Location.X / (gamePanel.Width / GridColumns),
                firstItem.Location.Y / (gamePanel.Height / GridRows)
            );

            Point secondPosition = new Point(
                secondItem.Location.X / (gamePanel.Width / GridColumns),
                secondItem.Location.Y / (gamePanel.Height / GridRows)
            );

            // Check if the items are adjacent
            if (Math.Abs(firstPosition.X - secondPosition.X) + Math.Abs(firstPosition.Y - secondPosition.Y) != 1)
            {
                MessageBox.Show("Items must be adjacent to swap.");
                return;
            }

            // Step 2: Temporarily swap the items
            Point tempLocation = firstItem.Location;
            firstItem.Location = secondItem.Location;
            secondItem.Location = tempLocation;

            // Swap the logical positions (Row and Column) in the tiles' data
            if (firstItem is GameTile firstTile && secondItem is GameTile secondTile)
            {
                int tempRow = firstTile.Row;
                int tempCol = firstTile.Column;

                firstTile.Row = secondTile.Row;
                firstTile.Column = secondTile.Column;

                secondTile.Row = tempRow;
                secondTile.Column = tempCol;
            }

            // Step 3: Check for matches after swapping
            if (DestroyMatches(gamePanel))
            {
                Debug.WriteLine("it is entered");
                // If matches are found, process them
                FillGrid(gamePanel);
            }
            else
            {
                // Revert the swap if no matches are found
                tempLocation = firstItem.Location;
                firstItem.Location = secondItem.Location;
                secondItem.Location = tempLocation;

                // Revert logical positions
                if (firstItem is GameTile revertedFirstTile && secondItem is GameTile revertedSecondTile)
                {
                    int tempRow = revertedFirstTile.Row;
                    int tempCol = revertedFirstTile.Column;

                    revertedFirstTile.Row = revertedSecondTile.Row;
                    revertedFirstTile.Column = revertedSecondTile.Column;

                    revertedSecondTile.Row = tempRow;
                    revertedSecondTile.Column = tempCol;
                }

                MessageBox.Show("Swap didn't result in a match. Swap reverted.");
            }
        }



        private bool CheckForMatches(Panel gamePanel)
        {
            // Create a dictionary to map grid positions to PictureBox controls
            Dictionary<Point, PictureBox> cellMap = new Dictionary<Point, PictureBox>();

            foreach (PictureBox cell in gamePanel.Controls.OfType<PictureBox>())
            {
                Point position = new Point(
                    cell.Location.X / (gamePanel.Width / GridColumns),
                    cell.Location.Y / (gamePanel.Height / GridRows)
                );

                // Map the cell if not already mapped
                if (!cellMap.ContainsKey(position))
                {
                    cellMap.Add(position, cell);
                }
            }

            // Step 1: Detect horizontal matches
            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns - 2; col++)
                {
                    // Check three consecutive tiles in a row
                    if (cellMap.TryGetValue(new Point(col, row), out PictureBox cell1) &&
                        cellMap.TryGetValue(new Point(col + 1, row), out PictureBox cell2) &&
                        cellMap.TryGetValue(new Point(col + 2, row), out PictureBox cell3) &&
                        cell1.Tag != null && cell1.Tag.Equals(cell2.Tag) && cell1.Tag.Equals(cell3.Tag))
                    {
                        return true; // Match found
                    }
                }
            }

            // Step 2: Detect vertical matches
            for (int col = 0; col < GridColumns; col++)
            {
                for (int row = 0; row < GridRows - 2; row++)
                {
                    // Check three consecutive tiles in a column
                    if (cellMap.TryGetValue(new Point(col, row), out PictureBox cell1) &&
                        cellMap.TryGetValue(new Point(col, row + 1), out PictureBox cell2) &&
                        cellMap.TryGetValue(new Point(col, row + 2), out PictureBox cell3) &&
                        cell1.Tag != null && cell1.Tag.Equals(cell2.Tag) && cell1.Tag.Equals(cell3.Tag))
                    {
                        return true; // Match found
                    }
                }
            }

            // No matches found
            return false;
        }

        private void ActivateSpecialItem(PictureBox specialItem, Panel gamePanel)
        {
            string specialItemType = specialItem.Tag.ToString();

            switch (specialItemType)
            {
                case "verticalRocket":
                    DestroyRow(specialItem);
                    break;
                case "horizontalRocket":
                    DestroyColumn(specialItem);
                    break;
                case "rainbowBall":
                    DestroyByColor();
                    break;
                case "jocker":
                    DestroyAround(specialItem);
                    break;
                case "copter":
                    DestroyRandomItems(5); // Destroy 5 random items
                    break;
                case "dynamit":
                    DestroyLargeArea(specialItem);
                    break;
                default:
                    return; // No action for non-special items
            }

            // Remove the special item after activation
            gamePanel.Controls.Remove(specialItem);

            // Refill the grid after the special item effect
            FillGrid(gamePanel);

            // Update the score
            HandleGameCycle(gameGrid);
            UpdateScoreLabel();
        }



        private void DestroyColumn(PictureBox clickedItem)
        {
            int col = clickedItem.Location.X / (gameGrid.Width / GridColumns);

            List<PictureBox> columnItems = new List<PictureBox>();

            // Collect all items in the column
            foreach (PictureBox cell in gameGrid.Controls.OfType<PictureBox>())
            {
                int cellCol = cell.Location.X / (gameGrid.Width / GridColumns);
                if (cellCol == col)
                {
                    columnItems.Add(cell);
                }
            }

            // Remove items in the column
            foreach (PictureBox item in columnItems)
            {
                gameGrid.Controls.Remove(item);

            }

            foreach (PictureBox item in columnItems)
            {
                ShowScorePopup(item, 10);
                AnimateCellRemoval(item);
                score += 10; // Add points for each destroyed item
            }

            UpdateScoreLabel();

        }




        private void DestroyRow(PictureBox clickedItem)
        {
            int row = clickedItem.Location.Y / (gameGrid.Height / GridRows);

            List<PictureBox> rowItems = new List<PictureBox>();

            // Collect all items in the row
            foreach (PictureBox cell in gameGrid.Controls.OfType<PictureBox>())
            {
                int cellRow = cell.Location.Y / (gameGrid.Height / GridRows);
                if (cellRow == row)
                {
                    rowItems.Add(cell);
                }
            }

            // Remove items in the row
            foreach (PictureBox item in rowItems)
            {
                gameGrid.Controls.Remove(item);

            }
            foreach (PictureBox item in rowItems)
            {
                ShowScorePopup(item, 10);
                AnimateCellRemoval(item);
                score += 10; // Add points for each destroyed item
            }




            // Update score and handle game cycle

            UpdateScoreLabel();
        }


        private Color? GetRandomColorFromGrid()
        {
            // Collect all unique colors from non-special items in the grid
            HashSet<Color> uniqueColors = new HashSet<Color>();

            foreach (PictureBox cell in gameGrid.Controls.OfType<PictureBox>())
            {
                bool isSpecialItem = cell.Tag != null && SpecialItems.Contains(cell.Tag.ToString());

                // Only consider non-special items
                if (!isSpecialItem)
                {
                    uniqueColors.Add(cell.BackColor);
                }
            }

            // If no colors are found, return null
            if (uniqueColors.Count == 0) return null;

            // Randomly select one color
            return uniqueColors.ElementAt(random.Next(uniqueColors.Count));
        }


        private void DestroyByColor()
        {
            // Get a random color from the available colors
            Color? randomColor = GetRandomColorFromGrid();

            // If no valid color is found, exit
            if (randomColor == null) return;

            List<PictureBox> matchingItems = new List<PictureBox>();

            // Collect all items with the matching color that are not special items
            foreach (PictureBox cell in gameGrid.Controls.OfType<PictureBox>())
            {
                bool isSpecialItem = cell.Tag != null && SpecialItems.Contains(cell.Tag.ToString());

                if (!isSpecialItem && cell.BackColor.Equals(randomColor))
                {
                    matchingItems.Add(cell);
                }
            }

            // Remove all matching items
            foreach (PictureBox item in matchingItems)
            {
                gameGrid.Controls.Remove(item);

            }
            foreach (PictureBox item in matchingItems)
            {
                ShowScorePopup(item, 10);
                AnimateCellRemoval(item);
                score += 10; // Add points for each destroyed item
            }




            // Update score and handle game cycle

            UpdateScoreLabel();
        }












        private void DestroyAround(PictureBox clickedItem)
        {
            int col = clickedItem.Location.X / (gameGrid.Width / GridColumns);
            int row = clickedItem.Location.Y / (gameGrid.Height / GridRows);

            // Define the area radius (1 for 3x3 area)
            int radius = 1;

            // Collect all items in the defined radius area
            List<PictureBox> areaItems = CollectAreaItems(col, row, radius);

            // Animate and remove items in the area
            foreach (PictureBox item in areaItems)
            {
                ShowScorePopup(item, 10);
                AnimateCellRemoval(item);
                gameGrid.Controls.Remove(item);
                score += 10; // Add points for each destroyed item
            }

            // Update score and handle game cycle
            UpdateScoreLabel();
            HandleGameCycle(gameGrid);
        }

        // Helper method to collect items in a given radius around a center point
        private List<PictureBox> CollectAreaItems(int centerCol, int centerRow, int radius)
        {
            List<PictureBox> areaItems = new List<PictureBox>();

            for (int r = centerRow - radius; r <= centerRow + radius; r++)
            {
                for (int c = centerCol - radius; c <= centerCol + radius; c++)
                {
                    foreach (PictureBox cell in gameGrid.Controls.OfType<PictureBox>())
                    {
                        int cellCol = cell.Location.X / (gameGrid.Width / GridColumns);
                        int cellRow = cell.Location.Y / (gameGrid.Height / GridRows);

                        if (cellCol == c && cellRow == r)
                        {
                            areaItems.Add(cell);
                        }
                    }
                }
            }

            return areaItems;
        }



        private void DestroyRandomItems(int count)
        {
            // Get all current items in the grid
            List<PictureBox> allItems = gameGrid.Controls.OfType<PictureBox>().ToList();

            // Select random items to destroy
            List<PictureBox> selectedItems = new List<PictureBox>();
            for (int i = 0; i < count && allItems.Count > 0; i++)
            {
                int randomIndex = random.Next(allItems.Count);
                selectedItems.Add(allItems[randomIndex]);
                allItems.RemoveAt(randomIndex); // Remove from the available list to avoid duplicates
            }

            // Animate and remove selected items
            foreach (PictureBox item in selectedItems)
            {
                ShowScorePopup(item, 10);  // Display the score popup
                AnimateCellRemoval(item); // Animate the removal
                gameGrid.Controls.Remove(item); // Remove the item from the grid
                score += 10;              // Add points for each destroyed item
            }

            // Update the score and handle game cycle
            UpdateScoreLabel();
            HandleGameCycle(gameGrid);
        }



        private void DestroyLargeArea(PictureBox clickedItem)
        {
            int col = clickedItem.Location.X / (gameGrid.Width / GridColumns);
            int row = clickedItem.Location.Y / (gameGrid.Height / GridRows);

            List<PictureBox> areaItems = new List<PictureBox>();

            // Collect all items in the 5x5 area around the clicked item
            for (int r = row - 2; r <= row + 2; r++)
            {
                for (int c = col - 2; c <= col + 2; c++)
                {
                    foreach (PictureBox cell in gameGrid.Controls.OfType<PictureBox>())
                    {
                        int cellCol = cell.Location.X / (gameGrid.Width / GridColumns);
                        int cellRow = cell.Location.Y / (gameGrid.Height / GridRows);

                        if (cellCol == c && cellRow == r)
                        {
                            areaItems.Add(cell);
                        }
                    }
                }
            }

            // Remove items in the 5x5 area
            foreach (PictureBox item in areaItems)
            {
                gameGrid.Controls.Remove(item);

            }
            foreach (PictureBox item in areaItems)
            {
                ShowScorePopup(item, 10);
                AnimateCellRemoval(item);
                score += 10; // Add points for each destroyed item
            }




            // Update score and handle game cycle
            UpdateScoreLabel();

        }


        private PictureBox focusedItem = null; // Tracks the currently focused item

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (isFilling) return; // Ignore key presses during animation
            if (e.KeyCode == Keys.P) { TogglePause(); return; }
            if (focusedItem == null)
            {
                focusedItem = gameGrid.Controls.OfType<PictureBox>().FirstOrDefault();
                HighlightFocusedItem();
                return;
            }

            int currentCol = focusedItem.Location.X / (gameGrid.Width / GridColumns);
            int currentRow = focusedItem.Location.Y / (gameGrid.Height / GridRows);

            switch (e.KeyCode)
            {
                case Keys.Up:
                    NavigateTo(currentRow - 1, currentCol);
                    break;
                case Keys.Down:
                    NavigateTo(currentRow + 1, currentCol);
                    break;
                case Keys.Left:
                    NavigateTo(currentRow, currentCol - 1);
                    break;
                case Keys.Right:
                    NavigateTo(currentRow, currentCol + 1);
                    break;
                case Keys.Enter:
                    HandleFocusedItemClick();
                    break;
            }
        }



        private void HighlightFocusedItem()
        {
            foreach (PictureBox cell in gameGrid.Controls.OfType<PictureBox>())
            {
                cell.BorderStyle = BorderStyle.FixedSingle; // Reset border for all items
            }

            if (focusedItem != null)
            {
                focusedItem.BorderStyle = BorderStyle.Fixed3D; // Highlight the focused item
            }
        }


        private void NavigateTo(int newRow, int newCol)
        {
            if (newRow >= 0 && newRow < GridRows && newCol >= 0 && newCol < GridColumns)
            {
                focusedItem = gameGrid.Controls.OfType<PictureBox>().FirstOrDefault(cell =>
                    cell.Location.X / (gameGrid.Width / GridColumns) == newCol &&
                    cell.Location.Y / (gameGrid.Height / GridRows) == newRow);

                HighlightFocusedItem();
            }
        }


        private void HandleFocusedItemClick()
        {
            if (focusedItem != null)
            {
                Item_Click(focusedItem, EventArgs.Empty); // Trigger the click event for the focused item
            }
        }


        private bool isPaused = false; // Tracks if the game is paused

        private void TogglePause()
        {
            if (isPaused)
            {
                // Resume the game
                gameTimer.Start();
                isPaused = false;
                MessageBox.Show("Game Resumed", "Pause");
            }
            else
            {
                // Pause the game
                gameTimer.Stop();
                isPaused = true;

                // Show a message box with Continue and Close options
                var result = MessageBox.Show("Game is paused. Do you want to continue or close the game? yes to continue or no to close the game",
                                             "Pause",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Resume the game
                    gameTimer.Start();
                    isPaused = false;
                }
                else if (result == DialogResult.No)
                {
                    // Close the form
                    this.Close();
                }
            }
        }

        private void ShowScorePopup(PictureBox cell, int points)
        {
            Label scorePopup = new Label
            {
                Text = $"+{points}",
                ForeColor = Color.Gold,
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.Transparent,
                Location = cell.Location,
                AutoSize = true
            };

            gameGrid.Controls.Add(scorePopup);

            Timer popupTimer = new Timer
            {
                Interval = 30
            };

            popupTimer.Tick += (sender, e) =>
            {
                if (scorePopup.Location.Y > cell.Location.Y - 30)
                {
                    scorePopup.Location = new Point(scorePopup.Location.X, scorePopup.Location.Y - 2);
                }
                else
                {
                    popupTimer.Stop();
                    gameGrid.Controls.Remove(scorePopup);
                }
            };

            popupTimer.Start();
        }


        private async void AnimateCellRemoval(PictureBox cell)
        {
            // Create a simple fade-out animation
            for (int opacity = 100; opacity >= 0; opacity -= 10)
            {
                cell.BackColor = Color.FromArgb(opacity * 255 / 100, cell.BackColor); // Fade out the color
                await Task.Delay(50); // Wait for 50ms between steps
            }

            // Remove the cell after the animation
            gameGrid.Controls.Remove(cell);
        }




    }

}