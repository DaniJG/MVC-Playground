using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace DJRM.Common.Infrastructure.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string Entity { get; private set; }
        public int EntityId { get; private set; }

        public EntityNotFoundException(string entity, int entityId, Exception innerException)
            : base("", innerException)
        {
            Entity = entity;
            EntityId = entityId;
        }


        public override string Message
        {
            get
            {
                if (!string.IsNullOrEmpty(Entity))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("The ").Append(Entity).Append(" with ID = ").Append(EntityId).Append(" couldn't be found");
                    return sb.ToString();
                }
                return base.Message;
            }
        }
    }
}