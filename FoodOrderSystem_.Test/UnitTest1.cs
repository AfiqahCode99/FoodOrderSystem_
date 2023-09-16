using FoodOrderSystem_.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Net.Http;

namespace FoodOrderSystem.Tests
{
    [TestClass]
    public class FoodControllerTests
    {
        private FoodController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Initialize your controller with a test connection string
            _controller = new FoodController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            // Optionally, you can use an in-memory database or a test database for testing
            // Initialize your test database or mock the database connection
        }

        [TestMethod]
        public void Get_ReturnsFoodList()
        {
            // Act
            var response = _controller.Get();
            var content = response.Content.ReadAsStringAsync().Result;
            var dataTable = JsonConvert.DeserializeObject<DataTable>(content);

            // Assert
            Assert.IsNotNull(dataTable);
            Assert.IsTrue(dataTable.Rows.Count > 0);
        }

        [TestMethod]
        public void GetByID_ValidId_ReturnsFoodItem()
        {
            // Arrange
            int validFoodId = 1; // Replace with a valid FoodID from your test data

            // Act
            var response = _controller.GetByID(validFoodId);
            var content = response.Content.ReadAsStringAsync().Result;
            var dataTable = JsonConvert.DeserializeObject<DataTable>(content);

            // Assert
            Assert.IsNotNull(dataTable);
            Assert.IsTrue(dataTable.Rows.Count == 1);
        }

        [TestMethod]
        public void GetByID_InvalidId_ReturnsNoContent()
        {
            // Arrange
            int invalidFoodId = -1; // Use an invalid FoodID

            // Act
            var response = _controller.GetByID(invalidFoodId);

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
