using System;
using System.Collections.Generic;
using System.Text;
using GymCore.Domain.Common;
using GymCore.Domain.Exceptions;


namespace GymCore.Domain.Entities
{
    public class WorkoutExercise : Entity
    {
        public Guid WorkoutId {  get; private set; }
        public Workout Workout { get; private set; } = null!;

        public Guid ExerciseId { get; private set; }
        public Exercise Exercise { get; private set; } = null!;

        public int Sets { get; private set; } 
        public int Repetitions { get; private set; }
        public decimal? Load {  get; private set; }
        public int? RestInSeconds { get; private set; } 
        public string? Notes { get; private set; }  


        protected WorkoutExercise()
        {
        }

        public WorkoutExercise(
            Workout workout,
            Exercise exercise,
            int sets,
            int repetitions,
            decimal? load,
            int? restInSeconds,
            string? notes)

        {
            if (workout is null)
                throw new DomainException("O treino é obrigatório");

            if (exercise is null)
                throw new DomainException("O exercício é obrigatório");

            if (sets <= 0)
                throw new DomainException("O número de séries deve ser maior que zero");

            if (repetitions <= 0)
                throw new DomainException("O número de repetições deve ser maior do que zero");

            if (load < 0)
                throw new DomainException("A carga não pode ser negativa");

            if (restInSeconds < 0)
                throw new DomainException("O descanso não pode ser negativo");

            Workout = workout;
            WorkoutId = workout.Id;
            Exercise = exercise;
            ExerciseId = exercise.Id;
            Sets = sets;
            Repetitions = repetitions; 
            Load = load; 
            RestInSeconds = restInSeconds;
            Notes = notes?.Trim();
        }
    }
}
