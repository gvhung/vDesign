using Base.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Entities.Complex.KLADR
{
    [ComplexType]
    public class Address
    {
        public Address()
        {
            Region = new AddressObject();
            District = new AddressObject();
            City = new AddressObject();
            Street = new AddressObject();
            Building = new AddressObject();
        }

        public string CountryTitle { get; set; }
        public int CountryCode { get; set; }

        [DetailView("Индекс")]
        [MaxLength(10)]
        public string Zip { get; set; }

        [DetailView("Регион")]
        public AddressObject Region { get; set; }

        [DetailView("Район")]
        public AddressObject District { get; set; }

        [DetailView("Город")]
        public AddressObject City { get; set; }

        [DetailView("Улица")]
        public AddressObject Street { get; set; }

        [DetailView("Номер дома")]
        public AddressObject Building { get; set; }

        [DetailView("Корпус/Строение")]
        [MaxLength(10)]
        public string BuildingAdd { get; set; }

        [DetailView("Офис")]
        [MaxLength(100)]
        public string Office { get; set; }

        [DetailView("Офис")]
        public string FullAddress { get; set; }
    }
}
