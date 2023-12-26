using Godot;
using System;
using System.Collections.Generic;

public class Main : Control
{
    private Label ResultLabel;

    private string PlayerMark;
    private string OpponentMark;
    private bool PlayersTurn;
    private bool IsEndRound;

    private CanvasLayer Overlay;
    private TextureButton XBtn;
    private TextureButton OBtn;
    private Button ExitBtn;

    private Control Cells;
    private List<TextureButton> CellList;

    private Texture texture;
    private string[] marksArray;
    private float Delay;

    private int PlayerScore;
    private int OpponentScore;

    private Label PlayerScoreLabel;
    private Label OpponentScoreLabel;

    private TextureRect Bar1;
    private TextureRect Bar2;

    public override void _Ready()
    {
        Bar1 = GetNode<TextureRect>("SlasherControl/Bar1");
        Bar2 = GetNode<TextureRect>("SlasherControl/Bar2");

        ResultLabel = GetNode<Label>("ResultLabel");
        PlayerScoreLabel = GetNode<Label>("ScoreboardBox/VsBox/PlayerBox/PlayerScore");
        OpponentScoreLabel = GetNode<Label>("ScoreboardBox/VsBox/OpponentBox/OpponentScore");

        Delay = 0.4f;
        marksArray = new string[9];
        texture = ResourceLoader.Load<Texture>("res://Assets/marks.png");
        Overlay = GetNode<CanvasLayer>("Overlay");
        XBtn = GetNode<TextureButton>("Overlay/MarkerChoices/Choices/X");
        OBtn = GetNode<TextureButton>("Overlay/MarkerChoices/Choices/O");
        ExitBtn = GetNode<Button>("Overlay/ExitBtn");

        ExitBtn.Connect("pressed", this, "ExitGame");
        XBtn.Connect("pressed", this, "SetPlayersMarkChoice", new Godot.Collections.Array {"x"});
        OBtn.Connect("pressed", this, "SetPlayersMarkChoice", new Godot.Collections.Array {"o"});

        Cells = GetNode<Control>("GridControl/Cells");
        Godot.Collections.Array cellsArr = Cells.GetChildren();
        CellList = new List<TextureButton>();
        foreach (TextureButton cell in cellsArr)
        {
            CellList.Add(cell);
            cell.Connect("pressed", this, "ClickCell", new Godot.Collections.Array {cell});
        }

        PlayerScore = 0;
        OpponentScore = 0;

        Overlay.Visible = true;
        Bar1.Visible = false;
        Bar2.Visible = false;
    }

    private void SetPlayersMarkChoice(string choice)
    {
        if (choice == "x")
        {
            PlayerMark = "x";
            OpponentMark = "o";
            PlayersTurn = true;
        }
        else 
        {
            PlayerMark = "o";
            OpponentMark = "x";
            PlayersTurn = false;
        }
        StartGame();
    }

    private void ExitGame()
    {
        GetTree().Quit();
    }

    private void StartGame()
    {
        Overlay.Hide();
        if (!PlayersTurn)
        {
            PCPlay();
        }
    }

    private bool IsMarksArrayFull()
    {
        foreach (object cell in marksArray)
        {
            if (cell == null) return false;
        }
        return true;
    }

    private async void PCPlay()
    {
        bool searching = true;
        TextureButton choice;
        while (searching && !IsMarksArrayFull())
        {
            Random random = new Random();
            int i = random.Next(9);
            if (marksArray[i] == null)
            {
                choice = CellList[i];
                await ToSignal(GetTree().CreateTimer(Delay), "timeout");
                MarkCell(false, choice);
                searching = false;
            }
        }
        PlayersTurn = true;
    }

    private void ClickCell(TextureButton cell)
    {
        if (PlayersTurn)
        {
            MarkCell(true, cell);
            PlayersTurn = false;
            PCPlay();
        }
    }

    private void MarkCell(bool isPlayer, TextureButton cell)
    {
        if (IsEndRound) return;

        AtlasTexture img = new AtlasTexture();
        img.Atlas = texture;
        string mark = "";

        if (isPlayer)
        {
            if (PlayerMark == "x")
            {
                img.Region = new Rect2(0f,100f,100f,100f);
                mark = "x";
            }
            else
            {
                img.Region = new Rect2(100f,100f,100f,100f);
                mark = "o";
            }
        }
        else
        {
            if (OpponentMark == "x")
            {
                img.Region = new Rect2(0f,100f,100f,100f);
                mark = "x";
            }
            else
            {
                img.Region = new Rect2(100f,100f,100f,100f);
                mark = "o";
            }
        }

        cell.TextureNormal = img;
        cell.Disabled = true;
        RecordMarks(cell, mark);
        CheckForWinner();
    }

