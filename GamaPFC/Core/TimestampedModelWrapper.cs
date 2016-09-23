using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class TimestampedModelWrapper<T> : ModelWrapper<T>
    {
        public TimestampedModelWrapper(T model) : base(model)
        {
        }

        public DateTime CreatedAt
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime? UpdatedAt
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }
    }
}
