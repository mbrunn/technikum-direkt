using FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.FluentValidation
{
    public class ParcelValidator : AbstractValidator<Parcel>
    {
        public ParcelValidator()
        {
            RuleFor(p => p.Weight).GreaterThan(0.0f);
            
            RuleSet("trackingId", () =>
            {
                RuleFor(p => p.TrackingId).Matches(@"^[A-Z0-9]{9}$");
            });
            
            RuleFor(p => p.TrackingId).Matches(@"^[A-Z0-9]{9}$");
            RuleFor(p => p.Recipient).SetValidator(new RecipientValidator());
            RuleFor(p => p.Sender).SetValidator(new RecipientValidator());
            
            RuleFor(p => p.VisitedHops).NotNull().ForEach(vh =>
            {
                vh.SetValidator(new HopArrivalValidator());
            });
            RuleFor(p => p.FutureHops).NotNull().ForEach(fh =>
            {
                fh.SetValidator(new HopArrivalValidator());
            });
        }
    }
}