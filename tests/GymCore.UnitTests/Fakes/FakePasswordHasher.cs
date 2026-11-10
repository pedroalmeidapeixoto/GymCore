using GymCore.Application.Common;

namespace GymCore.UnitTests.Fakes
{
    public class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return "hashed:" + password;
        }
    }
}