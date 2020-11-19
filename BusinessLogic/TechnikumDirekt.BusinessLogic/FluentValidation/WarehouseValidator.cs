using FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.FluentValidation
{
    public class WarehouseValidator : AbstractValidator<Warehouse>
    {
        public WarehouseValidator()
        {
            // TODO: should we include ÖÄÜ ? - good question :) sind die bei \w nicht dabei? :thinking:
            RuleFor(w => w.Description).Matches(@"^[\wÖÄÜöäü\d \-().]*$");
            RuleForEach(w => w.NextHops).SetValidator(new WarehouseNextHopsValidator());
        }
    }
}