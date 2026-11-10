namespace GymCore.Application.Users
{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
    }
}