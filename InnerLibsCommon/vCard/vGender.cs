using System.ComponentModel.DataAnnotations;

namespace Extensions.vCards
{
    public enum vGender
    {
        [Display(Name = "Male")]
        M,
        [Display(Name = "Female")]
        F,
        [Display(Name = "Other")]
        O,
        [Display(Name = "None")]
        N,
        [Display(Name = "Unknow")]
        U
    }
}