using GymCore.Domain.Entities;

namespace GymCore.Application.Students
{
    public static class StudentMappings
    {
        public static StudentResponse ToResponse(this Student student)
        {
            return new StudentResponse(
                student.Id,
                student.User.Name,
                student.User.Email,
                MaskCpf(student.Cpf),
                student.BirthDate,
                student.Phone,
                student.User.IsActive);
        }

        private static string MaskCpf(string cpf)
        {
            return $"***.{cpf[3..6]}.{cpf[6..9]}-**";
        }
    }
}