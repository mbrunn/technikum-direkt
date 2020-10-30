using FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.FluentValidation
{
    public class WarehouseNextHopsValidator : AbstractValidator<WarehouseNextHops>
    {
        public WarehouseNextHopsValidator()
        {
            RuleFor(wnh => wnh.Hop).NotNull().SetValidator(new HopValidator());
        }
    }
}