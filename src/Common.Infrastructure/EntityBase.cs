using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DJRM.Common.Infrastructure
{
    public abstract class EntityBase
    {
        public EntityBase()
        {
            Id = EntityBase.GetNewId();
        }

        [ReadOnly(true)]
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            return obj is EntityBase
                    && this.ObjectType == ((EntityBase)obj).ObjectType
                    && this.Id == ((EntityBase)obj).Id;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 29) + this.ObjectType.GetHashCode();
            hash = (hash * 29) + this.Id.GetHashCode();
            return hash;
        }

        public abstract Type ObjectType { get; }

        public static bool operator ==(EntityBase entity1, EntityBase entity2)
        {
            if ((object)entity1 == null && (object)entity2 == null)
            {
                return true;
            }

            if ((object)entity1 == null || (object)entity2 == null)
            {
                return false;
            }

            if (entity1.ObjectType != entity2.ObjectType)
            {
                return false;
            }

            return entity1.Id == entity2.Id;
        }

        public static bool operator !=(EntityBase entity1, EntityBase entity2)
        {
            return !(entity1 == entity2);
        }

        public bool IsNew { get { return (this.Id < 1); } }

        private static int GetNewId()
        {
            lock (_newIntLock)
            {
                int newId = _lastId;
                if (ID_RESET_VALUE < _lastId)
                {
                    _lastId--;
                }
                else
                {
                    _lastId = -1;
                }
                return newId;
            }
        }
        private static int _lastId = -1;
        private static Object _newIntLock = new Object();
        private const int ID_RESET_VALUE = -150000;

    }
}
