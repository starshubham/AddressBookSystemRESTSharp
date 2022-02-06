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
                JObject jsonObj = new JObject();
                jsonObj.Add("firstname", v.FirstName);
                jsonObj.Add("lastname", v.LastName);
                jsonObj.Add("PhoneNumber", v.PhoneNumber);
                jsonObj.Add("address", v.Address);
                jsonObj.Add("city", v.City);
                jsonObj.Add("state", v.State);
                jsonObj.Add("zip", v.Zip);
                jsonObj.Add("email", v.Email);

                //Added parameters to the request object such as the content-type and attaching the jsonObj with the request
                request.AddParameter("application/json", jsonObj, ParameterType.RequestBody);

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

        /* UC24:- Ability to Update Entry in Address Book JSONServer and sync with Address Book Application Memory. 
                  - Use RESTSharp for REST Api Calls from MSTest Test Code.
        */

        [TestMethod]
        public void OnCallingPutAPI_ReturnContactObjects()
        {
            //Arrange
            //Initialize the request for PUT to add new employee
            RestRequest request = new RestRequest("/Contacts/4", Method.Put);
            JObject jsonObj = new JObject();
            jsonObj.Add("firstname", "Chanda");
            jsonObj.Add("lastname", "Devi");
            jsonObj.Add("PhoneNumber", "9855669988");
            jsonObj.Add("address", "Patahi");
            jsonObj.Add("city", "Jaunpur");
            jsonObj.Add("state", "UP");
            jsonObj.Add("zip", "222205");
            jsonObj.Add("email", "chanda@gmail.com");
            //Added parameters to the request object such as the content-type and attaching the jsonObj with the request
            request.AddParameter("application/json", jsonObj, ParameterType.RequestBody);

            //Act
            RestResponse response = client.ExecuteAsync(request).Result;

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Contact contact = JsonConvert.DeserializeObject<Contact>(response.Content);
            Assert.AreEqual("Chanda", contact.FirstName);
            Assert.AreEqual("Devi", contact.LastName);
            Assert.AreEqual("222205", contact.Zip);
            Console.WriteLine(response.Content);
        }

        /*UC25:- Ability to Delete Entry in Address Book JSONServer and sync with Address Book Application Memory.
                 - Use RESTSharp for REST Api Calls from MSTest Test Code.
         */
        [TestMethod]
        public void OnCallingDeleteAPI_ReturnSuccessStatus()
        {
            //Arrange
            //Initialize the request for PUT to add new employee
            RestRequest request = new RestRequest("/Contacts/5", Method.Delete);

            //Act
            RestResponse response = client.ExecuteAsync(request).Result;

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Console.WriteLine(response.Content);
        }

    }
}
