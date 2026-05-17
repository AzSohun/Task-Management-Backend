using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Validators
{
    public class CreateTaskDtoValidator: AbstractValidator<CreateTaskDto>
    {

        public CreateTaskDtoValidator()
        {

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is Required")
                .MaximumLength(100).WithMessage("Title Must Not Exceed 100 Characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Title Must Not Exceed 100 Characters");

        }

    }
}
