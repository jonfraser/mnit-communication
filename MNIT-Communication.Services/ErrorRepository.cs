using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNIT.ErrorLogging;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Services
{
    public class ErrorRepository: IErrorRepository
    {
        private readonly IRepository repository;
        private readonly IRuntimeContext runtimeContext;

        public ErrorRepository(IRepository repository, IRuntimeContext runtimeContext)
        {
            this.repository = repository;
            this.runtimeContext = runtimeContext;
        }

        public void AddError<T>(IError<T> error)
        {
            Task.Run(() => AddErrorAsync(error));
        }

        public async Task AddErrorAsync<T>(IError<T> error)
        {
            var castError = error as IError<Guid>;
            var userId = (await runtimeContext.CurrentProfile()).Id;
            
            if(castError == null)
                throw new ArgumentException("This Error Repository only handles Errors with an Id of type Guid");
            
            var toPersist = new Domain.Error(castError, userId);

            await repository.Upsert(toPersist);
        }
    }

}
