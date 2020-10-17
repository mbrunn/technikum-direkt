using FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.FluentValidation
{
    public class WarehouseValidator: AbstractValidator<Warehouse>
    {
        public WarehouseValidator()
        {
            // TODO - validate hop code??
            RuleFor(w => w.Description).Matches(@"^[\w\d -]*$");
        }
    }
}