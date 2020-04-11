using System;

namespace AddressService.Core.Dto
{
    public class AddressDetailsDto
    {
        public int Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Locality { get; set; }
        public string Postcode { get; set; }
        public int PostCodeId { get; set; }

        public DateTime LastUpdated { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(AddressLine1)}: {AddressLine1}, {nameof(AddressLine2)}: {AddressLine2}, {nameof(AddressLine3)}: {AddressLine3}, {nameof(Locality)}: {Locality}, {nameof(Postcode)}: {Postcode}, {nameof(PostCodeId)}: {PostCodeId}, {nameof(LastUpdated)}: {LastUpdated}";
        }
    }
}
