using ChessChallenge.API;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

// Do Not Use the AsParallel() function;

public class MyBot : IChessBot
{

    int[] pieceValues = { 0, 1, 3, 3, 5, 9, 10 };
    String[] opening = { "e2e4", "e7e5", "g1f3", "b8c6", "f1b5" };

    public Move Think(Board board, Timer timer)
    {
        int plyCount = board.PlyCount;
        if (plyCount < 5) return new Move(opening[plyCount], board);


        Move[] allMoves = board.GetLegalMoves();

        Move moveToPlay = allMoves[0];
        int eval = 0;

        foreach (Move move in allMoves)
        {
            int e = EvaluateMove(board, move, 4);
            if (e > eval)
            {
                eval = e;
                moveToPlay = move;
            }
        }
        return moveToPlay;
    }

    int EvaluateMove(Board board, Move moveToMake, int depth)
    {
        if (depth == 0) return 0;
            
        bool white = board.IsWhiteToMove;
        PieceList pawns = board.GetPieceList(PieceType.Pawn, white);
        PieceList rooks = board.GetPieceList(PieceType.Rook, white);
        PieceList bishops = board.GetPieceList(PieceType.Bishop, white);
        PieceList knights = board.GetPieceList(PieceType.Knight, white);
        PieceList queens = board.GetPieceList(PieceType.Queen, white);
        int eval = pawns.Count + 3 * knights.Count + 3 * bishops.Count + 5 * rooks.Count + 9 * queens.Count;
        board.MakeMove(moveToMake);
        if (moveToMake.IsCapture) eval += 10;
        if (board.IsInCheck()) eval -= 200;
        if (board.IsInCheckmate()) eval -= 400;
        int bestOpponentEval = 0;
        Move bestOpponentMove;
        foreach (Move move in board.GetLegalMoves())
        {
            int e = EvaluateMove(board, move, depth-1);
            if (e > bestOpponentEval)
            {
                bestOpponentMove = move;
                bestOpponentEval = eval;
            }
        }

        eval -= bestOpponentEval;

        board.UndoMove(moveToMake);
        return eval;
    }

}