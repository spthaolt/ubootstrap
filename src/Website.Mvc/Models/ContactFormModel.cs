using System.ComponentModel.DataAnnotations;

namespace Website.Mvc.Models
{ 
	public class ContactFormModel
	{
        [Required(ErrorMessage = "Please enter your name") ]
		public string Name {get; set;}

        public string Address1 {get; set;}

		public string Address2 {get; set;}

        [Required(ErrorMessage = "Please enter your email")]
        [DataType(DataType.EmailAddress)]
		public string Email {get; set;}

        [Required(ErrorMessage = "Please enter a message")]
		public string Message {get; set;}

        // Umbraco node properties (could also be passed as params to the macro?
        public int PageId { get; set; }
        public string ThankYouPageId { get; set; }
        public string EmailBody { get; set; }
        public string EmailReplyBody { get; set; }
        public string EmailTo { get; set; }
        public string EmailSubject { get; set; }
        public string EmailReplySubject { get; set; }

	}
}