using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Testing;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class TransferServiceMoqTests
    {
        [TestMethod]
        public void WireTransferWithInsufficientFundsThrowsAnError()
        {
            //Preparation
            Account origin = new Account() { Funds = 0};
            Account destination = new Account() { Funds = 0};
            decimal amountToTranfer = 5m;

            //NOTE: Using a Moq
            string customErrorMessage = "custom error message";
            var mockValidateWireTransfer = new Mock<IValidateWireTransfer>();
            mockValidateWireTransfer.Setup(x => x.Validate(origin, destination, amountToTranfer))
                .Returns(new OperationResult(false, customErrorMessage));

            var service = new TransferServiceValidation(mockValidateWireTransfer.Object);
            Exception expectedException = null;

            //Testing
            try
            {
                service.WireTransfer(origin, destination, amountToTranfer);
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }

            //Verification
            if (expectedException is null) { Assert.Fail("An exception was expected"); }

            Assert.IsTrue(expectedException is ApplicationException);
            Assert.AreEqual(customErrorMessage, expectedException.Message);
        }

        [TestMethod]
        public void WireTransferCorrectlyEditFunds()
        {
            //Preparation
            Account origin = new Account() { Funds = 10m};
            Account destination = new Account() { Funds = 5m};
            decimal amountToTranfer = 7m;

            //NOTE: Using a Moq
            var mockValidateWireTransfer = new Mock<IValidateWireTransfer>();
            mockValidateWireTransfer.Setup(x => x.Validate(origin, destination, amountToTranfer))
                .Returns(new OperationResult(true));
            var service = new TransferServiceValidation(mockValidateWireTransfer.Object);

            //Testing
            service.WireTransfer(origin, destination, amountToTranfer);

            //Verification
            Assert.AreEqual(3m, origin.Funds);
            Assert.AreEqual(12m, destination.Funds);
        }

    }
}