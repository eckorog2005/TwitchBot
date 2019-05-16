using System;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Twitchbot.Games.Helpers{
    public interface IRandomNumberGenerator{
        int RandomNumber(int max);
    }
}