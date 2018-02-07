using AutoMapper;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public abstract class ViewModel<T1, T2> : ViewModel where T1 : class where T2 : class
    {
        public override void Map(IMapper mapper)
        {
            Map(mapper.Map<T1>(this), mapper.Map<T2>(this));
        }

        public abstract void Map(T1 message1, T2 message2);
    }

    public abstract class ViewModel<T> : ViewModel where T : class
    {
        public override void Map(IMapper mapper)
        {
            Map(mapper.Map<T>(this));
        }

        public abstract void Map(T message);
    }

    public abstract class ViewModel
    {
        public abstract void Map(IMapper mapper);
    }
}