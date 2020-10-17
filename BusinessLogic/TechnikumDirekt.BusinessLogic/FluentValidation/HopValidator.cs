using FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.FluentValidation
{
    public class HopValidator: AbstractValidator<Hop>
    {
        public HopValidator()
        {
            /*RuleSet("code", () =>
            {
                RuleFor(h => h.Code).Matches(@"^[A-Z]{4}\\d{1,4}$");
            });
            RuleFor(h => h.Code).Matches(@"^[A-Z]{4}\\d{1,4}$");*/
            RuleFor(h => h).NotNull();
            /*
            RuleFor(h => h.LocationCoordinates).NotNull();
            */
        }    
    }
}