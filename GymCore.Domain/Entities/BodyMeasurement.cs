using System;
using System.Collections.Generic;
using System.Text;
using GymCore.Domain.Common;
using GymCore.Domain.Enums;
using GymCore.Domain.Exceptions;

namespace GymCore.Domain.Entities
{
    public class BodyMeasurement : Entity
    {
        public Guid PhysicalAssessmentId { get; private set; }
        public PhysicalAssessment PhysicalAssessment { get; private set; } = null!;

        public MeasurementType MeasurementType { get; private set; }
        public decimal Value { get; private set; }
        public MeasurementUnit Unit { get; private set; } 

        protected BodyMeasurement()
        {
        }

        public BodyMeasurement(
            PhysicalAssessment physicalAssessment,
            MeasurementType measurementType,
            decimal value,
            MeasurementUnit unit)
        {
            if (physicalAssessment is null)
                throw new DomainException("A avaliação física da medida é obrigatória");

            if (value <= 0)
                throw new DomainException("O valor da medida deve ser maior que zero");

            PhysicalAssessment = physicalAssessment;
            PhysicalAssessmentId = physicalAssessment.Id;
            MeasurementType = measurementType;
            Value = value;
            Unit = unit;
        }

    }
}
