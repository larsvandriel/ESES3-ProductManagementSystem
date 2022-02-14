using ProductManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Contracts
{
    public interface IKafkaContext<T>
    {
        void SendProductToKafkaTopic(string kafkaTopic, T entity);
    }
}
