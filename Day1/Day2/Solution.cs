namespace AdventOfCode2022.Day2;
internal static class Solution
{
    private enum Hand
    {
        Rock,
        Paper,
        Scissors
    }

    public static async Task RunPart1()
    {
        var lines = await File.ReadAllLinesAsync("Day2\\day2input.txt");

        int score = 0;
        
        foreach (var line in lines)
        {
            Hand opponent = line[0] switch
            {
                'A' => Hand.Rock,
                'B' => Hand.Paper,
                'C' => Hand.Scissors
            };
            Hand me = line[2] switch
            {
                'X' => Hand.Rock,
                'Y' => Hand.Paper,
                'Z' => Hand.Scissors
            };

            switch (me)
            {
                case Hand.Rock: score += 1; break;
                case Hand.Paper: score += 2; break;
                case Hand.Scissors: score += 3; break;
            }

            if (me == opponent)
            {
                score += 3;
            }
            else
            {
                switch (opponent)
                {
                    case Hand.Rock when me == Hand.Paper: score += 6; break;
                    case Hand.Rock when me == Hand.Scissors: score += 0; break;
                    case Hand.Paper when me == Hand.Scissors: score += 6; break;
                    case Hand.Paper when me == Hand.Rock: score += 0; break;
                    case Hand.Scissors when me == Hand.Rock: score += 6; break;
                    case Hand.Scissors when me == Hand.Paper: score += 0; break;
                }
            }
        }

        Console.WriteLine($"Total Score: {score}");

    }

    public static async Task RunPart2()
    {
        var lines = await File.ReadAllLinesAsync("Day2\\day2input.txt");

        int score = 0;

        foreach (var line in lines)
        {
            Hand opponent = line[0] switch
            {
                'A' => Hand.Rock,
                'B' => Hand.Paper,
                'C' => Hand.Scissors
            };
            Hand me = line[2] switch
            {
                //Lose
                'X' when opponent == Hand.Rock => Hand.Scissors,
                'X' when opponent == Hand.Paper => Hand.Rock,
                'X' => Hand.Paper,
                //Draw
                'Y' => opponent,
                //Win
                'Z' when opponent == Hand.Rock => Hand.Paper,
                'Z' when opponent == Hand.Scissors => Hand.Rock,
                'Z' => Hand.Scissors
            };

            switch (me)
            {
                case Hand.Rock: score += 1; break;
                case Hand.Paper: score += 2; break;
                case Hand.Scissors: score += 3; break;
            }

            if (me == opponent)
            {
                score += 3;
            }
            else
            {
                switch (opponent)
                {
                    case Hand.Rock when me == Hand.Paper: score += 6; break;
                    case Hand.Rock when me == Hand.Scissors: score += 0; break;
                    case Hand.Paper when me == Hand.Scissors: score += 6; break;
                    case Hand.Paper when me == Hand.Rock: score += 0; break;
                    case Hand.Scissors when me == Hand.Rock: score += 6; break;
                    case Hand.Scissors when me == Hand.Paper: score += 0; break;
                }
            }
        }

        Console.WriteLine($"Total Score: {score}");

    }
}
