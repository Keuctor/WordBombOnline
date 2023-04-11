using System.Collections;
using UnityEngine;

namespace ilasm.WordBomb
{
    public static class KeyCodeUtilities
    {
        private static KeyCode[] allowedLetters;

        public static KeyCode[] GetAllowedLetters()
        {
            if (allowedLetters != null)
                return allowedLetters;
            allowedLetters = new KeyCode[]
            {
                KeyCode.Q,
                KeyCode.W,
                KeyCode.E,
                KeyCode.R,
                KeyCode.T,
                KeyCode.Y,
                KeyCode.U,
                KeyCode.I,
                KeyCode.O,
                KeyCode.P,
                KeyCode.A,
                KeyCode.S,
                KeyCode.D,
                KeyCode.F,
                KeyCode.G,
                KeyCode.H,
                KeyCode.J,
                KeyCode.K,
                KeyCode.L,
                KeyCode.Z,
                KeyCode.X,
                KeyCode.C,
                KeyCode.V,
                KeyCode.B,
                KeyCode.N,
                KeyCode.M,
                KeyCode.Comma,//Ö
                KeyCode.Period,//Ç
                KeyCode.LeftBracket,//Ğ
                KeyCode.RightBracket,//Ü
                KeyCode.Semicolon,//Ş
                KeyCode.Quote,//İ
            };
            return allowedLetters;
        }
    }
}