namespace Application.Dtos
{
    public class DetailedStoreDto : StoreDto
    {
        public CompanyDto Company { get; set; } = null!;
    }
}