using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Talabat.APIs.Dtos
{
	public class RegisterDto
	{
		[Required]
		public string DisplayName { get; set; } = null!;

		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;

		[Required]
		public string Phone { get; set; } = null!;
		[Required]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}$", ErrorMessage = "password must have 1 uppercase,1 lowercase,1 number , 1 nonalphanumeric, and at least 6 characters")]
		public string Password { get; set; } = null!;
		//"(?=^.{6,10}$) (?=.*\\d) (?=. *[a-z]) (?=.*[A-Z]) (?=.*[ !@#$%^&amp; * ()_+}{&quot; : ; '?/&gt; .&lt;,]) (?!. *\\s).*$"||password must have 1 uppercase,1 lowercase,1 number , 1 nonalphanumeric, and at least 6 characters
	}
}
