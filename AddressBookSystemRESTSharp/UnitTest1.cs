using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace AddressBookSystemRESTSharp
{
    [TestClass]
    public class UnitTest1
    {
        RestClient client;

        [TestInitialize]
        public void SetUp()
        {
            //Initialize the base URL to execute requests made by the instance
            client = new RestClient("http://localhost:3000");
        }
        private RestResponse GetContactList()
        {
            //Arrange
            //Initialize the request object with proper method and URL
            RestRequest request = new RestRequest("/Contacts", Method.Get);
            //Act
            // Execute the request
            RestSharp.RestResponse response = client.ExecuteAsync(request).Result;
            return response;
        }

        /* UC22:- Ability to Read Entries of Address Book from JSONServer.
                  - Use RESTSharp for REST Api Calls from MSTest Test Code.
                  - Update Address Book Memory of the Data retrieved from JSON Server.
        */
        [TestMethod]
        public void ReadEntriesFromJsonServer()
        {
            RestResponse response = GetContactList();
            // Check if the status code of response equals the default code for the method requested
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            // Convert the response object to list of employees
            List<Contact> employeeList = JsonConvert.DeserializeObject<List<Contact>>(response.Content);
            Assert.AreEqual(2, employeeList.Count);
            foreach (Contact c in employeeList)
            {
                Console.WriteLine($"Id: {c.Id}\tFullName: {c.FirstName} {c.LastName}\tPhoneNo: {c.PhoneNumber}\tAddress: {c.Address}\tCity: {c.City}\tState: {c.State}\tZip: {c.Zip}\tEmail: {c.Email}");
            }
        }

        /*UC23:- Ability to Add Multiple Entries to Address Book JSONServer and sync with Address Book Application Memory.
                 - Use RESTSharp for REST Api Calls from MSTest Test Code
         */

        [TestMethod]
        public void OnCallingPostAPIForAContactListWithMultipleContacts_ReturnContactObject()
        {
            // Arrange
            List<Contact> contactList = new List<Contact>();
            contactList.Add(new Contact { FirstName = "Priyanshu", LastName = "Singh", PhoneNumber = "789456123", Address = "Pandeypur", City = "Varanasi", State = "UP", Zip = "229554", Email = "ps@gmail.com" });
            contactList.Add(new Contact { FirstName = "Vishal", LastName = "Seth", PhoneNumber = "781654987", Address = "Charashu Chauraha", City = "Jaunpur", State = "UP", Zip = "442206", Email = "vs@gmail.com" });
            contactList.Add(new Contact { FirstName = "Ekta", LastName = "Kumbhare", PhoneNumber = "7856239865", Address = "Bajaj Nagar", City = "Pune", State = "Maharashtra", Zip = "442203", Email = "ek@gmail.com" });

            //Iterate the loop for each contact
            foreach (var v in contactList)
            {
                //Initialize the request for POST to add new contact
                RestRequest request = new RestRequest("/Contacts", Method.Post);
                request.RequestFormat = DataFormat.Json;

                //Added parameters to the request object such as the content-type and attaching the jsonObj with the request
                request.AddBody(v);

                //Act
                RestResponse response = client.ExecuteAsync(request).Result;

                //Assert
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Contact contact = JsonConvert.DeserializeObject<Contact>(response.Content);
                Assert.AreEqual(v.FirstName, contact.FirstName);
                Assert.AreEqual(v.LastName, contact.LastName);
                Assert.AreEqual(v.PhoneNumber, contact.PhoneNumber);
                Console.WriteLine(response.Content);
            }
        }

    }
}
