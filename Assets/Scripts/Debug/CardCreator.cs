
using System;
using System.Collections.Generic;
using System.Text;
using Random = UnityEngine.Random;

public static class CardCreator
{
    private static int[] directionX = { 1, 0, -1, 0 };
    private static int[] directionY = { 0, 1, 0, -1 };

    public static int[,] CreateMap(int len)
    {
        int[,] grid = new int[len, len];

        // 随机选择一个点作为起始点，格子作为1，然后走完所有的格子，每个格子走一次，
        // 保证每个格子都能走到，最后一个格子走到起始点，保证是一个环

        // 不是所有的起点都可以作为起始点，需要删除一些点，保证有解
        // 计算每个点的联通点个数，一个格子如果有N个方向可以走，这个格子的联通点个数就是N
        // 计算所有格子的联通点个数，如果联通点的总数，联通点为偶数的点总数是偶数，那么所有格子都可以作为起始点
        // 如果起点的联通数为奇数，那么需要

        int startX, startY;

        // 1. 随机选择一个点作为起始点
        startX = UnityEngine.Random.Range(0, len);
        startY = UnityEngine.Random.Range(0, len);
        grid[startX, startY] = 1;


        // 开始走地图
        if (FindPathWithStack(grid, startX, startY, 1))
        {
            return grid;
        }

        return null;
    }

    private static void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    private static bool IsSafe(int[,] grid, int x, int y)
    {
        int N = grid.GetLength(0);
        return x >= 0 && x < N && y >= 0 && y < N && grid[x, y] == 0;
    }


    // 递归版本，最大支持6，>= 7, Unity就会卡死
    private static bool FindPath(int[,] grid, int x, int y, int moveCount)
    {
        int N = grid.GetLength(0);

        // 走完所有的格子
        if (moveCount == N * N)
        {
            return true;
        }

        int[] direction = { 0, 1, 2, 3 };
        Shuffle(direction);

        foreach (int i in direction)
        {
            int newX = x + directionX[i];
            int newY = y + directionY[i];

            if (IsSafe(grid, newX, newY))
            {
                grid[newX, newY] = moveCount + 1;

                if (FindPath(grid, newX, newY, moveCount + 1))
                {
                    return true;
                }

                grid[newX, newY] = 0;
            }
        }

        return false;
    }

    // 非递归版本，最大可以支持到8,9很慢，10会卡死
    private static bool FindPathWithStack(int[,] grid, int x, int y, int moveCount)
    {
        int N = grid.GetLength(0);

        Stack<(int, int, int)> stack = new Stack<(int x, int y, int moveCount)>();
        stack.Push((x, y, 1));

        while (stack.Count > 0)
        {
            (x, y, moveCount) = stack.Pop();

            if (moveCount == N * N)
            {
                return true;
            }

            bool moved = false;

            int[] direction = { 0, 1, 2, 3 };

            Shuffle(direction);

            foreach (int i in direction)
            {
                int newX = x + directionX[i];
                int newY = y + directionY[i];

                if (IsSafe(grid, newX, newY))
                {
                    grid[newX, newY] = moveCount + 1;
                    stack.Push((newX, newY, moveCount + 1));
                    moved = true;
                    break;
                }
            }

            if (!moved && stack.Count > 0)
            {
                var (backtrackX, backTrackY, backtrackMove) = stack.Pop();
                grid[backtrackX, backTrackY] = 0;
            }
        }

        return false;
    }

    #region For Debug
    private static string PrintGrid(int[,] grid)
    {
        StringBuilder sb = new StringBuilder();
        int N = grid.GetLength(0);
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                sb.Append(grid[i, j]);
                sb.Append(',');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
    #endregion
}
