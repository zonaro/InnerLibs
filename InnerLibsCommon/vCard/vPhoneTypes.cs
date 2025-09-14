using System.ComponentModel.DataAnnotations;

namespace Extensions.vCards
{
    public enum vPhoneTypes
    {
        [Display(Name = "Voice")]
        VOICE,
        FAX,
        MSG,
        PAGER,
        BBS,
        MODEM,
        CAR,
        ISDN,
        VIDEO,
        TEXT  
    }
}