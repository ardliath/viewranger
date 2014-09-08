﻿using Liath.ViewRanger.Exceptions;
using Liath.ViewRanger.RequestBuilders;
using Liath.ViewRanger.Responses;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Liath.ViewRanger.Tests.RequestBuilderTests.GetLastPositionRequestTests
{
    /// <summary>
    /// These tests represent when the xml has come back from this.MakeRequest();
    /// </summary>
    [TestFixture]
    public class SuccessXmlDownloadedTests
    {
        [TestCase("NoLocations.xml")]
        [TestCase("TwoLocations.xml")]
        public void Throws_when_invalid_number_of_locations(string filename)
        {
            var request = new Mock<GetLastPositionRequest>(Guid.NewGuid().ToString());
            request.CallBase = true;
            request.Setup(x => x.MakeRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(this.GetXDocument(filename));
            Assert.Throws<UnexpectedResponseException>(() => request.Object.Request());
        }

        #region Properties

        [Test]
        public void Check_Latitude()
        {
            var location = this.GetLocationFromSuccessfulResponse();
            Assert.AreEqual(52.2087M, location.Latitude);
        }

        [Test]
        public void Check_Speed()
        {
            var location = this.GetLocationFromSuccessfulResponse();
            Assert.AreEqual(1M, location.Speed);
        }

        [Test]
        public void Check_Heading()
        {
            var location = this.GetLocationFromSuccessfulResponse();
            Assert.AreEqual(270M, location.Heading);
        }

        [Test]
        public void Check_Altitude()
        {
            var location = this.GetLocationFromSuccessfulResponse();
            Assert.AreEqual(10M, location.Altitude);
        }

        [Test]
        public void Check_Date()
        {
            var location = this.GetLocationFromSuccessfulResponse();
            Assert.AreEqual(new DateTime(2011, 09, 22, 09, 52, 58), location.Date);
        }

        #endregion

        #region Nulls

        [Test]
        public void Check_Latitude_is_null_when_ommitted()
        {
            var location = this.GetLocationFromEmptyResponse();
            Assert.IsNull(location.Latitude);
        }

        [Test]
        public void Check_Longitude_is_null_when_ommitted()
        {
            var location = this.GetLocationFromEmptyResponse();
            Assert.IsNull(location.Longitude);
        }

        [Test]
        public void Check_Speed_is_null_when_ommitted()
        {
            var location = this.GetLocationFromEmptyResponse();
            Assert.IsNull(location.Speed);
        }

        [Test]
        public void Check_Altitude_is_null_when_ommitted()
        {
            var location = this.GetLocationFromEmptyResponse();
            Assert.IsNull(location.Altitude);
        }

        [Test]
        public void Check_Heading_is_null_when_ommitted()
        {
            var location = this.GetLocationFromEmptyResponse();
            Assert.IsNull(location.Heading);
        }

        [Test]
        public void Check_Date_is_null_when_ommitted()
        {
            var location = this.GetLocationFromEmptyResponse();
            Assert.IsNull(location.Date);
        }

        #endregion
        
        [TestCase("LATITUDE")]
        [TestCase("LONGITUDE")]
        [TestCase("DATE")]
        [TestCase("ALTITUDE")]
        [TestCase("SPEED")]
        [TestCase("HEADING")]
        public void Ensure_throws_when_value_cannot_be_parsed(string node)
        {            
            Assert.Throws<UnexpectedResponseException>(() =>
                {
                    var location = this.GetLocationFromInvalidResponse(node);
                });
        }

        private Location GetLocationFromSuccessfulResponse()
        {
            var request = new Mock<GetLastPositionRequest>(Guid.NewGuid().ToString());
            request.CallBase = true;
            request.Setup(x => x.MakeRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(this.GetXDocument("Successful.xml"));
            return request.Object.Request();
        }

        private Location GetLocationFromInvalidResponse(string nodeName)
        {
            var xml = this.GetXDocument("Successful.xml");
            xml.Descendants(nodeName).Single().Value = Guid.NewGuid().ToString();
            var request = new Mock<GetLastPositionRequest>(Guid.NewGuid().ToString());
            request.CallBase = true;
            request.Setup(x => x.MakeRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(xml);
            return request.Object.Request();
        }

        private Location GetLocationFromEmptyResponse()
        {
            var request = new Mock<GetLastPositionRequest>(Guid.NewGuid().ToString());
            request.CallBase = true;
            request.Setup(x => x.MakeRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(this.GetXDocument("Empty.xml"));
            return request.Object.Request();
        }

        private XDocument GetXDocument(string name)
        {
            return XDocument.Load(string.Concat(@"RequestBuilderTests\GetLastPositionRequestTests\RequestTests\SampleResponses\", name));
        }
    }
}
