using System.Collections.Generic;
using FluentValidation;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.Tests
{
    public class TrackingLogicTests
    {
        private readonly Recipient _recipient1 = new Recipient
        {
            Name = "Michi Mango",
            Street = "TestStreet 1",
            PostalCode = "1234",
            City = "Mistelbach Weltstadt",
            Country = "AT"
        };
        
        private readonly Recipient _recipient2 = new Recipient
        {
            Name = "Benji Bananas",
            Street = "Banana Street 2",
            PostalCode = "4242",
            City = "Banana City",
            Country = "AT"
        };
        
        private const string ValidTrackingNumber = "A123BCD23";
        private const string InvalidTrackingNumber = "A123BaD23";
        private const string NotfoundTrackingNumber = "000000000";
        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";
        private const string NotfoundHopCode = "ABCD0000";

        [OneTimeSetUp]
        public void Setup()
        {
            WarehouseLogic.Warehouses = new List<Warehouse>
            {
                new Warehouse
                {
                    HopType = HopType.Warehouse,
                    Code = ValidHopCode,
                    Description = "Descriptive description",
                    ProcessingDelayMins = 100,
                    LocationName = "Benjis Hut",
                    LocationCoordinates = new Point(43.645074, -115.993081),
                    Level = 5,
                    NextHops = new List<WarehouseNextHops>()
                }
            };
        }
        
        /*#region ReportParcelDelivery Tests

        [Test]
        public void ReportParcelDelivery_DoesNotThrow_WithValidTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            tl.SubmitParcel(parcel);

            Assert.DoesNotThrow(() => tl.ReportParcelDelivery(parcel.TrackingId));
        }
        
        [Test]
        public void ReportParcelDelivery_Throws_WithNotfoundTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());

            Assert.Throws<TrackingLogicException>(() => tl.ReportParcelDelivery(NotfoundTrackingNumber));
        }
        
        [Test]
        public void ReportParcelDelivery_Throws_WithInvalidTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());

            Assert.Throws<ValidationException>(() => tl.ReportParcelDelivery(InvalidTrackingNumber));
        }

        #endregion

        #region ReportParcelHop Tests

        [Test]
        public void ReportParcelHop_DoesNotThrow_WithValidTrackingIdAndHopCode()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            tl.SubmitParcel(parcel);
            
            Assert.DoesNotThrow(() => tl.ReportParcelHop(parcel.TrackingId, ValidHopCode));
        }
        
        [Test]
        public void ReportParcelHop_Throws_WithNotfoundTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            
            Assert.Throws<TrackingLogicException>(() => tl.ReportParcelHop(NotfoundTrackingNumber, ValidHopCode));
        }
        
        [Test]
        public void ReportParcelHop_Throws_WithNotfoundHopCode()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            tl.SubmitParcel(parcel);
            
            Assert.Throws<TrackingLogicException>(() => tl.ReportParcelHop(parcel.TrackingId, NotfoundHopCode));
        }
        
        [Test]
        public void ReportParcelHop_Throws_WithInvalidTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            
            Assert.Throws<ValidationException>(() => tl.ReportParcelHop(InvalidTrackingNumber, ValidHopCode));
        }
        
        [Test]
        public void ReportParcelHop_Throws_WithInvalidHopCode()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            
            Assert.Throws<ValidationException>(() => tl.ReportParcelHop(ValidTrackingNumber, InvalidHopCode));
        }

        #endregion

        #region SubmitParcel Tests

        [Test]
        public void SubmitParcel_DoesNotThrow_WithValidParcel()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.DoesNotThrow(() => tl.SubmitParcel(parcel));
        }

        [Test]
        public void SubmitParcel_Throws_WithInvalidParcel()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            var parcel = new Parcel
            {
                Weight = -2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.Throws<ValidationException>(() => tl.SubmitParcel(parcel));
        }
        
        #endregion

        #region TrackParcel Tests

        [Test]
        public void TrackParcel_DoesNotThrow_WithValidAndExistingTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            tl.SubmitParcel(parcel);

            Parcel result = null;
            Assert.DoesNotThrow(() =>
            {
                result = tl.TrackParcel(parcel.TrackingId);
            });
            
            Assert.IsNotNull(result);
            Assert.AreEqual(result.TrackingId, parcel.TrackingId);
        }
        
        [Test]
        public void TrackParcel_Throws_WithNonexistentTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());

            Assert.Throws<TrackingLogicException>(() => tl.TrackParcel(NotfoundTrackingNumber));
        }
        
        [Test]
        public void TrackParcel_Throws_WithInvalidTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());

            Assert.Throws<ValidationException>(() => tl.TrackParcel(InvalidTrackingNumber));
        }

        #endregion

        #region TransitionParcelFromPartner Tests

        [Test]
        public void TransitionParcelFromPartner_DoesNotThrow_WithValidParcelAndTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.DoesNotThrow(() => tl.TransitionParcelFromPartner(parcel, ValidTrackingNumber));
        }
        
        [Test]
        public void TransitionParcelFromPartner_Throws_WithInuseTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.DoesNotThrow(() => tl.TransitionParcelFromPartner(parcel, ValidTrackingNumber));
            Assert.Throws<TrackingLogicException>(() => tl.TransitionParcelFromPartner(parcel, ValidTrackingNumber));
        }
        
        [Test]
        public void TransitionParcelFromPartner_Throws_WithInvalidTrackingId()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.Throws<ValidationException>(() => tl.TransitionParcelFromPartner(parcel, InvalidTrackingNumber));
        }
        
        [Test]
        public void TransitionParcelFromPartner_Throws_WithInvalidParcel()
        {
            var tl = new TrackingLogic(new ParcelValidator(), new RecipientValidator(), new HopArrivalValidator(), new HopValidator());
            var parcel = new Parcel
            {
                Weight = -2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.Throws<ValidationException>(() => tl.TransitionParcelFromPartner(parcel, ValidTrackingNumber));
        }

        #endregion*/
    }
}