using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Testing;

namespace MoviesAPI.Tests
{
    [TestClass]
    public class TransferServiceTests
    {
        [TestMethod]
        public void WireTransferWithInsufficientFundsThrowsAnError()
        {
            //Preparation
            Account origin = new Account() { Funds = 0};
            Account destination = new Account() { Funds = 0};
            decimal amountToTranfer = 5m;
            var service = new TransferService();
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
            Assert.AreEqual("The origin account doesn't have enough funds.", expectedException.Message);
        }

        [TestMethod]
        public void WireTransferCorrectlyEditFunds()
        {
            //Preparation
            Account origin = new Account() { Funds = 10m};
            Account destination = new Account() { Funds = 5m};
            decimal amountToTranfer = 7m;
            var service = new TransferService();

            //Testing
            service.WireTransfer(origin, destination, amountToTranfer);

            //Verification
            Assert.AreEqual(3m, origin.Funds);
            Assert.AreEqual(12m, destination.Funds);
        }
    }
}
