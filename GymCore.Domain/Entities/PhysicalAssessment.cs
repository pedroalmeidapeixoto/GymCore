using System;
using System.Collections.Generic;
using System.Text;
using GymCore.Domain.Common;
using GymCore.Domain.Exceptions;

namespace GymCore.Domain.Entities
{
    public class PhysicalAssessment : Entity
    {
        public Guid StudentId { get; private set; }
        public Student Student { get; private set; } = null!;

        public Guid PersonalTrainerId { get; private set; }
        public PersonalTrainer PersonalTrainer { get; private set; } = null!;

        public DateOnly AssessmentDate { get; private set; }
        public decimal Weight { get; private set; }
        public decimal Height { get; private set; }
        public decimal? BodyFatPercentage {  get; private set; }
        public decimal? MuscleMass { get; private set; }
        public string? Notes { get; private set; }

        protected PhysicalAssessment()
        {
        }

        public PhysicalAssessment(
            Student student,
            PersonalTrainer personalTrainer,
            DateOnly assessmentDate,
            decimal weight,
            decimal height,
            decimal? bodyFatPercentage,
            decimal? muscleMass,
            string? notes)

        {
            if (student is null)
                throw new DomainException("O aluno da avaliação é obrigatório");

            if (personalTrainer is null)
                throw new DomainException("O Personal Trainer da avaliação é obrigatório");

            if (weight <= 0)
                throw new DomainException("O peso deve ser maior do que zero");

            if (height <= 0)
                throw new DomainException("A altura deve ser maior do que zero");

            if (bodyFatPercentage < 0 || bodyFatPercentage > 100)
                throw new DomainException("O percentual de gordura deve estar entre 0 e 100");

            if (muscleMass < 0)
                throw new DomainException("A massa muscular não pode ser negativa");

            Student = student;
            StudentId = student.Id;
            PersonalTrainer = personalTrainer;
            PersonalTrainerId = personalTrainer.Id;
            AssessmentDate = assessmentDate; 
            Weight = weight;
            Height = height;
            BodyFatPercentage = bodyFatPercentage; 
            MuscleMass = muscleMass;
            Notes = notes?.Trim();

        }
    }
}
