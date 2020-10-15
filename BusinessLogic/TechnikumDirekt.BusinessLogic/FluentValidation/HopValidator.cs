using FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.FluentValidation
{
    public class HopValidator: AbstractValidator<Hop>
    {
        public HopValidator()
        {
            RuleFor(h => h).NotNull();
        }    
    }
}