    private void RecordMarks(TextureButton cell, string mark)
    {
        int numName = cell.GetPositionInParent();
        marksArray[numName] = mark;   
    }

    private void CheckForWinner()
    {
        string pattern = "";
        for (int i = 0; i < 10; i++)
        {
            switch(i)
            {
                case 0:
                    pattern = marksArray[0] + marksArray[1] + marksArray[2];
                    break;
                case 1:
                    pattern = marksArray[3] + marksArray[4] + marksArray[5];
                    break;
                case 2:
                    pattern = marksArray[6] + marksArray[7] + marksArray[8];
                    break;
                case 3:
                    pattern = marksArray[0] + marksArray[3] + marksArray[6];
                    break;
                case 4:
                    pattern = marksArray[1] + marksArray[4] + marksArray[7];
                    break;
                case 5:
                    pattern = marksArray[2] + marksArray[5] + marksArray[8];
                    break;
                case 6:
                    pattern = marksArray[0] + marksArray[4] + marksArray[8];
                    break;
                case 7:
                    pattern = marksArray[2] + marksArray[4] + marksArray[6];
                    break;
                
                case 8:
                    pattern = marksArray[1] + marksArray[4] + marksArray[7] + marksArray[3] + marksArray[5];
                    break;
                case 9:
                    pattern = marksArray[0] + marksArray[4] + marksArray[8] + marksArray[2] + marksArray[6];
                    break;
            }
            if (pattern == "xxx")
            {
                DeclareWinner("x", i);
            }
            else if (pattern == "ooo")
            {
                DeclareWinner("o", i);
            }
            else if (pattern == "xxxxx")
            {
                DeclareWinner("x", i);
            }
            else if (pattern == "ooooo")
            {
                DeclareWinner("o", i);
            }
            else
            {
                if (!IsEndRound)
                {
                    CheckForDraw();
                }
            }
        }
    }

    private void DeclareWinner(string winner, int casePattern)
    {
        if (winner == PlayerMark)
        {
            ResultLabel.Text = "You Win!";
            AddPointPlayer();
        }
        else
        {
            ResultLabel.Text = "You Lose!";
            AddPointOpponent();
        }
        ResultLabel.Show();
        DrawSlash(casePattern);
        IsEndRound = true;
    }

    private void CheckForDraw()
    {
        if (IsMarksArrayFull())
        {
            DeclareDraw();
        }
        else
        {
            return;
        }
    }

    private void DeclareDraw()
    {
        ResultLabel.Text = "It's a Draw!";
        ResultLabel.Show();
    }

    private void AddPointPlayer()
    {
        PlayerScore++;
        UpdateScore();
    }

    private void AddPointOpponent()
    {
        OpponentScore++;
        UpdateScore();
    }

    private void UpdateScore()
    {
        PlayerScoreLabel.Text = PlayerScore.ToString().PadLeft(3, '0');
        OpponentScoreLabel.Text = OpponentScore.ToString().PadLeft(3, '0');
    }

    private void DrawSlash(int pattern)
    {
        Bar1.Visible = true;
        switch(pattern)
        {
            case 0:
                Bar1.SetPosition(new Vector2(416f,148f));
                Bar1.SetRotation(0f);
                break;
            case 1:
                Bar1.SetPosition(new Vector2(416f,264f));
                Bar1.SetRotation(0f);
                break;
            case 2:
                Bar1.SetPosition(new Vector2(416f,384f));
                Bar1.SetRotation(0f);
                break;

            case 3:
                Bar1.SetPosition(new Vector2(560f,28f));
                Bar1.SetRotation((float)(Math.PI / 2));
                break;
            case 4:
                Bar1.SetPosition(new Vector2(672f,28f));
                Bar1.SetRotation((float)(Math.PI / 2));
                break;
            case 5:
                Bar1.SetPosition(new Vector2(800f,28f));
                Bar1.SetRotation((float)(Math.PI / 2));
                break;

            case 6:
                Bar1.SetPosition(new Vector2(496f,92f));
                Bar1.SetRotation((float)(Math.PI / 4));
                break;
            case 7:
                Bar1.SetPosition(new Vector2(852f,92f));
                Bar1.SetRotation((float)(3 * Math.PI / 4));
                break;

            case 8:
                Bar1.SetPosition(new Vector2(672f,28f));
                Bar1.SetRotation((float)(Math.PI / 2));
                Bar2.Visible = true;
                Bar2.SetPosition(new Vector2(416f,264f));
                Bar2.SetRotation(0f);
                break;
            case 9:
                Bar1.SetPosition(new Vector2(496f,92f));
                Bar1.SetRotation((float)(Math.PI / 4));
                Bar2.Visible = true;
                Bar2.SetPosition(new Vector2(852f,92f));
                Bar2.SetRotation((float)(3 * Math.PI / 4));
                break;
        }
    }
}
