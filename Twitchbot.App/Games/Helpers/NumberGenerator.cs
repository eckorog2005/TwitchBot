using System;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Twitchbot.Games.Helpers{
    public class NumberGenerator : IRandomNumberGenerator{

        public NumberGenerator(){

        }

        
        //returns a random number from 0 to max value - 1
        public int RandomNumber(int max){
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[4];
            provider.GetBytes(byteArray);

            //convert 4 bytes to an integer
            var randomInteger = Math.Abs(BitConverter.ToInt32(byteArray, 0));
            return randomInteger % max;
        }
    }
}