using FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.FluentValidation
{
    public class WarehouseValidator: AbstractValidator<Warehouse>
    {
        public WarehouseValidator()
        {
            //TODO: should we include ÖÄÜ ?
            RuleFor(w => w.Description).Matches(@"^[\wÖÄÜöäü\d -]*$");
            RuleForEach(w => w.NextHops).SetValidator(new WarehouseNextHopsValidator());
        }
    }
}