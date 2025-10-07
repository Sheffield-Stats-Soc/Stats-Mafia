# Overview

The app manages the functionality of the Stats Mafia game. This includes setup, rolling, visualisation and other summary stats calculations, timed discussion, elimination, and win condition logic.

# Scope

- Simple desktop app (WPF UI)
- Core logic as a .NET class library
- Configurable players, rounds, imposters, lives and other settings

# Setup
- Enter unique validated player names (<= 20 chars)
- Configure: number of rounds, rolls per round, number of imposters, lives, discussion time (default to 180s)
- Randomly assign biased dice secretly

# Gameplay
- For each round:
  1. System rolls each player's die k times and records the results
  2. Show a table with each player's name and their raw rolls
  3. Under each player's row, include a menu with all the tools avaliable to players for this round
     - Round 1: Nothing
     - Round 2: Summary Stats (mean, SD, count, min. max)
     - Round 3: Histogram
     - Round 4+: Can easily add more tools as additional blocks in this menu (confidence intervals, hypothesis tests)
  4. Discussion Timer (display only, timing doesn't actually affect functionality)
  5. Vote, choose a player from a drop down menu or a skip option, before pressing a next round button, if a player is selected, they are ejected from the round, a wrong vote leads to decremented lives
- Win condition:
  - Win if all imposters are identified before lives run out
  - Lose if lives reach zero

# Functional Requirements
## Enums
- ToolType {None, SummaryStats, Historgam, ConfidenceIntervals, Other}
## Data Classes
- Player (Id: int, Name: string, RollsPerRound: List\<List\<>>, AllRolls: List\<int> (flattened list of all rolls), IsImposter: bool)
- Settings (NumRounds: int, RollsPerRound:int, NumImposters: int, Lives: int, DiscussionSeconds: int)
- Round (Index: int, PlayerRolls: Dictionary\<int, List\<int>>, ToolsUnlocked: List\<ToolType>)
- IDice (interface), contains roll method
  - FairDice : IDice
  - BiasedDice : IDice (uses a custom probability mass function)
## Logic Classes
- GameManager
  - Initialises players, assigns imposters and dice
  - Increments rounds, generating olls
  - Tracks lives, current round, history and game state (in progress, win, loss)
- StatisticsManager
  - Completes calculations for per-player statistics
## UI
- Setup window: names (also provides editing options for name list), settings and a start game button
- Main window:
  - Header: current round, remaining lives, and timer
  - Player data table: name + raw roll data
  - Collapsable tool menu under each player (updates with each round)
  - Voting menu (list of all players and a skip button), confirm button to start next round and finalise vote
- Histogram Window:
  - Pop-out window for histograms produced for player data

## Extra Scope
- TTS/Narration

# Program Structure
- Specification/
  - gameplay.md
  - requirements.md
- Logic/
  - Player.cs
  - Settings.cs
  - etc.
- Utilities/
  - ColourHelper.cs
  - SpeechSystem.cs
- Windows/
  - Histogram.xaml
  - Histogram.xaml.cs
  - NameInput.xaml (This is to become the general set up window)
  - NameInput.xaml.cs
- MainWindow.xaml
- MainWindow.xaml.cs
- App.xaml
- App.xmal.cs