using FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.FluentValidation
{
    public class ParcelValidator : AbstractValidator<Parcel>
    {
        public ParcelValidator()
        {
            RuleFor(p => p.Weight).GreaterThan(0);
            RuleFor(p => p.TrackingId).Matches(@"^[A-Z0-9]{9}$");
        }
    }
}