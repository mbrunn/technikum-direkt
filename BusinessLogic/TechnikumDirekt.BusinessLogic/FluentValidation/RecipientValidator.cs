using FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.FluentValidation
{
    public class RecipientValidator : AbstractValidator<Recipient>
    {
        public RecipientValidator()
        {
            RuleFor(r => r.PostalCode).Matches(@"^[A][-][\d]{4}$").When(r=>r.Country == "Austria" || r.Country == "Österreich");
            RuleFor(r => r.Street).Matches(@"^[A-Z][A-Za-z]*[ ]([\d]+[\w\/]*)*").When(r=>r.Country == "Austria" || r.Country == "Österreich");
            RuleFor(r => r.City).Matches(@"^[A-Z][A-Za-z- ]*$");
            RuleFor(r => r.Name).Matches(@"^[A-Z][A-Za-z- ]*$");
        }
    }
}