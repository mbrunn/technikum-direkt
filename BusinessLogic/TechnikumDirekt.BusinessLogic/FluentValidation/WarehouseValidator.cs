using FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.FluentValidation
{
    public class WarehouseValidator: AbstractValidator<Warehouse>
    {
        public WarehouseValidator()
        {
            RuleFor(w => w.Description).Matches(@"^[\w\d -]*$");
        }
    }
}