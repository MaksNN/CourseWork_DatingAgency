using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PasswordValidity;

namespace PasswordValidityTests
{
    [TestClass]
    public class PasswordValidityTests
    {
        [TestMethod]
        public void EmptyStringTestMethod()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RequiredLengthTestMethod1()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("maks");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RequiredLengthTestMethod2()
        {
            bool expected = true;
            bool result = Password.CheckPasswordValidation("maksNN1");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RequiredLengthTestMethod3()
        {
            bool expected = true;
            bool result = Password.CheckPasswordValidation("maksNN123");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RequiredLengthTestMethod4()
        {
            bool expected = true;
            bool result = Password.CheckPasswordValidation("maksNNnewPassword1234565432134");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RequiredLengthTestMethod5()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("maksNNnewPassword1234565432134saw");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CharTypeTestMethod1()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("максНН123");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CharTypeTestMethod2()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("максNN123");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CharTypeTestMethod3()
        {
            bool expected = true;
            bool result = Password.CheckPasswordValidation("maksNN123");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AcceptableSpecCharTestMethod1()
        {
            bool expected = true;
            bool result = Password.CheckPasswordValidation("maksNN123!-_,.?*");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AcceptableSpecCharTestMethod2()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("maksNN123&");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AcceptableDigitsTestMethod1()
        {
            bool expected = true;
            bool result = Password.CheckPasswordValidation("maksNN1234");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AcceptableDigitsTestMethod2()
        {
            bool expected = true;
            bool result = Password.CheckPasswordValidation("maksNN123456");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AcceptableDigitsTestMethod3()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("maksNN1234567");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AcceptableDigitsTestMethod4()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("maksNNCar");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NotAcceptableSpacesTestMethod1()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("maksNN Car");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NotAcceptableSpacesTestMethod2()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation(" maksNNCar");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NotAcceptableSpacesTestMethod3()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("maksNNCar ");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void LowercaseCharsPresenceTestMethod1()
        {
            bool expected = true;
            bool result = Password.CheckPasswordValidation("maksNNcar1");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void LowercaseCharsPresenceTestMethod2()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("MAKSNN1");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UppercaseCharsPresenceTestMethod1()
        {
            bool expected = true;
            bool result = Password.CheckPasswordValidation("maksNNCAR1");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UppercaseCharsPresenceTestMethod2()
        {
            bool expected = false;
            bool result = Password.CheckPasswordValidation("maksnn1");
            Assert.AreEqual(expected, result);
        }
    }
}
