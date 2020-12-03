using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.Services.Controllers;
using TechnikumDirekt.Services.Mapper;
using BlParcel = TechnikumDirekt.BusinessLogic.Models.Parcel;
using BlRecipient = TechnikumDirekt.BusinessLogic.Models.Recipient;

namespace TechnikumDirekt.Services.Tests
{
    [TestFixture]
    public class ParcelWebhookApiTests
    {
        private ITrackingLogic _trackingLogic;
        private IMapper _mapper;
        private NullLogger<ParcelWebhookApiController> _logger;

        private const string TrackingIdWithWebhooks = "ABCD12345";
        private const string ValidTrackingId = "DCBA12345";
        private const string InValidTrackingId = "ZYSCA5432";
        private const string NonExistingTrackingId = "NOTEX5432";
        private const string ValidUrl = "http://www.yeetmann-gruppe.at";

        //consts for UnsubscribeParcelWebhook
        private const long ExistingWebhookId = 1;
        private const long NonExistingWebhookId = 2;
        
        
        
        private readonly BlRecipient _recipient1 = new BlRecipient
        {
            Name = "Michi Mango",
            Street = "TestStreet 1",
            PostalCode = "1234",
            City = "Mistelbach Weltstadt",
            Country = "AT"
        };

        private readonly BlRecipient _recipient2 = new BlRecipient
        {
            Name = "Benji Bananas",
            Street = "Banana Street 2",
            PostalCode = "4242",
            City = "Banana City",
            Country = "AT"
        };
        
        [OneTimeSetUp]
        public void Setup()
        {
            var mockMapperConfig = new MapperConfiguration(c => c.AddProfile(new BlMapperProfile()));
            _mapper = new AutoMapper.Mapper(mockMapperConfig);

            var validParcel = new BlParcel
            {
                Weight = 2.0f,
                Sender = _mapper.Map<BlRecipient>(_recipient1),
                Recipient = _mapper.Map<BlRecipient>(_recipient2),
                State = BlParcel.StateEnum.InTransportEnum
            };
            
            var blWebhook1 = new Webhook()
            {
                CreationDate = DateTime.Now,
                Id = 1,
                Parcel = validParcel,
                Url = ValidUrl
            };
            
            var blWebhook2 = new Webhook()
            {
                CreationDate = DateTime.Now,
                Id = 2,
                Parcel = validParcel,
                Url = ValidUrl
            };

            var blWebhooks = new List<Webhook>()
            {
                blWebhook1,
                blWebhook2
            };
            
            var mockTrackingLogic = new Mock<ITrackingLogic>();
            // Setup - GetAllSubscribersByTrackingId
            mockTrackingLogic.Setup(tl => tl.GetAllSubscribersByTrackingId(TrackingIdWithWebhooks))
                .Returns(blWebhooks);
            mockTrackingLogic.Setup(tl => tl.GetAllSubscribersByTrackingId(NonExistingTrackingId))
                .Throws<BusinessLogicNotFoundException>();
            mockTrackingLogic.Setup(tl => tl.GetAllSubscribersByTrackingId(InValidTrackingId))
                .Throws<BusinessLogicValidationException>();
            
            // Setup - RemoveParcelWebhook
            mockTrackingLogic.Setup(tl => tl.RemoveParcelWebhook(ExistingWebhookId));
            mockTrackingLogic.Setup(tl => tl.RemoveParcelWebhook(NonExistingWebhookId))
                .Throws<BusinessLogicNotFoundException>();
           
            // Setup - SubscribeParcelWebhook
            mockTrackingLogic.Setup(tl => tl.SubscribeParcelWebhook(ValidTrackingId, ValidUrl)).Returns(blWebhook1);
            mockTrackingLogic.Setup(tl => tl.SubscribeParcelWebhook(NonExistingTrackingId, ValidUrl)).Throws<BusinessLogicNotFoundException>();
            mockTrackingLogic.Setup(tl => tl.SubscribeParcelWebhook(InValidTrackingId, ValidUrl)).Throws<BusinessLogicValidationException>();

            
            _trackingLogic = mockTrackingLogic.Object;
            _logger = NullLogger<ParcelWebhookApiController>.Instance;
        }
        
        [Test]
        public void ListParcelWebhooks_TrackingIdWithWebhooks_Ok()
        {
            //Arrange
            var controller = new ParcelWebhookApiController(_mapper, _logger, _trackingLogic);
            
            //Act
            var response = controller.ListParcelWebhooks(TrackingIdWithWebhooks);
            
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
        }

        [Test]
        public void ListParcelWebhooks_InValidTrackingId_NotFound()
        {
            //Arrange
            var controller = new ParcelWebhookApiController(_mapper, _logger, _trackingLogic);
            
            //Act
            var response = controller.ListParcelWebhooks(InValidTrackingId);
            
            //Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public void ListParcelWebhooks_NullTrackingId_NotFound()
        {
            //Arrange
            var controller = new ParcelWebhookApiController(_mapper, _logger, _trackingLogic);
            
            //Act
            var response = controller.ListParcelWebhooks(NonExistingTrackingId);
            
            //Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(response);
        }

        //--------------------------------------------------------------------------------------------------------------

        [Test]
        public void SubscribeParcelWebhook_ValidTrackingId_Ok()
        {
            //Arrange
            var controller = new ParcelWebhookApiController(_mapper, _logger, _trackingLogic);
            
            //Act
            var response = controller.SubscribeParcelWebhook(ValidTrackingId, ValidUrl);
            
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
        }

        [Test]
        public void SubscribeParcelWebhook_NonExistingTrackingId_NotFound()
        {
            //Arrange
            var controller = new ParcelWebhookApiController(_mapper, _logger, _trackingLogic);
            
            //Act
            var response = controller.SubscribeParcelWebhook(NonExistingTrackingId, ValidUrl);
            
            //Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(response);
        }

        [Test]
        public void SubscribeParcelWebhook_InValidTrackingId_NotFound()
        {
            //Arrange
            var controller = new ParcelWebhookApiController(_mapper, _logger, _trackingLogic);
            
            //Act
            var response = controller.SubscribeParcelWebhook(InValidTrackingId, ValidUrl);
            
            //Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [Test]
        public void UnsubscribeParcelWebhook_ExistingWebhookId_Ok()
        {
            //Arrange
            var controller = new ParcelWebhookApiController(_mapper, _logger, _trackingLogic);
            
            //Act
            var response = controller.UnsubscribeParcelWebhook(ExistingWebhookId);
            
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
        }

        [Test]
        public void UnsubscribeParcelWebhook_NonExistingWebhookId_NotFound()
        {
            //Arrange
            var controller = new ParcelWebhookApiController(_mapper, _logger, _trackingLogic);
            
            //Act
            var response = controller.UnsubscribeParcelWebhook(NonExistingWebhookId);
            
            //Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(response);
        }
    }
}