using Base.Service;


namespace Base.Security.Service
{
    public class PostService : BaseObjectService<Post>, IPostService
    {
        public PostService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}