using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace SFA.DAS.EAS.Application.Messages
{
    public abstract class ViewModel<T> : ViewModel where T : class
    {
        [IgnoreMap]
        [Required]
        public T Message { get; set; }

        public override void Map(IMapper mapper)
        {
            Message = mapper.Map<T>(this);
        }
    }

    public abstract class ViewModel
    {
        public abstract void Map(IMapper mapper);
    }
}