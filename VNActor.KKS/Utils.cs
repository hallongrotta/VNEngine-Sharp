using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;

namespace VNActor.KKS
{
    class Utils
    {
        public static byte[] SerializeData<T>(T item)
        {
            try
            {
                return MessagePackSerializer.Serialize(item, StandardResolver.Instance);
            }
            catch (FormatterNotRegisteredException)
            {
                return MessagePackSerializer.Serialize(item, ContractlessStandardResolver.Instance);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }


        public static T DeserializeData<T>(byte[] s)
        {
            try
            {
                return MessagePackSerializer.Deserialize<T>(s, StandardResolver.Instance);
            }
            catch (FormatterNotRegisteredException)
            {
                return MessagePackSerializer.Deserialize<T>(s, ContractlessStandardResolver.Instance);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
    }
}
