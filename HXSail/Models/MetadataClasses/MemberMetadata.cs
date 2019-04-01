using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HXSail.Models;

namespace HXSail.Models
{
    // metadata for memebr class
    [ModelMetadataType(typeof(MemberMetadata))]
    public partial class Member : IValidatableObject
    {
        //self_validation for memebr class
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            SailContext _context = new SailContext();


            //trim all strings
            FirstName = FirstName.Trim();
            LastName = LastName.Trim();
            if (SpouseFirstName != null) SpouseFirstName = SpouseFirstName.Trim();
            if (SpouseLastName != null) SpouseLastName = SpouseLastName.Trim();
            if (Street != null) Street = Street.Trim();
            if (City != null ) City = City.Trim();
            if (ProvinceCode != null ) ProvinceCode = ProvinceCode.Trim();
            if (PostalCode != null) PostalCode = PostalCode.Trim();
            HomePhone = HomePhone.Trim();
            if (Email !=null) Email = Email.Trim();


            //capitalize designated fields
            FirstName = HXClassLibrary.HXValidations.HXCapitalize(FirstName);
            LastName = HXClassLibrary.HXValidations.HXCapitalize(LastName);
            SpouseFirstName = HXClassLibrary.HXValidations.HXCapitalize(SpouseFirstName);
            SpouseLastName = HXClassLibrary.HXValidations.HXCapitalize(SpouseLastName);
            Street = HXClassLibrary.HXValidations.HXCapitalize(Street);
            City = HXClassLibrary.HXValidations.HXCapitalize(City);


            // format full name
            if (SpouseFirstName == "")
                FullName = LastName + ", " + FirstName;
            else if (SpouseFirstName != "" && SpouseLastName != "")
                if (SpouseLastName == LastName)
                    FullName = LastName + ", " + FirstName + " & " + SpouseFirstName;
                else
                    FullName = LastName + ", " + FirstName + " & " + SpouseLastName + ", " + SpouseFirstName;
            else if (SpouseFirstName != "" && SpouseLastName == "")
                FullName = LastName + ", " + FirstName + " & " + SpouseFirstName;


            //validate province code
            Province province = null;
            string errorMessage = "";
            try
            {
                ProvinceCode = (ProvinceCode + "").ToUpper();
                province = _context.Province.Find(ProvinceCode);
                if (province == null)
                    errorMessage = "the province code is not on file";
            }
            catch (Exception ex)
            {
                errorMessage = $"fetching provinceCode error: {ex.GetBaseException().Message}";
                
            }
            if (errorMessage != "")
            {
                yield return new ValidationResult(
                    errorMessage,
                    new[] { "ProvinceCode" });
            }

            //validate postal code
            if (!string.IsNullOrEmpty(PostalCode))
            {
                if (string.IsNullOrEmpty(ProvinceCode))
                {
                    yield return new ValidationResult(
                        "province code is required when having the postal code",
                        new[] { "ProvinceCode" });
                }
                else
                {
                    if (province == null)
                    {
                        yield return new ValidationResult(
                            "The province code is not on file",
                            new[] { "ProvinceCode" });
                    }
                    else
                    {
                        if (province.CountryCode == "CA")
                        {
                            if (HXClassLibrary.HXValidations.HXPostalCodeValidation(PostalCode))
                                HXClassLibrary.HXValidations.HXPostalCodeFormat(PostalCode);
                            else yield return new ValidationResult(
                                "the postal code is invalid in Canada (it should follow the format: A1B 1E1)",
                                new[] { "PostalCode" });
                        }
                        else if (province.CountryCode == "US")
                        {
                            string postalCode = PostalCode;
                            if (HXClassLibrary.HXValidations.HXZipCodeValidation(ref postalCode))
                                PostalCode = postalCode;
                            else yield return new ValidationResult(
                                "the zip code is invalid in the US (it should have 5 or 9 digits)",
                                new[] { "PostalCode" });
                        }
                    }
                }
            }

            //validate home phone
            if (HXClassLibrary.HXValidations.HXExtractDigits(HomePhone).Length == 10)
            {
                HomePhone = HXClassLibrary.HXValidations.HXExtractDigits(HomePhone).Insert(3, "-").Insert(6, "-");
            }
            else
            {
                HomePhone = HXClassLibrary.HXValidations.HXExtractDigits(HomePhone);
                yield return new ValidationResult(
                "a valid phone number should have 10 digits",
                new[] { "HomePhone" });
            }

            //validate year joined
            if (YearJoined > Convert.ToInt32(DateTime.Now.Year))
            {
                yield return new ValidationResult(
                    "year joined can not be in the future", 
                    new[] { "YearJoined" });
            }

            var memberId = _context.Member.Find(MemberId);
            if (memberId == null)
            {
                if (YearJoined == null)
                    yield return new ValidationResult(
                        "year joined can only be null for existing records.",
                        new[] { "YearJoined" });
            }

            //validate using canada post
            if (UseCanadaPost == false)
            {
                if (string.IsNullOrEmpty(Email))
                    yield return new ValidationResult(
                        "Must have a valid email if not using Canada Post",
                        new[] {"Email" });
            }
            else
            {
                if (string.IsNullOrEmpty(Street) || string.IsNullOrEmpty(City) ||
                    string.IsNullOrEmpty(ProvinceCode) || string.IsNullOrEmpty(PostalCode))
                    yield return new ValidationResult(
                        "street, city, province code, postal code can not be null when using Canada Post.",
                        new[] {"UseCanadaPost" });
            }


            yield return ValidationResult.Success;
        }
    }

    //metadata for member class
    public class MemberMetadata
    {
        public int MemberId { get; set; }
        public string FullName { get; set; }

        [Display(Name = "First Name")]
        [Required] 
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Spouse First Name")]
        public string SpouseFirstName { get; set; }

        [Display(Name = "Spouse Last Name")]
        public string SpouseLastName { get; set; }

        [Display(Name = "Street Address")]
        public string Street { get; set; }

        public string City { get; set; }

        [Display(Name = "Province Code")]
        public string ProvinceCode { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Home Phone")]
        [Required]
        public string HomePhone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Year Joined")]
        public int? YearJoined { get; set; }

        [Display(Name = "Comments")]
        public string Comment { get; set; }

        [Display(Name = "Task Exempt?")]
        public bool TaskExempt { get; set; }

        [Display(Name = "Use Canada Post?")]
        public bool UseCanadaPost { get; set; }
    }
}
