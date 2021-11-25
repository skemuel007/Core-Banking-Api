using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Individual : BaseModel
    {
        [Key]
        public int IndividualCustId { get; set; }
        [Required]
        public string CustomerNumber { get; set; }
        //Required fields
        [Required]
        public string Title { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string OtherName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }

        public string Telephone { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Occupation { get; set; }
        public int FirstAccOfficerId { get; set; }

        public int PostCode { get; set; }

        public string Picture { get; set; }
        public string Signature { get; set; }
        public string Address { get; set; }


        public string MaritalStatus { get; set; }
        public int? NumberOfChildren { get; set; }
        public string HomeType { get; set; }


        public string HouseNumber { get; set; }
        public string City { get; set; }
        public string PostalAddress { get; set; }


        public string IdVerified { get; set; }
        public string IdNumber { get; set; }
        public DateTime IdDateIssued { get; set; }
        public DateTime IdDateExpire { get; set; }
        public string SSNumber { get; set; }
        public string TinNumber { get; set; }
        public string AddressVerified { get; set; }

        public int Status { get; set; }
        public string SecurityGroup { get; set; }
        public string BiometricIdNumber { get; set; }

        public string MotherMaidenName { get; set; }
        public string MotherFirstName { get; set; }
        public DateTime MotherDateOfBirth { get; set; }
        public string SpouseName { get; set; }
        public string SpouseOtherName { get; set; }
        public DateTime SpouseDateOfBirth { get; set; }

        //Next of kin details
        public string KName { get; set; }
        public string KAddress { get; set; }
        public string KRelation { get; set; }
        public string KPhone { get; set; }
        public string KEmail { get; set; }
        public string OtherContact { get; set; }
        public string Note { get; set; }

        //Alert
        public bool BroadcastAlert { get; set; }
        public bool SupportAlert { get; set; }
        //Related Doc
        public int? RelatedDocumentId { get; set; }
        [Required(ErrorMessage = "Country Id Required")]
        public int CountryId { get; set; }
        public CountryList CountryList { get; set; }
        //public CustomerType CustomerType { get; set; }
        //[Required(ErrorMessage = "Customer Type Id Required")]
        //public int CustomerTypeId { get; set; }
        [Required(ErrorMessage = "Target Id Required")]
        public int TargetId { get; set; }
        public Target Target { get; set; }
        [Required(ErrorMessage = "Rating Id Required")]
        public int RatingId { get; set; }
        public Rating Rating { get; set; }
        [Required(ErrorMessage = "Sector Id Required")]
        public int SectorId { get; set; }
        public Sector Sector { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }
}