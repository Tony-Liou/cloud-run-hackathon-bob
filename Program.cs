using System.Text.Json.Serialization;
using Action = CloudRunHackathonCsharp.Action;

const string N = "N";
const string E = "E";
const string S = "S";
const string W = "W";

var app = WebApplication.Create(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

var myName = "https://cloud-run-hackathon-go-n5xjuxfciq-uc.a.run.app";

app.MapGet("/", () => "Let the battle begin!");
app.MapPost("/", (ArenaUpdate model) =>
{
    Console.WriteLine(model);
    if (myName != model.Links.Self.Href)
    {
        myName = model.Links.Self.Href;
    }

    return Play(model);
});

app.Run($"http://0.0.0.0:{port}");

#region Local functions

string Play(ArenaUpdate input)
{
    var (board, myPos) = GetBoard(input.Arena.State, input.Arena.Dims[0], input.Arena.Dims[1]);
    var targetName = FindAttackableEnemy(board, myPos);
    if (string.IsNullOrEmpty(targetName))
    {
        var enemyPos = FindNearestEnemy(board, myPos);
        targetName = board[enemyPos.Y, enemyPos.X];
    }

    var action = TakeAction(myName, targetName, input.Arena.State);
    return action.ToString();
}

(string[,] board, Position myself) GetBoard(Dictionary<string, PlayerState> playerInfo, int x, int y)
{
    string[,] board = new string[y, x];
    Position myself = default;
    foreach (var pair in playerInfo)
    {
        if (pair.Key != myName)
        {
            board[pair.Value.Y, pair.Value.X] = pair.Key;
        }
        else
        {
            myself = new Position(pair.Value.X, pair.Value.Y);
        }
    }

    return (board, myself);
}

string? FindAttackableEnemy(string?[,] board, Position myself)
{
    for (int i = 1; i <= 3; i++)
    {
        int y = myself.Y - i;
        int x = myself.X;

        if (IsInside(x, y, board) && board[y, x] != "")
        {
            return board[y, x];
        }

        y = myself.Y + i;
        if (IsInside(x, y, board) && board[y, x] != "")
        {
            return board[y, x];
        }

        y = myself.Y;
        x = myself.X - i;
        if (IsInside(x, y, board) && board[y, x] != "")
        {
            return board[y, x];
        }

        x = myself.X + i;
        if (IsInside(x, y, board) && board[y, x] != "")
        {
            return board[y, x];
        }
    }

    return null;
}

static bool IsInside(int x, int y, string?[,] board)
{
    int row = board.GetLength(0);
    int col = board.GetLength(1);

    return x >= 0 && x < col && y >= 0 && y < row;
}

Action TakeAction(string attackerName, string targetName, Dictionary<string, PlayerState> playerInfo)
{
    var attacker = playerInfo[attackerName];
    var target = playerInfo[targetName];

    // Same column
    if (attacker.X == target.X)
    {
        // Below target
        if (attacker.Y > target.Y)
        {
            switch (attacker.Direction)
            {
                case N:
                    return Action.T;
                case E:
                    return Action.L;
                case S:
                    return Action.R;
                case W:
                    return Action.R;
            }
        }
        else // Beyond target
        {
            switch (attacker.Direction)
            {
                case N:
                    return Action.R;
                case E:
                    return Action.R;
                case S:
                    return Action.T;
                case W:
                    return Action.L;
            }
        }
    }
    else if (attacker.Y == target.Y) // Same row
    {
        if (attacker.X > target.X) // Right
        {
            switch (attacker.Direction)
            {
                case N:
                    return Action.L;
                case E:
                    return Action.L;
                case S:
                    return Action.R;
                case W:
                    return Action.T;
            }
        }
        else // Left
        {
            switch (attacker.Direction)
            {
                case N:
                    return Action.R;
                case E:
                    return Action.T;
                case S:
                    return Action.L;
                case W:
                    return Action.L;
            }
        }
    }
    else // Not in the same row or column
    {
        if (attacker.X > target.X) // Right
        {
            if (attacker.Y > target.Y) // Bottom right
            {
                switch (attacker.Direction)
                {
                    case N:
                        return Action.F;
                    case E:
                        return Action.L;
                    case S:
                        return Action.R;
                    case W:
                        return Action.F;
                }
            }
            else // Top right
            {
                switch (attacker.Direction)
                {
                    case N:
                        return Action.L;
                    case E:
                        return Action.R;
                    case S:
                        return Action.F;
                    case W:
                        return Action.F;
                }
            }
        }
        else
        {
            if (attacker.Y > target.Y) // Bottom left
            {
                switch (attacker.Direction)
                {
                    case N:
                        return Action.F;
                    case E:
                        return Action.F;
                    case S:
                        return Action.L;
                    case W:
                        return Action.R;
                }
            }
            else // Top left
            {
                switch (attacker.Direction)
                {
                    case N:
                        return Action.R;
                    case E:
                        return Action.F;
                    case S:
                        return Action.F;
                    case W:
                        return Action.L;
                }
            }
        }
    }

    return (Action)Random.Shared.Next(4);
}

// Use BFS to find the nearest enemy.
static Position FindNearestEnemy(string[,] board, Position myself)
{
    int[,] dir = { { 0, -1 }, { 0, 1 }, { -1, 0 }, { 1, 0 } };
    bool[,] visited = new bool[board.GetLength(0), board.GetLength(1)];

    Queue<Position> q = new();
    q.Enqueue(myself);
    visited[myself.Y, myself.X] = true;

    while (q.Count > 0)
    {
        var pos = q.Dequeue();

        for (int i = 0; i < 4; i++)
        {
            int x = pos.X + dir[i, 0];
            int y = pos.Y + dir[i, 1];
            if (IsInside(x, y, board) && !visited[y, x])
            {
                if (!string.IsNullOrEmpty(board[y, x]))
                {
                    return new Position(x, y);
                }

                q.Enqueue(new Position(x, y));
                visited[y, x] = true;
            }
        }
    }

    return default;
}

#endregion

#region Models

internal readonly struct Position
{
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }
}

internal record ArenaUpdate([property: JsonPropertyName("_links")] Links Links, Arena Arena);

internal record Links(Self Self);

internal record Self(string Href);

internal record Arena(List<int> Dims, Dictionary<string, PlayerState> State);

internal record PlayerState(int X, int Y, string Direction, bool WasHit, int Score);

#endregion