using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;
using TechnikumDirekt.DataAccess.Sql;

namespace TechnikumDirekt.DataAccess.Tests
{
    public class ParcelRepositoryTests
    {
        private ITechnikumDirektContext _technikumDirektContext;
        private IParcelRepository _parcelRepository;
        private List<Parcel> _entities;
        private List<HopArrival> _hopArrivals;
        private ILogger<ParcelRepository> _logger;

        private const string ValidTrackingNumber = "A123BCD23";
        private const string ValidTrackingNumber2 = "B123BCD56";
        private const string InvalidTrackingNumber = "A123BaD23";
        private const string NotfoundTrackingNumber = "000000000";
        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";
        private const string NotfoundHopCode = "ABCD0000";
        private Parcel _validParcel;

        private readonly Recipient _recipient1 = new Recipient
        {
            Name = "Michi Mango", Street = "TestStreet 1", PostalCode = "1234", City = "Mistelbach Weltstadt",
            Country = "AT"
        };

        private readonly Recipient _recipient2 = new Recipient
        {
            Name = "Benji Bananas", Street = "Banana Street 2", PostalCode = "4242", City = "Banana City",
            Country = "AT"
        };

        [SetUp]
        public void Setup()
        {
            _validParcel = new Parcel
            {
                TrackingId = ValidTrackingNumber,
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                HopArrivals = new List<HopArrival>
                    {new HopArrival {HopCode = ValidHopCode, ParcelTrackingId = ValidTrackingNumber}}
            };

            _entities = new List<Parcel>()
            {
                _validParcel
            };

            _hopArrivals = new List<HopArrival>
            {
                new HopArrival {HopCode = ValidHopCode, ParcelTrackingId = ValidTrackingNumber}
            };

            var dbMock = new Mock<ITechnikumDirektContext>();
            dbMock.Setup(p => p.Parcels).Returns(DbContextMock.GetQueryableMockDbSet<Parcel>(_entities));
            dbMock.Setup(p => p.SaveChanges()).Returns(1);
            dbMock.Setup(p => p.HopArrivals).Returns(DbContextMock.GetQueryableMockDbSet(_hopArrivals));

            dbMock.Setup(p => p.Parcels.Find(It.IsAny<object[]>()))
                .Returns<object[]>((keyValues) =>
                    _entities.FirstOrDefault(y => y.TrackingId == (string) keyValues.GetValue(0)));

            _technikumDirektContext = dbMock.Object;
            _logger = NullLogger<ParcelRepository>.Instance;
        }

        #region GetByTrackingId

        [Test]
        public void GetByTrackingId_ReturnsValidParcel_WithValidTrackingId()
        {
            _parcelRepository = new ParcelRepository(_technikumDirektContext, _logger);
            var parcel = _parcelRepository.GetByTrackingId(ValidTrackingNumber);
            Assert.NotNull(parcel);
            Assert.AreSame(_validParcel, parcel);
        }

        [Test]
        public void GetByTrackingId_ReturnsNull_WithInValidTrackingId()
        {
            _parcelRepository = new ParcelRepository(_technikumDirektContext, _logger);
            var parcel = _parcelRepository.GetByTrackingId(InvalidTrackingNumber);
            Assert.Null(parcel);
        }

        #endregion

        #region Update

        [Test]
        public void Update_DoesNotThrow_WithValidParcel()
        {
            _parcelRepository = new ParcelRepository(_technikumDirektContext, _logger);
            var updatedParcel = _validParcel;
            updatedParcel.Weight = _validParcel.Weight + 1.0f;
            Assert.DoesNotThrow(() => _parcelRepository.Update(_validParcel));
        }

        #endregion

        #region Add

        [Test]
        public void Add_ReturnsTrackingId_WithValidParcel()
        {
            _parcelRepository = new ParcelRepository(_technikumDirektContext, _logger);
            var trackingId = _parcelRepository.Add(_validParcel);
            Assert.AreEqual(_validParcel.TrackingId, trackingId);
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_DoesNotThrow_WithValidParcel()
        {
            _parcelRepository = new ParcelRepository(_technikumDirektContext, _logger);
            Assert.DoesNotThrow(() => _parcelRepository.Delete(_validParcel));
        }

        [Test]
        public void Delete_DoesNotThrow_WithValidTrackingId()
        {
            _parcelRepository = new ParcelRepository(_technikumDirektContext, _logger);
            Assert.DoesNotThrow(() => _parcelRepository.Delete(_validParcel.TrackingId));
        }

        [Test]
        public void Delete_DoesThrow_WithValidParcel()
        {
            //???
        }

        #endregion
    }
}