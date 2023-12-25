using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class Main : Node2D
{
    private Node2D Cells;
    private Godot.Collections.Array CellsArray;
    private int TurnCount;
    private bool IsX;
    private bool IsGameOver;
    private Label DeclareLabel;
    private Label GameOverLabel;
    private TextureRect Slasher;
    private TextureRect TurnSign;
    private Button ReloadBtn;
    private Button QuitBtn;
    private Button NextBtn;
    private Texture Texture;
    private bool PlayerTurn;
    private float _delay;

    public override void _Ready()
    {
        base._Ready();
        _delay = 1.0f;
        PlayerTurn = true;
        IsGameOver = false;
        ReloadBtn = GetNode<Button>("Control/Buttons/Reload");
        QuitBtn = GetNode<Button>("Control/Buttons/Quit");
        NextBtn = GetNode<Button>("Control/Buttons/Next");
        Slasher = GetNode<TextureRect>("Slasher");
        TurnSign = GetNode<TextureRect>("Control/TurnSign");
        DeclareLabel = GetNode<Label>("Control/Labels/DeclareLabel");
        GameOverLabel = GetNode<Label>("Control/Labels/GameOverLabel");
        Texture = ResourceLoader.Load<Texture>("res://Assets/marks.png");
        TurnCount = 0;
        Cells = GetNode<Node2D>("Cells");
        CellsArray = Cells.GetChildren();
        Slasher.Visible = false;
        SetGrid();
        ReloadBtn.Connect("pressed", this, "ReloadScene");
        QuitBtn.Connect("pressed", this, "QuitGame");
    }

    private void UpdateTurn()
    {
        if (IsGameOver) return;
        float pos;
        //updates image on left side sign to show which is the current turn x or o
        AtlasTexture img = new AtlasTexture();
        img.Atlas = Texture;
        pos = IsX ? 200f : 100f;
        img.Region = new Rect2(pos, 0f, 100f, 100f);
        TurnSign.Texture = img;

        IsX = TurnCount % 2 == 0;
        Cell.IsPlayerTurn = IsX;
        if (!IsX)
        {
            PCPlayCell();
        }
    }

    private void QuitGame()
    {
        GetTree().Quit();
    }

    private void ReloadScene()
    {
        GetTree().ReloadCurrentScene();
    }

    private void SetGrid()
    {
        int count = 0;
        float size = 120f;
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (CellsArray[count] is Cell _cell)
                {
                    _cell.Position = new Vector2(col * size,row * size);
                    _cell.SetRow(row);
                    _cell.SetCol(col);
                    _cell.Connect("ClickSignal", this, "OnCellClicked");
                }
                count++;
            }
        }
    }

    private void OnCellClicked(Cell _cell)
    {
        if (IsGameOver)
        {
            return;
        }
        else
        {
            IsX = TurnCount % 2 == 0;
            Cell.IsPlayerTurn = IsX;
            _cell.MarkCell(IsX);
            CheckForWinner();
            TurnCount++;
            
            UpdateTurn();
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        foreach (Cell _cell in CellsArray)
        {
            _cell._CellInput(@event);
        }
    }

    private void CheckForWinner()
    {
        
        int patInt = -1;
        for (int i = 0; i < 8; i++)
        {
            string pattern = "";
            switch (i)
            {
                case 0:
                    pattern = ((Cell)CellsArray[0]).marker + ((Cell)CellsArray[1]).marker + ((Cell)CellsArray[2]).marker;
                    patInt = i;
                    break;
                case 1:
                    pattern = ((Cell)CellsArray[3]).marker + ((Cell)CellsArray[4]).marker + ((Cell)CellsArray[5]).marker;
                    patInt = i;
                    break;
                case 2:
                    pattern = ((Cell)CellsArray[6]).marker + ((Cell)CellsArray[7]).marker + ((Cell)CellsArray[8]).marker;
                    patInt = i;
                    break;
                case 3:
                    pattern = ((Cell)CellsArray[0]).marker + ((Cell)CellsArray[3]).marker + ((Cell)CellsArray[6]).marker;
                    patInt = i;
                    break;
                case 4:
                    pattern = ((Cell)CellsArray[1]).marker + ((Cell)CellsArray[4]).marker + ((Cell)CellsArray[7]).marker;
                    patInt = i;
                    break;
                case 5:
                    pattern = ((Cell)CellsArray[2]).marker + ((Cell)CellsArray[5]).marker + ((Cell)CellsArray[8]).marker;
                    patInt = i;
                    break;
                case 6:
                    pattern = ((Cell)CellsArray[0]).marker + ((Cell)CellsArray[4]).marker + ((Cell)CellsArray[8]).marker;
                    patInt = i;
                    break;
                case 7:
                    pattern = ((Cell)CellsArray[2]).marker + ((Cell)CellsArray[4]).marker + ((Cell)CellsArray[6]).marker;
                    patInt = i;
                    break;
            }

            if (pattern == "xxx")
            {
                DeclareWinner("x");
                DrawSlash(patInt);
            }
            else if (pattern == "ooo")
            {
                DeclareWinner("o");
                DrawSlash(patInt);
            }
            else
            {
                if (!IsGameOver)
                {
                    CheckDraw();
                }
            }
        }
    }

    private void DeclareWinner(string winner)
    {
        DeclareLabel.Text = winner + " wins!";
        DeclareGameOver();
    }

    private void CheckDraw()
    {
        foreach (Cell _cell in CellsArray)
        {
            if (_cell.marker == "")
            {
                return;
            }
        }
        DeclareDraw();
    }

    private void DeclareDraw()
    {
        DeclareLabel.Text = "It's a draw!";
        DeclareGameOver();
    }

    private void DeclareGameOver()
    {
        IsGameOver = true;
        GameOverLabel.Text = "GAME OVER!";
    }

    private void DrawSlash(int pattern)
    {
        Slasher.Visible = true;

        switch (pattern)
        {
            case -1:
                break;

            case 0:
                Slasher.SetRotation(0f);
                Slasher.SetPosition(new Vector2(348f,168f));
                break;
            case 1:
                Slasher.SetRotation(0f);
                Slasher.SetPosition(new Vector2(348f,280f));
                break;
            case 2:
                Slasher.SetRotation(0f);
                Slasher.SetPosition(new Vector2(348f,404f));
                break;


            case 3:
                Slasher.SetRotation((float)Math.PI * 0.5f);
                Slasher.SetPosition(new Vector2(412f,120f));
                break;
            case 4:
                Slasher.SetRotation((float)Math.PI * 0.5f);
                Slasher.SetPosition(new Vector2(528f,120f));
                break;
            case 5:
                Slasher.SetRotation((float)Math.PI * 0.5f);
                Slasher.SetPosition(new Vector2(644f,120f));
                break;

            case 6:
                Slasher.SetRotation((float)Math.PI * 0.25f);
                Slasher.SetPosition(new Vector2(404f,160f));
                break;
            case 7:
                Slasher.SetRotation((float)Math.PI * -0.258f);
                Slasher.SetPosition(new Vector2(400f,410f));
                break;
        }
    }

    private Cell EmptyCell()
    {
        List<Cell> emptyCells = AvailableCellsList();
        int size = emptyCells.Count;
        Random random = new Random();
        Cell _cell = emptyCells[random.Next(size)];
        return _cell;
    }

    private async void PCPlayCell()
    {
        Cell _cell = EmptyCell();
        await ToSignal(GetTree().CreateTimer(_delay), "timeout");
        OnCellClicked(_cell);

    }

    private List<Cell> AvailableCellsList()
    {
        List<Cell> availableCells = new List<Cell>();
        foreach(Cell _cell in CellsArray)
        {
            if (!_cell.HasMark) availableCells.Add(_cell);
        }

        return availableCells;
    }

}
