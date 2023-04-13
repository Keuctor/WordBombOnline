using System;
using System.Collections.Generic;
using UnityEngine;

public static class UserValidator
{
    
    public static bool IsValidName(string name)
    {
        if(string.IsNullOrEmpty(name)) return false;    

        if (name.Length < 3)
        {
            return false;
        }

        if (name.Length > 20)
        {
            return false;
        }
        
        if (name.Contains("(") || name.Contains("<") || name.Contains(">") || name.Contains(")") ||
            name.Contains(".") || name.Contains("{") || name.Contains("}"))
        {
            return false;
        }
        return true;
    }

    public static bool IsValidPassword(string password)
    {
        if(password.Length < 4) {
            return false;
        }
        else if(password.Length > 30) {
            return false; 
        }

        return true;
    }

    //New message received/sent
    //Cencore needed
    public static bool CheckPlayerMessageIsValid(string message, out string validatedMessage)
    {
        //if (message.Length == 0)
        //{
        //    newMessage = "";
        //    return false;
        //};

        //var messageParts = message.Split(' ');
        //for (int i = 0; i < messageParts.Length; i++)
        //{
        //    messageParts[i] = WordProvider.Censore(messageParts[i].ToUpper());
        //}
        //newMessage = string.Join(" ", messageParts);
        validatedMessage = message;
        return true;
    }

}
