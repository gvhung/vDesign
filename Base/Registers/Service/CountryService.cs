namespace Base.Registers.Service
{
    using Base.Registers.Entities;
    using Base.Service;

    public class CountryService : BaseObjectService<Country>, ICountryService
    {
        public CountryService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }
    }
}