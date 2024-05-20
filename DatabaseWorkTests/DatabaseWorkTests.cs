using DatabaseWork;
using Npgsql;

namespace DatabaseWorkTests
{
    [TestClass]
    public class DatabaseWorkTests
    {
        [TestMethod]
        public void ConnectionTestMethod1()
        {
            NpgsqlConnection con = DatabaseOperations.Connect("localhost", "5433", "maksNN", "maksNN", "Dating_Agency");
        }

        [TestMethod]
        [ExpectedException(typeof(ConnectionException))]
        public void ConnectionTestMethod2()
        {
            NpgsqlConnection con = DatabaseOperations.Connect("", "5433", "maksNN", "maksNN", "Dating_Agency");
        }

        [TestMethod]
        [ExpectedException(typeof(ConnectionException))]
        public void ConnectionTestMethod3()
        {
            NpgsqlConnection con = DatabaseOperations.Connect("localhost", "", "maksNN", "maksNN", "Dating_Agency");
        }

        [TestMethod]
        [ExpectedException(typeof(ConnectionException))]
        public void ConnectionTestMethod4()
        {
            NpgsqlConnection con = DatabaseOperations.Connect("localhost", "5433", "", "maksNN", "Dating_Agency");
        }

        [TestMethod]
        [ExpectedException(typeof(ConnectionException))]
        public void ConnectionTestMethod5()
        {
            NpgsqlConnection con = DatabaseOperations.Connect("localhost", "5433", "maksNN", "", "Dating_Agency");
        }

        [TestMethod]
        [ExpectedException(typeof(ConnectionException))]
        public void ConnectionTestMethod6()
        {
            NpgsqlConnection con = DatabaseOperations.Connect("localhost", "5433", "maksNN", "maksNN", "");
        }

        [TestMethod]
        public void AuthenticationTestMethod1()
        {
            bool expected = true;
            bool result = DatabaseOperations.Authenticate("superuser", "superuser");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AuthenticationTestMethod2()
        {
            bool expected = false;
            bool result = DatabaseOperations.Authenticate("superuser", "wrong_password");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AuthenticationTestMethod3()
        {
            bool expected = false;
            bool result = DatabaseOperations.Authenticate("wrong_login", "superuser");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AuthenticationTestMethod4()
        {
            bool expected = false;
            bool result = DatabaseOperations.Authenticate("wrong_login", "wrong_password");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetUserDataTestMethod1()
        {
            object expected = DatabaseOperations.GetUserData("superuser", "superuser");
        }

        [TestMethod]
        [ExpectedException(typeof(GetDataException))]
        public void GetUserDataTestMethod2()
        {
            object expected = DatabaseOperations.GetUserData("wrong_login", "superuser");
        }

        [TestMethod]
        [ExpectedException(typeof(GetDataException))]
        public void GetUserDataTestMethod3()
        {
            object expected = DatabaseOperations.GetUserData("superuser", "wrong_password");
        }

        [TestMethod]
        [ExpectedException(typeof(GetDataException))]
        public void GetUserDataTestMethod4()
        {
            object expected = DatabaseOperations.GetUserData("wrong_login", "wrong_password");
        }
    }
}