using System.ComponentModel.DataAnnotations;

namespace WebAPI_simple.Models.DTO
{
    public class AddAuthorRequestDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "Tên tác giả phải có độ dài tối thiểu 3 ký tự")]
        public string FullName { set; get; }
    }
}
