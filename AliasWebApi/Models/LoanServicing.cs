using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class LoanServicing : BaseModel
    {
        [Key]
        public int LoanServicingCustId { get; set; }

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
        public string DateOfBirth { get; set; }

        public string Telephone { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public int Occupation { get; set; }

        public int FirstAccOfficerId { get; set; }

        public int SecondAccOfficerId { get; set; }

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
        public DateTime? IdDateIssued { get; set; }
        public DateTime? IdDateExpire { get; set; }
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

        //Employement Details

        public string NameOfEmployer { get; set; }
        public DateTime DateEmployed { get; set; }
        public string CurrentPosition { get; set; }
        public string CurrentStation { get; set; }
        public string AverageSalary { get; set; }
        public string Banker { get; set; }

        //Alert
        public bool TransactionAlert { get; set; }
        public bool GeneralBroadAlert { get; set; }
        public bool InvestmentAlert { get; set; }
        public bool LoanPaymentAlert { get; set; }
        public bool SupportAlert { get; set; }

        //Related Doc
        public int? RelatedDocumentId { get; set; }

        //Relational informtion
        [Required(ErrorMessage = "Country Id Required")]
        public int CountryId { get; set; }
        public CountryList CountryList { get; set; }

        public CustomerType CustomerType { get; set; }
        [Required(ErrorMessage = "Customer Id Required")]
        public int CustomerTypeId { get; set; }
        [Required(ErrorMessage = "Target Id Required")]
        public int TargetId { get; set; }
        public Target Target { get; set; }
        [Required(ErrorMessage = "Rating Id Required")]
        public int RatingId { get; set; }
        public Rating Rating { get; set; }
        [Required(ErrorMessage = "Sector Id Required")]
        public int SectorId { get; set; }
        public Sector Sector { get; set; }
    }
}