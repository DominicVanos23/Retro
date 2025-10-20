using Microsoft.AspNetCore.Mvc;

namespace MajokoRentals.Controllers
{
    public class ChatbotController : Controller
    {
        [HttpGet]
        public IActionResult Chatbot()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendMessage(string message)
        {
            string response;

            if (string.IsNullOrWhiteSpace(message))
                response = "Please enter a message before sending.";
            else if (message.ToLower().Contains("hello") || message.ToLower().Contains("hi"))
                response = "Hello there! 👋 How can I assist you with MajokoRentals today?";
            else if (message.ToLower().Contains("rent"))
                response = "We offer a wide range of properties for rent. 🏠 You can browse listings under the 'Properties' section.";
            else if (message.ToLower().Contains("apply"))
                response = "To apply for a property, click 'Apply Now' on the property page and complete your details.";
            else if (message.ToLower().Contains("thank you") || message.ToLower().Contains("thanks"))
                response = "You're welcome! 😊";
            else if (message.ToLower().Contains("help"))
                response = "Sure! You can ask about rental listings, applications, or contact info.";
            else if (message.ToLower().Contains("contact"))
                response = "You can reach us at 📧 support@majokorentals.com or call 📞 +27 81 555 1234.";
            else
                response = "I'm still learning 🤖. Please contact support@majokorentals.com for detailed help.";

            return Json(new { reply = response });
        }
    }
}
