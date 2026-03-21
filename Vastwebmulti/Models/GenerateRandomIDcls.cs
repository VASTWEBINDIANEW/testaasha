using System;

namespace Vastwebmulti.Models
{
    public static class GenerateRandomIDcls
    {

        public static string GenerateUniqueRandomNumber(string generatetype, int numberoflength)
        {
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";

            string characters = numbers;
            if (generatetype == "ALPHANUM")
            {
                characters += alphabets + small_alphabets + numbers;
            }
            int length = numberoflength;
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }
    }
}